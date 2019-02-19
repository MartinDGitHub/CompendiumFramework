using CF.Common.Config;
using CF.Common.DI;
using CF.WebBootstrap.Config;
using System;

namespace CF.WebBootstrap.DI
{
    class WebBootstrapRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IFooConfig, FooConfig>(Lifetime.Scoped);
        }
    }
}
