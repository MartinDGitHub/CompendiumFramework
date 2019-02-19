using CF.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.DI
{
    class CommonRegistrations : IRegistrations
    {
        public void RegisterServices(IContainer container)
        {
            if (container == null) throw new ArgumentNullException(nameof(container));

            container.Register<IScopedMessageRecorder, ScopedMessageRecorder>(Lifetime.Scoped);
        }
    }
}
