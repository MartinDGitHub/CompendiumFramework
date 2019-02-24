using CF.Common.DI;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;

namespace CF.Infrastructure.DI
{
    internal class Container : IContainer
    {
        private readonly SimpleInjector.Container _simpleInjectorContainer;

        public Lifetime DefaultLifetime => Lifetime.Transient;

        public Container(SimpleInjector.Container simpleInjectorContainer)
        {
            this._simpleInjectorContainer = simpleInjectorContainer;
        }

        public void Dispose()
        {
            this._simpleInjectorContainer.Dispose();
        }

        public void Register(Type serviceType, Type implementationType)
        {
            this.Register(serviceType, implementationType, this.DefaultLifetime);
        }

        public void Register(Type serviceType, Type implementationType, Lifetime lifetime)
        {
            this._simpleInjectorContainer.Register(serviceType, implementationType, this.ResolveLifestyle(lifetime));
        }

        public void Register<TService, TImplementation>() 
            where TService : class
            where TImplementation : class, TService
        {
            this.Register<TService, TImplementation>(this.DefaultLifetime);
        }

        public void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService
        {
            this._simpleInjectorContainer.Register<TService, TImplementation>(this.ResolveLifestyle(lifetime));
        }

        private Lifestyle ResolveLifestyle(Lifetime lifetime)
        {
            Lifestyle lifestyle;

            switch (lifetime)
            {
                case Lifetime.Transient:
                    lifestyle = Lifestyle.Transient;
                    break;
                case Lifetime.Scoped:
                    lifestyle = new AsyncScopedLifestyle();
                    break;
                case Lifetime.Singleton:
                    lifestyle = Lifestyle.Singleton;
                    break;
                default:
                    throw new Exception($"The specified lifetime [{Enum.GetName(typeof(Lifetime), lifetime)}] is unsuported.");
            }

            return lifestyle;
        }
    }
}
