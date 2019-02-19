using CF.Common.DI;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;

namespace CF.Infrastructure.DI
{
    internal class Container : IContainer
    {
        private readonly SimpleInjector.Container _simpleInjectorContainer;

        public Container(SimpleInjector.Container simpleInjectorContainer)
        {
            this._simpleInjectorContainer = simpleInjectorContainer;
        }

        public void Dispose()
        {
            this._simpleInjectorContainer.Dispose();
        }

        void IContainer.Register<TService, TImplementation>()
        {
            this._simpleInjectorContainer.Register<TService, TImplementation>();
        }

        void IContainer.Register<TService, TImplementation>(Lifetime lifetime)
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

            this._simpleInjectorContainer.Register<TService, TImplementation>(lifestyle);
        }
    }
}
