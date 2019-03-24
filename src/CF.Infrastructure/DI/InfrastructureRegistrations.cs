using CF.Application.Repositories;
using CF.Common.Config;
using CF.Common.DI;
using CF.Common.Logging;
using CF.Infrastructure.Logging;

namespace CF.Infrastructure.DI
{
    internal class InfrastructureRegistrations : RegistrationsBase, IRegistrations
    {
        public InfrastructureRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterServices()
        {
            // Register the container itself for the duration of the application lifetime.
            this.Container.Register<IContainer, Container>(Lifetime.Singleton);

            // Only one logger should be registered per application lifetime.
            this.Container.Register<ILogger, Logger>(Lifetime.Singleton);
            this.Container.Register(typeof(ILogger<>), typeof(Logger<>), Lifetime.Singleton);

            // Register configuration per request to ensure that any configuration changes are discovered 
            // and to keep authorization consistent throughout a request.
            this.RegisterDerivedInterfaceImplementations<IConfig>(this.Container, Lifetime.Scoped);

            // Resolve repositories per request to ensure that authentication dependencies are not stale.
            this.RegisterDerivedInterfaceImplementations<IRepository>(this.Container, Lifetime.Scoped);
        }
    }
}
