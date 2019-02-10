using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.DI
{
    public interface IRegistrations
    {
        void RegisterServices(IRegistrar registrar);
    }
}
