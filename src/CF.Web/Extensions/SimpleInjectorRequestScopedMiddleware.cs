﻿using Microsoft.AspNetCore.Http;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CF.Web.Extensions
{
    public class SimpleInjectorRequestScopedMiddleware : IMiddleware
    {
        private Container _container;

        public SimpleInjectorRequestScopedMiddleware(Container container)
        {
            this._container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using (AsyncScopedLifestyle.BeginScope(this._container))
            {
                await next(context);
            }
        }
    }
}
