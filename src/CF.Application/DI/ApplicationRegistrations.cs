using CF.Application.Repositories;
using CF.Application.Services;
using CF.Common.DI;

namespace CF.Application.DI
{
    internal class ApplicationRegistrations : RegistrationsBase, IRegistrations
    {
        public ApplicationRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterServices()
        {
            this.RegisterPolicies();

            // Resolve repositories per request to ensure that authentication dependencies are not stale.
            this.RegisterDerivedInterfaceImplementations<IRepository>(this.Container, Lifetime.Scoped);

            // Resolve services per request to ensure that dependencies are not stale.
            this.RegisterDerivedInterfaceImplementations<IService>(this.Container, Lifetime.Scoped);
        }
    }
}
