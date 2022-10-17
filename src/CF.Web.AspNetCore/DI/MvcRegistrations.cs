using CF.Common.DI;
using CF.Web.AspNetCore.Helpers;

namespace CF.Web.AspNetCore.DI
{
    internal class MvcRegistrations : RegistrationsBase, IRegistrations
    {
        public MvcRegistrations(IContainer container) : base(container)
        {
        }

        public void RegisterServices()
        {
            Container.Register<IPartialViewHelper, PartialViewHelper>(Lifetime.Scoped);
        }
    }
}
