using CF.Common.DI;
//using SimpleInjector;
//using SimpleInjector.Lifestyles;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace CF.Infrastructure.DI
{
    internal class Container : IContainer
    {
        /*
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

        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            this._simpleInjectorContainer.RegisterInstance<TService>(instance);
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

        public void Register(Type serviceType, Assembly implementationAssembly)
        {
            this.Register(serviceType, implementationAssembly, this.DefaultLifetime);
        }

        public void Register(Type serviceType, Assembly implementationAssembly, Lifetime lifetime)
        {
            this._simpleInjectorContainer.Register(serviceType, implementationAssembly, this.ResolveLifestyle(lifetime));
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

        TService IServiceLocatorContainer.GetInstance<TService>()
        {
            return this._simpleInjectorContainer.GetInstance<TService>();
        }

        object IServiceLocatorContainer.GetInstance(Type serviceType)
        {
            return this._simpleInjectorContainer.GetInstance(serviceType);
        }
        */
        private readonly IServiceCollection _serviceCollection;

        public Lifetime DefaultLifetime => Lifetime.Transient;

        public Container(IServiceCollection serviceCollection)
        {
            this._serviceCollection = serviceCollection ?? throw new ArgumentNullException(nameof(serviceCollection));
        }

        public void Dispose()
        {
            // No need to dispose the ASP.NET Core container.
        }

        public void RegisterInstance<TService>(TService instance) where TService : class
        {
            this._serviceCollection.AddSingleton(instance);
        }

        public void Register(Type implementationType, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    this._serviceCollection.AddTransient(implementationType);
                    break;
                case Lifetime.Scoped:
                    this._serviceCollection.AddScoped(implementationType);
                    break;
                case Lifetime.Singleton:
                    this._serviceCollection.AddSingleton(implementationType);
                    break;
                default:
                    throw new InvalidOperationException($"The lifetime [{lifetime}] is unsupported for container registrations.");
            }
        }

        public void Register(Type serviceType, Type implementationType)
        {
            this.Register(serviceType, implementationType, this.DefaultLifetime);
        }

        public void Register(Type serviceType, Type implementationType, Lifetime lifetime)
        {
            switch (lifetime)
            {
                case Lifetime.Transient:
                    this._serviceCollection.AddTransient(serviceType, implementationType);
                    break;
                case Lifetime.Scoped:
                    this._serviceCollection.AddScoped(serviceType, implementationType);
                    break;
                case Lifetime.Singleton:
                    this._serviceCollection.AddSingleton(serviceType, implementationType);
                    break;
                default:
                    throw new InvalidOperationException($"The lifetime [{lifetime}] is unsupported for container registrations.");
            }
        }


        public void Register<TImplementation>(Lifetime lifetime)
        {
            this.Register(typeof(TImplementation), lifetime);
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
            switch (lifetime)
            {
                case Lifetime.Transient:
                    this._serviceCollection.AddTransient<TService, TImplementation>();
                    break;
                case Lifetime.Scoped:
                    this._serviceCollection.AddScoped<TService, TImplementation>();
                    break;
                case Lifetime.Singleton:
                    this._serviceCollection.AddSingleton<TService, TImplementation>();
                    break;
                default:
                    throw new InvalidOperationException($"The lifetime [{lifetime}] is unsupported for container registrations.");
            }
        }
    }
}
