using CF.Infrastructure.DI;
using CF.Web.AspNetCore.Authentication;
using CF.Web.AspNetCore.Extensions.ServiceCollection;
using CF.Web.AspNetCore.Filters;
using CF.Web.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CF.WebBootstrap
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Use MVC for presentation and Web API.
            services.AddMvc(config =>
            {
                // By default, only permit authenticated users to access controller actions.
                var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(Constants.DefaultAuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();

                // Add framework filters.
                config.Filters.Add(new AuthorizeFilter(policy));

                // Add custom filters.
                config.Filters.Add<ApiActionResultPackageActionFilter>();
                config.Filters.Add<RedirectMessageActionFilter>();
            })
            // The cookie-based temp data provider is used to round-trip messages using the post-redirect-get (PRG) pattern.
            .AddCookieTempDataProvider(x =>
            {
                // Protect the cookie against client-side access.
                x.Cookie.HttpOnly = true;
                // Mark as essential to system function so that the cookie is used regardless of the cookie
                // policy being accepted.
                x.Cookie.IsEssential = true;
            })
            // Support a specific compaitibility level so that upgrades are be deliberate.
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(config =>
            {
                config.RootPath = "ClientApp/build";
            });

            // Use extended API versioning.
            services.AddApiVersioning();

            // Bootstrap configuration before adding custom services configuration that may rely on configuration.
            services.AddCustomConfig(configuration);

            // Ensure the service provider is available for rare cases where 
            // we need to manually resolve instances from the DI container.
            services.AddSingleton<IServiceProvider>(services.BuildServiceProvider(validateScopes: true));

            // Add a custom IoC/DI container and perform the custom registrations to set up the container.
            services.AddCustomContainer();

            // The HTTP Context is required for accessing the claims principal for auth concerns.
            services.AddHttpContextAccessor();

            // Add custom authorization and the authentication that authorization depends on.
            services.AddCustomAuthorization();

            // Add a local (in-memory) cache provider.
            services.AddLazyCache();
        }


        public static void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            // Register logging enrichment middleware for subsequent middlewares that log.
            app.UseMiddleware<LoggerScopesMiddleware>();

            if (env.IsDevelopment())
            {
                // These should come before the global exception handler to catch and handle exceptions
                // rethrown from it in a development environment.
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // Validate the the container registrations in development mode. This assumes there are no
                // runtime registrations that would change the contents of the container in other environments.
                // NOTE: this operation can be expensive. Consider running it as an integration test.
                var containerRegistry = new ContainerRegistry<IServiceCollection>();
                containerRegistry.Container.EnsureValid(serviceProvider);
            }

            // Globally handle any exceptions raised by subsequent middlewares. This should come after:
            //  1) scoped middleware to ensure injected dependencies are properly scoped;
            //  2) logging middleware to ensure scope properties are setup;
            //  3) developer exception/error page middlewares to permit them to display errors for local development.
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

            // Indicate to browsers that all interactions must be over HTTPS.
            app.UseHsts();

            // Redirect HTTP to HTTPS prior to processing requests.
            app.UseHttpsRedirection();

            // Serve up static content prior to authentication/authorization.
            app.UseStaticFiles();

            // Enable cookies (with consent) for authentication.
            // NOTE: cookies that are not marked as essential will not be set without consent.
            app.UseCookiePolicy();

            // Secure the application with authentication, and by extension, authorization.
            app.UseAuthentication();

            // Use the MVC pattern for routing requests to actions.
            // We use attribute routing instead of conventional routing for the following reasons:
            // 1) Makes routing deliberate and declarative.
            // 2) Consistency of routing specification across Web API and MVC actions.
            // 3) Permits deriving from Controller in the CF.Web.AspNetCore project.
            app.UseMvc();

            // Use React as the SPA. This should come after MVC, so that controller action routing
            // takes precedence.
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    // NOTE: 
                    // The following error may occur when running under IIS if the application pool has insufficient permissions. Running locally as Local System 
                    // will resolve this due to the elevated permissions of that account. IMPORTANT: always run the application pool under minimum permissions in 
                    // deployed environments, especially production.
                    // Error: EPERM: operation not permitted, mkdir 'C:\Windows\system32\config\systemprofile\AppData\Roaming\npm'
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
