using CF.Common.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.DI
{
    class CommonRegistrations : IRegistrations
    {
        public void RegisterServices(IRegistrar registrar)
        {
            if (registrar == null) throw new ArgumentNullException(nameof(registrar));

            registrar.Register<IScopedMessageRecorder, ScopedMessageRecorder>(Lifetime.Scoped);
        }
    }
}
