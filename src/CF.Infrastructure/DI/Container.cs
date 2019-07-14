using CF.Common.Constants;
using CF.Common.DI;
using CF.Infrastructure.DI.Verification;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CF.Infrastructure.DI
{
    internal class Container : IContainer
    {
        private static IDictionary<Lifetime, ServiceLifetime> ServiceLifetimeByLifetime = new Dictionary<Lifetime, ServiceLifetime>
        {
            { Lifetime.Transient, ServiceLifetime.Transient },
            { Lifetime.Scoped, ServiceLifetime.Scoped },
            { Lifetime.Singleton, ServiceLifetime.Singleton },
        };

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

        public void Register(Type serviceType, Type implementationType)
        {
            this.Register(serviceType, implementationType, this.DefaultLifetime);
        }

        public void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            this.Register<TService, TImplementation>(this.DefaultLifetime);
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

        public void Register(Type serviceType, Assembly implementationAssembly)
        {
            this.Register(serviceType, implementationAssembly, this.DefaultLifetime);
        }

        public void Register<TService>(Assembly implementationAssembly) where TService : class
        {
            this.Register(typeof(TService), implementationAssembly);
        }

        public void Register(Type serviceType, Assembly implementationAssembly, Lifetime lifetime)
        {
            // Implementations can't be directly registered to an open-generic (e.g. Foo<>) ; instead, they must be
            // registered to their specified implementation types (e.g. Foo<Bar>).
            if (serviceType.IsGenericTypeDefinition)
            {
                this.RegisterAsImplementedInterfaces(serviceType, implementationAssembly, lifetime);
            }
            else
            {
                this._serviceCollection.Scan(scan => scan
                    .FromAssemblies(implementationAssembly)
                    .AddClasses(classes => classes.AssignableTo(serviceType))
                    .As(serviceType)
                    .WithLifetime(ServiceLifetimeByLifetime[lifetime])
                );
            }
        }

        public void Register<TService>(Assembly implementationAssembly, Lifetime lifetime) where TService : class
        {
            this.Register(typeof(TService), implementationAssembly, lifetime);
        }

        public void Register(Type serviceType, Lifetime lifetime)
        {
            this.Register(serviceType, Assembly.GetCallingAssembly(), lifetime);
        }

        public void Register<TService>(Lifetime lifetime) where TService : class
        {
            this.Register(typeof(TService), Assembly.GetCallingAssembly(), lifetime);
        }

        public void RegisterAsImplementedInterfaces(Type assignableToType, Assembly implementationAssembly)
        {
            this.RegisterAsImplementedInterfaces(assignableToType, implementationAssembly, this.DefaultLifetime);
        }

        public void RegisterAsImplementedInterfaces<TAssignableToType>(Assembly implementationAssembly) where TAssignableToType : class
        {
            this.RegisterAsImplementedInterfaces(typeof(TAssignableToType), implementationAssembly, this.DefaultLifetime);
        }

        public void RegisterAsImplementedInterfaces(Type assignableToType, Assembly implementationAssembly, Lifetime lifetime)
        {
            this._serviceCollection.Scan(scan => scan
                .FromAssemblies(implementationAssembly)
                .AddClasses(classes => classes.AssignableTo(assignableToType))
                .AsImplementedInterfaces()
                .WithLifetime(ServiceLifetimeByLifetime[lifetime])
            );
        }

        public void RegisterAsImplementedInterfaces<TAssignableToType>(Assembly implementationAssembly, Lifetime lifetime) where TAssignableToType : class
        {
            this.RegisterAsImplementedInterfaces(typeof(TAssignableToType), implementationAssembly, lifetime);
        }

        public void RegisterAsImplementedInterfaces(Type assignableToType, Lifetime lifetime)
        {
            this.RegisterAsImplementedInterfaces(assignableToType, Assembly.GetCallingAssembly(), lifetime);
        }

        public void RegisterAsImplementedInterfaces<TAssignableToType>(Lifetime lifetime) where TAssignableToType : class
        {
            this.RegisterAsImplementedInterfaces(typeof(TAssignableToType), Assembly.GetCallingAssembly(), lifetime);
        }
        
        public void EnsureValid(IServiceProvider serviceProvider)
        {
            // Map between Microsoft lifetimes and Simple Injector lifestyles.
            var LifestyleByServiceLifetime = new Dictionary<ServiceLifetime, Lifestyle>
            {
                { ServiceLifetime.Transient, Lifestyle.Transient },
                { ServiceLifetime.Scoped, Lifestyle.Scoped },
                { ServiceLifetime.Singleton, Lifestyle.Singleton },
            };

            // As part of verification, Simple Injector will need to resolve instances from the container. 
            // Create a service scope from which to do that.
            using (var serviceScope = serviceProvider.CreateScope())
            {
                var container = new SimpleInjector.Container();

                // Simple Injector requires that we set a default scoped lifestyle when there are scoped lifetimes.
                container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

                // Some of the .NET built-in, implementation types (e.g. HttpContextFactory) that are registered with the container have multiple 
                // public constructors which isn't supported in Simple Injector by default - so override to permit.
                container.Options.ConstructorResolutionBehavior = new GreediestConstructorBehavior(this._serviceCollection);

                var serviceDescriptorsByServiceType = new Dictionary<Type, List<ServiceDescriptor>>();
                foreach (var serviceDescriptor in this._serviceCollection)
                {
                    if (!serviceDescriptorsByServiceType.ContainsKey(serviceDescriptor.ServiceType))
                    {
                        serviceDescriptorsByServiceType[serviceDescriptor.ServiceType] = new List<ServiceDescriptor>();
                    }
                    serviceDescriptorsByServiceType[serviceDescriptor.ServiceType].Add(serviceDescriptor);
                }

                foreach (var serviceDescriptor in serviceDescriptorsByServiceType.Where(x => x.Value.Count() == 1).Select(x => x.Value.First()))
                {
                    if (serviceDescriptor.ImplementationType == null && serviceDescriptor.ImplementationFactory != null)
                    {
                        container.Register(serviceDescriptor.ServiceType, () => serviceDescriptor.ImplementationFactory(serviceScope.ServiceProvider));
                    }
                    else if (serviceDescriptor.ImplementationType == null && serviceDescriptor.ImplementationInstance != null)
                    {
                        container.RegisterInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                        container.Collection.AppendInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                    }
                    else
                    {
                        if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                        {
                            container.RegisterConditional(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime], x => true);
                        }
                        else
                        {
                            container.Register(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                            container.Collection.Append(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                        }
                    }
                }

                MethodInfo appendImplementationFactoryMethodInfo = null;

                foreach (var kvp in serviceDescriptorsByServiceType.Where(x => x.Value.Count() > 1))
                {
                    {
                        // This assumes ASP.NET Core will resolve the last registration if there are multiple
                        // and the dependency is singular TService, not an IEnumerable<TService>.
                        var serviceDescriptor = kvp.Value.Last(x => x.ImplementationType != null);
                        if (serviceDescriptor.ServiceType.IsGenericTypeDefinition)
                        {
                            container.RegisterConditional(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime], x => true);
                        }
                        else
                        {
                            container.Register(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                        }
                    }


                    foreach (var serviceDescriptor in kvp.Value)
                    {

                        if (serviceDescriptor.ImplementationType == null && serviceDescriptor.ImplementationFactory != null)
                        {
                            if (appendImplementationFactoryMethodInfo == null)
                            {
                                appendImplementationFactoryMethodInfo = typeof(ContainerCollectionRegistrator).GetMethods(BindingFlags.Public | BindingFlags.Instance)
                                    .Where(x =>
                                        x.Name == nameof(ContainerCollectionRegistrator.Append) && x.IsGenericMethod && x.ContainsGenericParameters &&
                                        x.GetParameters().Count() == 2 && x.GetParameters()[0].Name == "instanceCreator" && x.GetParameters()[1].Name == "lifestyle")
                                    .Single();
                            }
                            var genericMethodInfo = appendImplementationFactoryMethodInfo.MakeGenericMethod(serviceDescriptor.ServiceType);

                            var lambda = Expression.Lambda(
                                Expression.GetFuncType(serviceDescriptor.ServiceType),
                                Expression.Convert(
                                    Expression.Invoke(
                                        Expression.Constant(serviceDescriptor.ImplementationFactory),
                                        Expression.Constant(serviceProvider, typeof(IServiceProvider))),
                                    serviceDescriptor.ServiceType),
                                null)
                                .Compile();

                            genericMethodInfo.Invoke(container.Collection, new object[] { lambda, LifestyleByServiceLifetime[serviceDescriptor.Lifetime] });
                        }
                        else if (serviceDescriptor.ImplementationType == null && serviceDescriptor.ImplementationInstance != null)
                        {
                            container.Collection.AppendInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                        }
                        else
                        {
                            container.Collection.Append(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                        }
                    }
                }

                // Compensate for Microsoft populating the service collection with the following service -> implementation types
                // which have constructor dependencies on service types that Microsoft does not populate the container with. 
                // These service types with the missing dependency registrations will not successfully resolve unless the framework, 
                // application, or Microsoft libraries begin to register implementation types for the dependencies.

                // HostedServiceExecutor -> HostedServiceExecutor
                if (!this._serviceCollection.Any(x => x.ServiceType == typeof(IHostedService)))
                {
                    container.Collection.Register(new IHostedService[0]);
                }
                // ITagHelperComponentManager -> TagHelperComponentManager
                if (!this._serviceCollection.Any(x => x.ServiceType == typeof(ITagHelperComponent)))
                {
                    container.Collection.Register(new ITagHelperComponent[0]);
                }

                // Microsoft's registrations often have captive dependency (https://blog.ploeh.dk/2014/06/02/captive-dependency/) lifetime issues.
                // For example, a singleton might have a dependency that is registered as a transient. This finer-grained lifestyle dependency
                // is risky -- the coarser-grained instance may end up using a stale dependency.
                try
                {
                    container.Verify();
                }
                catch (DiagnosticVerificationException ex)
                {
                    // The Microsoft ASP.NET Core registrations have a large number of lifestyle mismatches
                    // e.g.  IModelMetadataProvider: DefaultModelMetadataProvider (Singleton) depends on ICompositeMetadataDetailsProvider (Transient).
                    // It's not in our power to resolve these easily, so filter down to the framework
                    // registrations whose lifetimes we do have control over.
                    var frameworkLifestyleMismatches = ex.Errors
                        .Where(x => x.DiagnosticType == DiagnosticType.LifestyleMismatch)
                        .Cast<LifestyleMismatchDiagnosticResult>()
                        .Where(x =>
                        x.ServiceType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.") ||
                        x.Relationship.ImplementationType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}."));

                    // Get warnings that are not lifestyle mismatches.
                    var nonLifestyleMismatchResults = ex.Errors.Where(x => x.Severity == DiagnosticSeverity.Warning && x.DiagnosticType != DiagnosticType.LifestyleMismatch);

                    // Rethrow the exception for results that we can ostensibly resolve.
                    if (frameworkLifestyleMismatches.Any() || nonLifestyleMismatchResults.Any())
                    {
                        throw;
                    }
                }
            }
        }
    }
}
