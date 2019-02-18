using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Application.DI
{
    public interface IRegistrations
    {
        void RegisterServices(IRegistrar registrar);
    }
}
