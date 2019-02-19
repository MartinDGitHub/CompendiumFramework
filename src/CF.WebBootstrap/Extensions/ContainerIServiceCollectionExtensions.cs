﻿using CF.Infrastructure.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using SimpleInjector;
using SimpleInjector.Integration.AspNetCore.Mvc;
using SimpleInjector.Lifestyles;

namespace CF.WebBootstrap.Extensions
{
    public static class ContainerIServiceCollectionExtensions
    {
        public static void AddCustomContainer(this IServiceCollection services)
        {
            var container = new Container();
            var containerRegistry = new ContainerRegistry<Container>();

            containerRegistry.RegisterContainer(container);

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSingleton<IControllerActivator>(new SimpleInjectorControllerActivator(container));

            services.AddSingleton<IViewComponentActivator>(new SimpleInjectorViewComponentActivator(container));

            services.EnableSimpleInjectorCrossWiring(container);

            services.UseSimpleInjectorAspNetRequestScoping(container);
        }
    }
}
