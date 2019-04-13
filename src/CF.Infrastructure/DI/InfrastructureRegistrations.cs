using CF.Application.Repositories;
using CF.Common.Correlation;
using CF.Common.DI;
using CF.Common.Logging;
using CF.Infrastructure.Correlation;
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
            this.Container.RegisterInstance(this.Container);
            this.Container.RegisterInstance((IServiceLocatorContainer)this.Container);

            // Only one non-generic logger should be registered per application lifetime.
            this.Container.Register<ILogger, Logger>(Lifetime.Singleton);
            this.Container.Register(typeof(ILogger<>), typeof(Logger<>), Lifetime.Singleton);

            // Resolve repositories per request to ensure that authentication dependencies are not stale.
            this.RegisterDerivedInterfaceImplementations<IRepository>(this.Container, Lifetime.Scoped);

            // Resolve one correlation ID provider per request to relate log entries created during the request.
            this.Container.Register<IScopedCorrelationIdProvider, ScopedCorrelationIdProvider>(Lifetime.Scoped);
        }
    }
}
