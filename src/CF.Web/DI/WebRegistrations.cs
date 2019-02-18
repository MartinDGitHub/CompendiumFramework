using CF.Application.Config;
using CF.Application.DI;
using CF.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.DI
{
    class WebRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IFooConfig, FooConfig>();
        }
    }
}
