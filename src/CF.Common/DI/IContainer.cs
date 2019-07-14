using System;
using System.Reflection;

namespace CF.Common.DI
{
    public interface IContainer : IDisposable
    {
        /// <summary>
        /// Gets the default lifetime for the container which will be applied if no lifetime is specified (unless otherwise indicated).
        /// </summary>
        Lifetime DefaultLifetime { get; }

        /// <summary>
        /// Registers an implementation instance as a singleton.
        /// </summary>
        void RegisterInstance<TService>(TService implementationInstance) where TService : class;

        /// <summary>
        /// Registers the implementation type to instantiate for a service type dependency, using the default lifetime.
        /// </summary>
        void Register(Type serviceType, Type implementationType);

        /// <summary>
        /// Registers the implementation type to instantiate for a service type dependency, using the default lifetime.
        /// </summary>
        void Register<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Registers the implementation type to instantiate for a service type dependency, using the specified lifetime.
        /// </summary>
        void Register(Type serviceType, Type implementationType, Lifetime lifetime);

        /// <summary>
        /// Registers the implementation type to instantiate for a service type dependency, using the specified lifetime.
        /// </summary>
        void Register<TService, TImplementation>(Lifetime lifetime)
            where TService : class
            where TImplementation : class, TService;

        /// <summary>
        /// Registers all implementation types in the implementation assembly which implement the service type, for that service type dependency, using the default lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register(Type serviceType, Assembly implementationAssembly);

        /// <summary>
        /// Registers all implementation types in the implementation assembly which implement the service type, for that service type dependency, using the default lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register<TService>(Assembly implementationAssembly) where TService : class;

        /// <summary>
        /// Registers all implementation types in the implementation assembly which implement the service type, for that service type dependency, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register(Type serviceType, Assembly implementationAssembly, Lifetime lifetime);

        /// <summary>
        /// Registers all implementation types in the implementation assembly which implement the service type, for that service type dependency, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register<TService>(Assembly implementationAssembly, Lifetime lifetime)
            where TService : class;

        /// <summary>
        /// Registers all implementation types in the calling assembly which implement the service type, for that service type dependency, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register(Type serviceType, Lifetime lifetime);

        /// <summary>
        /// Registers all implementation types in the calling assembly which implement the service type, for that service type dependency, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the service type.
        /// </remarks>
        void Register<TService>(Lifetime lifetime) where TService : class;

        /// <summary>
        /// Registers all implementation types in the implementation assembly which are assignable to the specified type, for all implemented interface type dependencies, using the default lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces(Type assignableToType, Assembly implementationAssembly);

        /// <summary>
        /// Registers all implementation types in the implementation assembly which are assignable to the specified type, for all implemented interface type dependencies, using the default lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces<TAssignableToType>(Assembly implementationAssembly) where TAssignableToType : class;

        /// <summary>
        /// Registers all implementation types in the implementation assembly which are assignable to the specified type, for all implemented interface type dependencies, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces(Type assignableToType, Assembly implementationAssembly, Lifetime lifetime);

        /// <summary>
        /// Registers all implementation types in the implementation assembly which are assignable to the specified type, for all implemented interface type dependencies, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces<TAssignableToType>(Assembly implementationAssembly, Lifetime lifetime) where TAssignableToType : class;

        /// <summary>
        /// Registers all implementation types in the calling assembly which are assignable to the specified type, for all implemented interface type dependencies, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces(Type assignableToType, Lifetime lifetime);

        /// <summary>
        /// Registers all implementation types in the calling assembly which are assignable to the specified type, for all implemented interface type dependencies, using the specified lifetime.
        /// </summary><remarks>
        /// Open generic types are supported as the assignable to type.
        /// </remarks>
        void RegisterAsImplementedInterfaces<TAssignableToType>(Lifetime lifetime) where TAssignableToType : class;

        /// <summary>
        /// Performs checking to ensure the registrations are valid, throwing an exception if errors are found.
        /// </summary><remarks>
        /// <para>
        /// Validation may check for circularities, inconsistent scoping (i.e. a singleton with a dependency on a transient),
        /// and other issues.
        /// </para><para>
        /// When the set of registrations is constant across environments, it should be sufficient to invoke this method
        /// only in the development environment to avoid incurring the cost of the validation. In other cases, where 
        /// runtime registrations may cause the set to vary across environments, consider always invoking this method after
        /// registrations are complete.
        /// </para>
        /// </remarks>
        void EnsureValid(IServiceProvider serviceProvider);
    }
}
