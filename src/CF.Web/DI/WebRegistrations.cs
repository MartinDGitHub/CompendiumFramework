using CF.Core.Config;
using CF.Core.DI;
using CF.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.DI
{
    public class WebRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IFooConfig, FooConfig>();
        }
    }
}
