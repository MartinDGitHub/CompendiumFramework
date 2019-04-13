using CF.Infrastructure.DI;
using CF.WebBootstrap.Authentication;
using CF.WebBootstrap.Extensions.ApplicationBuilder;
using CF.WebBootstrap.Extensions.ServiceCollection;
using CF.WebBootstrap.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;

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
                config.Filters.Add(new AuthorizeFilter(policy));
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

            // Add a custom IoC/DI container and perform the custom registrations to set up the container.
            services.AddCustomContainer();

            // Add custom authorization and the authentication that authorization depends on.
            services.AddCustomAuthorization();
        }


        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Use Simple Injector to register custom middleware to ensure it is present for verification
            // and to use the Simple Injector container.
            var container = new ContainerRegistry<Container>().ContainerImpl;

            // Register middleware that creates a request scope for subsequent middlewares that depend on request-scoped instances.
            app.UseMiddleware<AsyncScopedLifestyleMiddleware>(container);

            // Register logging enrichment middleware for subsequent middlewares that log.
            app.UseMiddleware<LoggerScopesMiddleware>(container);

            // Globally handle any exceptions raised by subsequent middlewares. This should come after scoped and logging middleware
            // so that it can inject scoped dependencies and log.
            app.UseMiddleware<GlobalExceptionHandlerMiddleware>(container);

            // Wire up a custom IoC/DI container integrated with MVC and the native .NET Core container.
            app.UseCustomContainer(env);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                // Indicate to browsers that all interactions must be over HTTPS.
                app.UseHsts();
            }

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
