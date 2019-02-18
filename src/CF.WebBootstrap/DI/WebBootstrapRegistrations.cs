using CF.Common.Config;
using CF.Common.DI;
using CF.Common.Logging;
using CF.WebBootstrap.Logging;
using System;

namespace CF.WebBootstrap.DI
{
    class WebBootstrapRegistrations: IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IFooConfig, FooConfig>(Lifetime.Scoped);

            registrar.Register<ILogger, Logger>(Lifetime.Singleton);
        }
    }
}
