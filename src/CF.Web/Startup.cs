using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.Webpack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using System.IO;
using System.Reflection;
using SimpleInjector.Lifestyles;
using CF.Core.Config;
using CF.Core.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using SimpleInjector.Integration.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Web.Settings;

namespace Web
{
    public class Startup
    {
        private readonly Container _container = new Container();

        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddApiVersioning();

            // Add custom logging sections.
            services.Configure<CFOptions>(this.Configuration);

            this.IntegrateSimpleInjector(services);
        }

        private void IntegrateSimpleInjector(IServiceCollection services)
        {
            this._container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(
                new SimpleInjectorControllerActivator(this._container));
            services.AddSingleton<IViewComponentActivator>(
                new SimpleInjectorViewComponentActivator(this._container));

            services.EnableSimpleInjectorCrossWiring(this._container);
            services.UseSimpleInjectorAspNetRequestScoping(this._container);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            this.InitializeContainer(app);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions
                {
                    HotModuleReplacement = true
                });

                this._container.Verify();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute(
                    name: "spa-fallback",
                    defaults: new { controller = "Home", action = "Index" });
            });
        }

        private void InitializeContainer(IApplicationBuilder app)
        {
            // Wire up for MVC components.
            this._container.RegisterMvcControllers(app);
            this._container.RegisterMvcViewComponents(app);

            // Wire up configuration
            this._container.Register<IFooConfig, FooConfig>();

            // Wire up with built-in DI container.
            this._container.AutoCrossWireAspNetComponents(app);

            // Permit framework assemblies to wire up services they own.
            var assemblies = new DirectoryInfo(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .GetFiles("CF.*.dll")
                .Select(x => Assembly.Load(AssemblyName.GetAssemblyName(x.FullName)));
            this._container.RegisterPackages(assemblies);
        }
    }
}
