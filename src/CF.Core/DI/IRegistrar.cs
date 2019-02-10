using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Core.DI
{
    public interface IRegistrar
    {
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;
    }
}
