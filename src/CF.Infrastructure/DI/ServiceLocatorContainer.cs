using Microsoft.Extensions.DependencyInjection;
using System;

namespace CF.Infrastructure.DI
{
    internal class ServiceLocatorContainer : IServiceLocatorContainer
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceLocatorContainer(IServiceProvider serviceProvider)
        {
            this._serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public TService GetInstance<TService>() where TService : class
        {
            return this._serviceProvider.GetService(typeof(TService)) as TService;
        }

        public object GetInstance(Type serviceType)
        {
            return this._serviceProvider.GetService(serviceType);
        }
    }
}
