using CF.Common.Config;
using CF.Common.DI;
using CF.WebBootstrap.Config;
using System;

namespace CF.WebBootstrap.DI
{
    class WebBootstrapRegistrations: IRegistrations
    {
        public void RegisterServices(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Register<IFooConfig, FooConfig>(Lifetime.Scoped);
        }
    }
}
