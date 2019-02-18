using System;
using System.Collections.Generic;
using System.Text;

namespace CF.Common.DI
{
    public interface IRegistrar
    {
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
    }
}
