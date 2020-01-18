using CF.Common.Constants;
using CF.Common.DI;
using CF.Infrastructure.DI.Verification;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
            this.RegisterAsImplementedInterfaces(assignableToType, implementationAssembly, implementationTypePredicate: null, lifetime);
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

        public void RegisterAsImplementedInterfaces(Type assignableToType, Assembly implementationAssembly, Predicate<Type> implementationTypePredicate, Lifetime lifetime)
        {
            this._serviceCollection.Scan(scan => scan
                .FromAssemblies(implementationAssembly)
                .AddClasses(classes => classes
                    // Limit to implementation types assignable to the specified service type.
                    .AssignableTo(assignableToType)
                    // If an implementation type predicate is provided, filter implementaiton types by it also.
                    .Where(type => implementationTypePredicate == null || implementationTypePredicate(type)))
                .AsImplementedInterfaces()
                .WithLifetime(ServiceLifetimeByLifetime[lifetime])
            );
        }

        public void RegisterAsImplementedInterfaces<TAssignableToType>(Assembly implementationAssembly, Predicate<Type> implementationTypePredicate, Lifetime lifetime) 
            where TAssignableToType : class
        {
            this.RegisterAsImplementedInterfaces(typeof(TAssignableToType), implementationAssembly, implementationTypePredicate, lifetime);
        }

        private class Ancestor
        {
            public Type ServiceType { get; set; }
            public Type ImplementationType { get; set; }
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
            using var serviceScope = serviceProvider.CreateScope();
            using var container = new SimpleInjector.Container();

            // Simple Injector requires that we set a default scoped lifestyle when there are scoped lifetimes.
            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();


            // Filter down to CF framework types and the types they depend upon (which may be third-party) in order to:
            //  1) Improve the performance of the verification by substantially reducing the number of types to validate;
            //  2) Avoid construction failures of Microsoft types outside of the CF reliance graph;
            //  3) Reduce registration issues involving Microsoft types outside of the CF rliance graph -- Simple Injector
            //     enforces rules for registration that Microsoft doesn't always follow;
            var serviceDescriptorsByServiceType = this._serviceCollection.GroupBy(x => x.ServiceType).ToDictionary(x => x.Key, x => x);

            var serviceDescriptorsByGenericServiceType = this._serviceCollection.Where(x => x.ServiceType.IsGenericType).GroupBy(x => x.ServiceType.GetGenericTypeDefinition()).ToDictionary(x => x.Key, x => x);

            // Resolve the framework implementation types that have been registered in the container. We don't need to worry 
            // about implementation functions as they are responsible to produce instances, not the container; likewise, 
            // implementation instances have already been constructed with their dependencies -- the container will not need to 
            // supply those dependencies. We just want the types that may need to be constructed by the container.
            var cfServiceTypes = _serviceCollection
                .Where(x => x.ImplementationType?.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.", StringComparison.Ordinal) ?? false)
                .Select(x => x.ServiceType)
                .ToHashSet();

            // Use a hash set to accumulate the unique set of service types in the framework dependency graph.
            
            var cfGraphServiceTypes = new HashSet<Type>();
            var cfGraphGenericServiceTypes = new HashSet<Type>();

            // Recurseively walk the dependency graph, starting with the registered framework service types that 
            // are registered for injection. Treat each of them as a potential root.
            foreach (var cfServiceType in cfServiceTypes)
            {
                RecurseDependencyGraph(cfGraphServiceTypes, new List<Ancestor>(), cfServiceType, 1);
            }

            // Avoid stack overflows due to circular dependencies. This is a somewhat arbitrary max level which assumes
            // a reasonable depth of dependencies. 
            const int maxLevel = 42;
            void RecurseDependencyGraph(HashSet<Type> dependencyGraphServiceTypes, List<Ancestor> serviceTypeAncestors, Type serviceType, int level)
            {
                IGrouping<Type, ServiceDescriptor> serviceDescriptors = null;
                if (serviceDescriptorsByServiceType.ContainsKey(serviceType))
                {
                    dependencyGraphServiceTypes.Add(serviceType);
                    serviceDescriptors = serviceDescriptorsByServiceType[serviceType];
                }
                else if (serviceType.IsGenericType && serviceDescriptorsByServiceType.ContainsKey(serviceType.GetGenericTypeDefinition()))
                {
                    dependencyGraphServiceTypes.Add(serviceType.GetGenericTypeDefinition());
                    serviceDescriptors = serviceDescriptorsByServiceType[serviceType.GetGenericTypeDefinition()];
                }

                if (serviceDescriptors != null)
                {
                    foreach (var serviceDescriptor in serviceDescriptors)
                    {
                        if (level < maxLevel)
                        {
                            var implementationType = serviceDescriptor.ImplementationType;
                            if (implementationType != null)
                            {
                                var parameterServiceTypes = serviceDescriptor.ImplementationType
                                    .GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                                    .SelectMany(x => x.GetParameters())
                                    .Select(x => x.ParameterType)
                                    .Distinct();

                                foreach (var parameterServiceType in parameterServiceTypes)
                                {
                                    // When there are multiple service type parameters, each represents a new branch whose descendents will have
                                    // differering ancestors. Therefore, instantiate a new branch of service types for each parameter to avoid
                                    // false positives where sibling parameter dependencies are falsely identified ancestors.
                                    var parameterServiceTypeAncestors = new List<Ancestor>(serviceTypeAncestors);
                                    parameterServiceTypeAncestors.Add(new Ancestor { ServiceType = serviceType, ImplementationType = implementationType });

                                    // Check if this parameter service type is already an ancestor, indicating a circular dependency.
                                    // If detected, throw an exception including the ancestor service types and their implementation types for troubleshooting.
                                    if (serviceTypeAncestors.Any(x => x.ServiceType == parameterServiceType))
                                    {
                                        var ancestorsDescription = string.Join(",\n", parameterServiceTypeAncestors.Select(x => $"Service Type: [{x.ServiceType.FullName}] Implementation Type: [{x.ImplementationType.FullName}]"));
                                        throw new InvalidOperationException($"Circular dependency detected for service type [{parameterServiceType.FullName}] via:\n{ancestorsDescription}");
                                    }

                                    RecurseDependencyGraph(dependencyGraphServiceTypes, parameterServiceTypeAncestors, parameterServiceType, level++);
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException($"Recursion deeper than [{maxLevel}] was detected when recursing the dependency graph.");
                        }
                    }
                }
                else
                {
                    if (serviceType.IsGenericType)
                    {
                        RecurseGenericGraph(cfGraphGenericServiceTypes, serviceType);
                    }
                }
            };

            void RecurseGenericGraph(HashSet<Type> dependencyGraphServiceTypes, Type genericType)
            {
                if (!genericType.IsGenericType)
                {
                    throw new ArgumentException($"Type [{genericType.FullName}] is not a generic type.");
                }

                if (serviceDescriptorsByGenericServiceType.ContainsKey(genericType.GetGenericTypeDefinition()))
                {
                    dependencyGraphServiceTypes.Add(genericType.GetGenericTypeDefinition());
                }

                foreach (var genericTypeArgument in genericType.GenericTypeArguments)
                {
                    if (genericTypeArgument.IsGenericType)
                    {
                        RecurseGenericGraph(dependencyGraphServiceTypes, genericTypeArgument);
                    }
                    else if (serviceDescriptorsByServiceType.ContainsKey(genericTypeArgument))
                    {
                        dependencyGraphServiceTypes.Add(genericTypeArgument);
                    }
                }
            }

            var genericServiceTypes = this._serviceCollection.Where(x => x.ServiceType.IsGenericType && cfGraphGenericServiceTypes.Contains(x.ServiceType.GetGenericTypeDefinition())).Select(x => x.ServiceType);

            foreach (var genericServiceType in genericServiceTypes)
            {
                RecurseDependencyGraph(cfGraphServiceTypes, new List<Ancestor>(), genericServiceType, 1);
            }


            // Group registrations by the service type they are registered for. For example, a service type may have multiple
            // implementation instances registered to it which are injectable as a collection. Likewise, a service type may have 
            // separate registrations for an implementation type, implementation instance, and an implementation factory. 
            var serviceTypeGroups = this._serviceCollection.Where(x => cfGraphServiceTypes.Contains(x.ServiceType) || cfGraphGenericServiceTypes.Contains(x.ServiceType))
                .GroupBy(x => x.ServiceType);

            // Some of the .NET built-in, implementation types (e.g. HttpContextFactory) that are registered with the container have multiple 
            // public constructors which isn't supported in Simple Injector by default - so override to permit.
            container.Options.ConstructorResolutionBehavior = new GreediestConstructorBehavior(this._serviceCollection.Where(x => cfGraphServiceTypes.Contains(x.ServiceType)).Select(x => x.ServiceType).ToHashSet());


            foreach (var serviceTypeGroup in serviceTypeGroups)
            {
                var serviceType = serviceTypeGroup.Key;

                // If there are multiple implementation types registered, assume .NET Core will resolve the last one.
                // Therefore, only register the last one for scalar dependencies.
                var lastServiceDescriptor = serviceTypeGroup.Last();
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
                if (serviceTypeGroup.Any(x => x.ImplementationInstance != null))
                {
                    var serviceDescriptors = serviceTypeGroup.Where(x => x.ImplementationInstance != null);
                    foreach (var serviceDescriptor in serviceDescriptors)
                    {
                        container.Collection.AppendInstance(serviceDescriptor.ServiceType, serviceDescriptor.ImplementationInstance);
                    }
                }
                if (serviceTypeGroup.Any(x => x.ImplementationType != null))
                {
                    var serviceDescriptors = serviceTypeGroup.Where(x => x.ImplementationType != null);

                    // When the implementation type is an open-generic, it cannot be registered as a collection member in Simple Injector.
                    if (!serviceTypeGroup.Key.IsGenericTypeDefinition)
                    {
                        foreach (var serviceDescriptor in serviceTypeGroup.Where(x => x.ImplementationType != null))
                        {
                            container.Collection.Append(serviceTypeGroup.Key, serviceDescriptor.ImplementationType, LifestyleByServiceLifetime[serviceDescriptor.Lifetime]);
                        }

                        //container.Collection.Append(serviceTypeGroup.Key, serviceTypeGroup.Where(x => x.ImplementationType != null).Select(x => x.ImplementationType));
                    }
                }
            }

            // Compensate for Microsoft populating the service collection with the following service -> implementation types
            // which have constructor dependencies on service types that Microsoft does not populate the container with. 
            // These service types with the missing dependency registrations will not successfully resolve unless the framework, 
            // application, or Microsoft libraries begin to register implementation types for the dependencies.

            // Nothing to compensate for right now...

            // Microsoft's registrations often have captive dependency (https://blog.ploeh.dk/2014/06/02/captive-dependency/) lifetime issues.
            // For example, a singleton might have a dependency that is registered as a transient. This finer-grained lifestyle dependency
            // is risky -- the coarser-grained instance may end up using a stale dependency.
            try
            {
                container.Verify();
            }
            catch (DiagnosticVerificationException ex)
            {
                // Separate the errors we can do something about from those we cannot, due to not having full
                // control over the ASP.NET Core framework.

                // The Microsoft ASP.NET Core registrations have a large number of lifestyle mismatches
                // e.g.  IModelMetadataProvider: DefaultModelMetadataProvider (Singleton) depends on ICompositeMetadataDetailsProvider (Transient).
                // It's not in our power to resolve these easily, so filter down to the framework
                // registrations whose lifetimes we do have control over.
                var frameworkLifestyleMismatchDiagnosticResults = ex.Errors
                    .Where(x => x.DiagnosticType == DiagnosticType.LifestyleMismatch)
                    .Cast<LifestyleMismatchDiagnosticResult>()
                    .Where(x =>
                    x.ServiceType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.", StringComparison.Ordinal) ||
                    x.Relationship.ImplementationType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.", StringComparison.Ordinal));

                // The Microsoft ASP.NET Core registrations may include transient lifetimes which are disposable.
                // Simple Injector doesn't like these - on reasonable grounds; we have no control over them, so ignore them.
                var frameworkDisposableTransientComponentDiagnosticResults =
                    ex.Errors
                    .Where(x => x.DiagnosticType == DiagnosticType.DisposableTransientComponent)
                    .Cast<DisposableTransientComponentDiagnosticResult>()
                    .Where(x =>
                    x.ServiceType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.", StringComparison.Ordinal) ||
                    x.Registration.Registration.ImplementationType.Namespace.StartsWith($"{FrameworkConstants.RootNamespace}.", StringComparison.Ordinal));

                // Treat all other diagnostic types as actionable until we discover new problems
                // with the ASP.NET Core registrations.
                var diagnosticResults = ex.Errors
                    .Where(x => 
                    // Anything more severe than information counts to fail the verification.
                    x.Severity != DiagnosticSeverity.Information && 
                    // Exclude the diagnostic types addressed above.
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
