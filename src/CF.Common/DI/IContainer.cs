using System;
using System.Reflection;

namespace CF.Common.DI
{
    public interface IContainer : IDisposable
    {
        Lifetime DefaultLifetime { get; }

        void RegisterInstance<TService>(TService instance) where TService : class;

        void Register(Type implementationType, Lifetime lifetime);

        void Register(Type serviceType, Type implementationType);

        void Register(Type serviceType, Type implementationType, Lifetime lifetime);

        void Register<TImplementation>(Lifetime lifetime);

        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService;
        /*
        void Register(Type serviceType, Assembly implementationAssembly);

        void Register(Type serviceType, Assembly implementationAssembly, Lifetime lifetime);
        */
    }
}
