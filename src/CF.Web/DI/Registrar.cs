using CF.Application.DI;
using CF.Common.DI;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Text;

namespace Web.DI
{
    public class Registrar : IRegistrar
    {
        private readonly Container _container;

        public Registrar(Container container) =>
            this._container = container ?? throw new ArgumentNullException(nameof(container));

        void IRegistrar.Register<TService, TImplementation>()
        {
            this._container.Register<TService, TImplementation>();
        }

        void IRegistrar.Register<TService, TImplementation>(Lifetime lifetime)
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

            this._container.Register<TService, TImplementation>(lifestyle);
        }
    }
}
