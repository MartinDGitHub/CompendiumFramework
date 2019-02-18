using CF.Application.DI;
using SimpleInjector;
using SimpleInjector.Integration.Web;
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
    }
}
