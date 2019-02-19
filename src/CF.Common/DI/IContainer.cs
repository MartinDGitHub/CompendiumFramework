using System;

namespace CF.Common.DI
{
    public interface IContainer : IDisposable
    {
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
    }
}
