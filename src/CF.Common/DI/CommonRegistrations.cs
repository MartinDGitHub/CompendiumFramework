using CF.Common.Correlation;
using CF.Common.Messaging;

namespace CF.Common.DI
{
    internal class CommonRegistrations :  RegistrationsBase, IRegistrations
    {
        public CommonRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterServices()
        {
            // Messages are recorded to the scope of an operation.
            this.Container.Register<IScopedMessageRecorder, ScopedMessageRecorder>(Lifetime.Scoped);

            this.RegisterPolicies();
        }
    }
}
