using CF.Infrastructure.DI;
using CF.Web.AspNetCore.Config.Sections;
using CF.Web.AspNetCore.Extensions.ServiceCollection;
using CF.Web.AspNetCore.Filters;
using CF.Web.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using AuthenticationConstants = CF.Web.AspNetCore.Authentication.Constants;
using CorsConstants = CF.Web.AspNetCore.Cors.Constants;

namespace CF.WebBootstrap
{
    public static class Startup
    {
        // Called before configure to set up the DI container.
        public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Read configuration for the purpose of this method. Configuration for DI injection via Options happens next.
            // Although inconvenient, this is in accordance with Microsoft guidance:
            //  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-3.0
            //  "Don't use IOptions<TOptions> or IOptionsMonitor<TOptions> in Startup.ConfigureServices. An inconsistent options state may exist due to the ordering of service registrations."
            Root rootConfig = new Root();
            configuration.Bind(rootConfig);

            // Bootstrap configuration before adding custom services configuration that may rely on configuration.
            services.AddCustomConfig(configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // Configure the CORS policy.
            // NOTE: these must be applied explicity on an opt-in basis at the controller or action level using the EnableCors attribute.
            // i.e. [EnableCors(Cors.Constants.Policies.Api)]
            services.AddCors(options =>
            {
                foreach (var policy in CorsConstants.Policies.All)
                {
                    if (rootConfig.Cors.OriginsByPolicy.ContainsKey(policy))
                    {
                        options.AddPolicy(policy, builder =>
                        {
                            builder.WithOrigins(rootConfig.Cors.OriginsByPolicy[policy] ?? Array.Empty<string>());
                        });
                    }
                }
            });

            // Use MVC for presentation and Web API.
            services.AddMvc(config =>
            {
                // By default, only permit authenticated users to access controller actions.
                var policy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(AuthenticationConstants.DefaultAuthenticationScheme)
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
            .SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

            // Support ordinary MVC controllers with views and Razor pages.
            services.AddControllersWithViews()
                // Configure native JSON behavior to be lenient (more-compatible) and somewhat align with JSON.NET.
                .AddJsonOptions(options =>
                {
                    // Deserialize case-insensitive similar to JSON.NET so that JavaScript camelCase property values
                    // can be set on C# class PascalCase properties.
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    // Ignore comments, similar to JSON.NET.
                    options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
                });
            services.AddRazorPages();

            // Support health checks.
            services.AddHealthChecks();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(config =>
            {
                config.RootPath = "ClientApp/build";
            });

            // Use extended API versioning.
            services.AddApiVersioning();

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

        // Called after ConfigureServices to apply configuration.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
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

            // Use the MVC pattern for routing requests to actions.
            // We use attribute routing instead of conventional routing for the following reasons:
            // 1) Makes routing deliberate and declarative.
            // 2) Consistency of routing specification across Web API and MVC actions.
            // 3) Permits deriving from Controller in the CF.Web.AspNetCore project.
            // NOTE: this must come before CORS and auth "Use"ings.
            app.UseRouting();

            // CORS is applied on an opt-in basis through the EnableCors attribute. Otherwise, we would app.UseCors here.

            // Secure the application with authentication and authorization.
            app.UseAuthentication();
            app.UseAuthorization();

            // Map endpoints.
            // NOTE: this must come after CORS and auth "Use"ings.
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
                endpoints.MapHealthChecks("/health");
            });

            // Use React as the SPA. This should come after MVC, so that controller action routing
            // takes precedence.
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    // To optimize change-build-run cycles in local development, proxy to the SPA when running under IIS Express.
                    // This assumes that the SPA has been started manually by:
                    // 1) opening the ClientApp folder in a command prompt
                    // 2) running the NPM development server: > npm start
                    // otherwise, the following error will occur:
                    // HttpRequestException: Failed to proxy the request to http://localhost:3000/, because the request to the proxy target failed. Check that the proxy target server is running and accepting requests to http://localhost:3000/.
                    //
                    // NOTE: 
                    // This does not work when running under IIS, due to the proxying not handling the application root (e.g. https://localhost/CF.Web/) when proxying.
                    // When running under IIS, run the NPM development server, and access the URL directly.
                    //
                    // There is a pull request to address this in version 3.3: https://github.com/facebook/create-react-app/pull/7259
                    spa.UseProxyToSpaDevelopmentServer("http://localhost:3000");
                }
            });
        }
    }
}
