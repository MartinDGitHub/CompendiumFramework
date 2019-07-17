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

                // Group registrations by the service type they are registered for. For example, a service type may have multiple
                // implementation instances registered to it which are injectable as a collection. Likewise, a service type may have 
                // separate registrations for an implementation type, implementation instance, and an implementation factory. 
                var serviceTypeGroups = this._serviceCollection.GroupBy(x => x.ServiceType);
                foreach (var serviceGroup in serviceTypeGroups)
                {
                    var serviceType = serviceGroup.Key;

                    // If there are multiple implementation types registered, assume .NET Core will resolve the last one.
                    // Therefore, only register the last one for scalar dependencies.
                    var lastServiceDescriptor = serviceGroup.Last();
                    if (lastServiceDescriptor.ImplementationFactory != null)
                    {
                        container.Register(lastServiceDescriptor.ServiceType, () => lastServiceDescriptor.ImplementationFactory(serviceScope.ServiceProvider));
                    }
                    else if (lastServiceDescriptor.ImplementationInstance != null)
                    {
                        container.RegisterInstance(lastServiceDescriptor.ServiceType, lastServiceDescriptor.ImplementationInstance);
                    }
                    else if (lastServiceDescriptor.ImplementationType != null)
                    {
                        container.Register(lastServiceDescriptor.ServiceType, lastServiceDescriptor.ImplementationType, LifestyleByServiceLifetime[lastServiceDescriptor.Lifetime]);
                    }

                    // When there are multiple implementation instances or types registered, translate them
                    // into Simple Injector collection registrations.
                    if (serviceGroup.Any(x => x.ImplementationInstance != null))
                    {
                        var serviceDescriptors = serviceGroup.Where(x => x.ImplementationInstance != null);
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            container.Collection.AppendInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                        }
                    }
                    if (serviceGroup.Any(x => x.ImplementationType != null))
                    {
                        var serviceDescriptors = serviceGroup.Where(x => x.ImplementationType != null);
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            // When the implementation type is an open-generic, it cannot be registered as a collection member in Simple Injector.
                            if (!serviceDescriptor.ImplementationType.IsGenericTypeDefinition)
                            {
                                container.Collection.Append(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                            }
                        }
                    }

                    /*
                    // Register implementation factories.
                    if (serviceGroup.Any(x => x.ImplementationFactory != null))
                    {
                        // There should only ever be one implementation factory registered to a service type, as it
                        // in principle is responsible for producing implementations.
                        var serviceDescriptor = serviceGroup.Single(x => x.ImplementationFactory != null);
                        container.Register(serviceDescriptor.ServiceType, () => serviceDescriptor.ImplementationFactory(serviceScope.ServiceProvider));
                    }

                    // Register implementation instances.
                    if (serviceGroup.Any(x => x.ImplementationInstance != null))
                    {
                        var serviceDescriptors = serviceGroup.Where(x => x.ImplementationInstance != null);

                        // Ensure a single implementation instance can be resolved for a plain service type dependency.
                        // If there are multiple implementation instances registered, assume .NET Core will resolve the last one.
                        var lastServiceDescriptor = serviceDescriptors.Last();
                        container.RegisterInstance(lastServiceDescriptor.ServiceType, lastServiceDescriptor.ImplementationInstance);

                        // Ensure one or more implementation instances can be resolved as a collection.
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            container.Collection.AppendInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                        }
                    }

                    // Register implementation types.
                    if (serviceGroup.Any(x => x.ImplementationType != null))
                    {
                        var serviceDescriptors = serviceGroup.Where(x => x.ImplementationType != null);

                        // Ensure a single implementation type can be resolved for a plain service type dependency.
                        // If there are multiple implementation types registered, assume .NET Core will resolve the last one.
                        var lastServiceDescriptor = serviceDescriptors.Last();
                        container.Register(lastServiceDescriptor.ServiceType, lastServiceDescriptor.ImplementationType, LifestyleByServiceLifetime[lastServiceDescriptor.Lifetime]);

                        // Ensure one or more implementation instances can be resolved as a collection.
                        foreach (var serviceDescriptor in serviceDescriptors)
                        {
                            if (serviceDescriptor.ImplementationType.IsGenericTypeDefinition)
                            {
                               // container.RegisterConditional(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime], x => true);
                            }
                            else
                            {
                                container.Collection.Append(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                            }
                        }
                    }
                    */
                }

                /*
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

                            //genericMethodInfo.Invoke(container.Collection, new object[] { lambda, LifestyleByServiceLifetime[serviceDescriptor.Lifetime] });
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
                */

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
                    // Process diagnostic errors into categories we can inspect for troubleshooting.

                    // The Microsoft ASP.NET Core registrations have a large number of lifestyle mismatches
                    // e.g.  IModelMetadataProvider: DefaultModelMetadataProvider (Singleton) depends on ICompositeMetadataDetailsProvider (Transient).
                    // It's not in our power to resolve these easily, so filter down to the framework
                    // registrations whose lifetimes we do have control over.
                    var frameworkLifestyleMismatchDiagnosticResults = ex.Errors
                        .Where(x => x.DiagnosticType == DiagnosticType.LifestyleMismatch)
                        .Cast<LifestyleMismatchDiagnosticResult>()
                        .Where(x =>
                        x.ServiceType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.") ||
                        x.Relationship.ImplementationType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}."));

                    // The Microsoft ASP.NET Core registrations may include transient lifetimes which are disposable.
                    // Simple Injector doesn't like these; we have no control over them, so ignore them.
                    var frameworkDisposableTransientComponentDiagnosticResults =
                        ex.Errors
                        .Where(x => x.DiagnosticType == DiagnosticType.DisposableTransientComponent)
                        .Cast<DisposableTransientComponentDiagnosticResult>()
                        .Where(x =>
                        x.ServiceType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.") ||
                        x.Registration.Registration.ImplementationType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}."));

                    // Get all of the results we can ostensibly act on.
                    var diagnosticResults = ex.Errors
                        .Where(x => 
                        x.Severity == DiagnosticSeverity.Warning && 
                        x.DiagnosticType != DiagnosticType.LifestyleMismatch &&
                        x.DiagnosticType != DiagnosticType.DisposableTransientComponent)
                    .Union(frameworkLifestyleMismatchDiagnosticResults)
                    .Union(frameworkDisposableTransientComponentDiagnosticResults);

                    // Rethrow the exception for results that we can ostensibly resolve.
                    if (diagnosticResults.Any())
                    {
                        throw;
                    }
                }
            }
        }
    }
}
