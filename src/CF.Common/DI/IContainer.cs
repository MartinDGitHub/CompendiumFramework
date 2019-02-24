using System;

namespace CF.Common.DI
{
    public interface IContainer : IDisposable
    {
        Lifetime DefaultLifetime { get; }

        void Register(Type serviceType, Type implementationType);

        void Register(Type serviceType, Type implementationType, Lifetime lifetime);

        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
    }
}
