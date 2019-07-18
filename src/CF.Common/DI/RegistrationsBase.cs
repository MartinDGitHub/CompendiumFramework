using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements;
using CF.Common.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CF.Common.DI
{
    public abstract class RegistrationsBase
    {
        protected IContainer Container { get; private set; }

        protected RegistrationsBase(IContainer container)
        {
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <summary>
        /// Register policies and their requirement handlers.
        /// </summary>
        protected void RegisterPolicies()
        {
            // Standalone policies are registered as transient as .NET Core transient registrations depend on them.
            this.Container.RegisterAsImplementedInterfaces<IPolicy>(this.GetType().Assembly, type => typeof(IStandalonePolicy).IsAssignableFrom(type), Lifetime.Transient);
            this.Container.Register(typeof(IRequirementHandler<>), this.GetType().Assembly, Lifetime.Transient);

            // Non-standalone policies are scoped to the request. This helps ensure the policy is consistent throughout
            // the lifetime of the request.
            this.Container.RegisterAsImplementedInterfaces<IPolicy>(this.GetType().Assembly, type => !typeof(IStandalonePolicy).IsAssignableFrom(type), Lifetime.Scoped);
            this.Container.Register(typeof(IRequirementHandler<,>), this.GetType().Assembly, Lifetime.Scoped);
        }

        protected void RegisterDerivedInterfaceImplementations<TInterface>(IContainer container, Lifetime lifetime) where TInterface : class
        {
            this.RegisterDerivedInterfaceImplementations(typeof(TInterface), container, lifetime);
        }

        protected void RegisterDerivedInterfaceImplementations(Type interfaceType, IContainer container, Lifetime lifetime)
        {
            // Assume that leaf interface types will be either in the same assembly as the ancestor interface type
            // or in this assembly.
            var interfaceTypes = ReflectionHelper.GetLeafInterfaceTypes(interfaceType, interfaceType.Assembly.GetTypes().Concat(this.GetType().Assembly.GetTypes()));
            // Only register implementation types in the assembly the derived registrations class is in.
            var implementationTypes = ReflectionHelper.GetImplementationTypes(this.GetType().Assembly, interfaceTypes);

            this.RegisterInterfaceImplementations(container, interfaceTypes, implementationTypes, lifetime);
        }

        /// <summary>
        /// Gets implementations that match exactly one of the interfaces provided.
        /// </summary>
        protected IEnumerable<(Type InterfaceType, Type ImplementationType)> GetInterfaceImplementations(HashSet<Type> interfaceTypes, IEnumerable<Type> implementationTypes)
        {
            var interfaceImplementations = implementationTypes.Select(implementationType =>
            {
                // Attempt to get a most-specific, explicit interface implementation first (e.g. Foo<Bar,Baz>).
                var matchingInterfaceTypes = interfaceTypes.Intersect(implementationType.GetInterfaces());
                // If no implementation of an explicit interface type was found, fall back to an implementation of open generic interface types (e.g. Foo<,>).
                if (!matchingInterfaceTypes.Any())
                {
                    // The explicit interface type (e.g. Foo<Bar,Baz>) on the implementation must be resolved here instead of the open
                    // generic version (e.g. Foo<,>) that was specified in the provided interface types. This is because implementations
                    // must be registered with unambiguous, explicit interfaces.
                    matchingInterfaceTypes = implementationType.GetInterfaces().Where(x => x.IsGenericType && interfaceTypes.Contains(x.GetGenericTypeDefinition()));
                }
                // Exactly one interface should have been matched.
                if (matchingInterfaceTypes.Count() != 1)
                {
                    throw new Exception($"[{matchingInterfaceTypes.Count()}] interfaces were matched to implementation type [{implementationType.FullName}] - only one may be matched. Matched explicit interfaces: [{string.Join(", ", matchingInterfaceTypes.Select(x => x.Name))}]. Matched open interfaces: [{string.Join(", ", matchingInterfaceTypes.Where(x => x.IsGenericType).Select(x => x.GetGenericTypeDefinition().Name))}].");
                }

                return (matchingInterfaceTypes.Single(), implementationType);
            });

            return interfaceImplementations;
        }

        /// <summary>
        /// Registers registers implementations to interfaces.
        /// </summary>
        protected void RegisterInterfaceImplementations(IContainer container, HashSet<Type> interfaceTypes, IEnumerable<Type> implementationTypes, Lifetime lifetime)
        {
            var interfaceImplementations = this.GetInterfaceImplementations(interfaceTypes, implementationTypes);
            foreach (var interfaceImplementation in interfaceImplementations)
            {
                container.Register(interfaceImplementation.InterfaceType, interfaceImplementation.ImplementationType, lifetime);
            }
        }
    }
}
