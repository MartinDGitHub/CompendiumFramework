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
                config.Filters.Add<WebActionResultCookieMessageActionFilter>();
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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


        public static void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // Register logging enrichment middleware for subsequent middlewares that log.
            app.UseMiddleware<LoggerScopesMiddleware>();

            // For local development, permit
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();

                // Validate the the container registrations in development mode. This assumes there are no
                // runtime registrations that would change the contents of the container in other environments.
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
            app.UseCookiePolicy();

            // Secure the application with authentication, and by extension, authorization.
            app.UseAuthentication();

            // Use the MVC pattern for routing requests to actions.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");

            });

            // Use React as the SPA.
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
