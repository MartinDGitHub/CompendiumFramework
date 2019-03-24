using CF.Common.Authorization.Policies;
using CF.Common.Authorization.Requirements.Handlers;
using CF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CF.Common.DI
{
    public abstract class RegistrationsBase
    {
        protected IContainer Container { get; private set; }

        protected RegistrationsBase(IContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            this.Container = container;
        }

        /// <summary>
        /// Register policies and their requirement handlers. Authorization concerns should be stable throughout 
        /// an operation so register as scoped to the operation.
        /// </summary><remarks>
        /// Note: this relies on each policy having a unique interface that ultimately derives from the root policy interfaces.
        /// </remarks>
        protected void RegisterPolicies()
        {
            this.RegisterDerivedInterfaceImplementations<IPolicy>(this.Container, Lifetime.Scoped);
            this.Container.Register(typeof(IRequirementHandler<>), this.GetType().Assembly, Lifetime.Scoped);
            this.Container.Register(typeof(IRequirementHandler<,>), this.GetType().Assembly, Lifetime.Scoped);
        }

        protected HashSet<Type> GetLeafInterfaceTypes<TInterface>(Assembly assembly, Regex namespaceFilter = null) where TInterface : class
        {
            return this.GetLeafInterfaceTypes(typeof(TInterface), assembly, namespaceFilter);
        }

        protected HashSet<Type> GetLeafInterfaceTypes(Type interfaceType, Assembly assembly, Regex namespaceFilter = null)
        {
            return this.GetLeafInterfaceTypes(interfaceType, assembly.GetTypes(), namespaceFilter);
        }

        /// <summary>
        /// Gets all leaf interface types that derive from the specified interface within the specified
        /// assembly and match the optional namespace filter. If the specified interface is a leaf interface, 
        /// it is returned.
        /// </summary>
        protected HashSet<Type> GetLeafInterfaceTypes(Type interfaceType, IEnumerable<Type> candidateTypes, Regex namespaceFilter = null)
        {
            // Get all candidate interfaces from the specified assembly.
            var candidateInterfaces = candidateTypes
                .Where(x =>
                // Reduce to interfaces.
                x.IsInterface &&
                // Reduce the interface type itself and interfaces that have the specified interface type as an ancestor.
                (x == interfaceType || x.GetInterfaces().Contains(interfaceType)) &&
                // Reduce to types that match the namespace filter (if supplied).
                (namespaceFilter == null || namespaceFilter.IsMatch(x.Namespace)));

            // Get all ancestor interfaces which other interfaces derive from.
            var ancestorInterfaces = candidateInterfaces
                // Get all of the interfaces that the interfaces derive from, this will exclude the leaf 
                // interfaces which no interfaces derive from, by definition.
                .SelectMany(x => x.GetInterfaces());

            // Leaf interfaces are the candidate interfaces that are not ancestors of other interfaces.
            var leafInterfaces = candidateInterfaces
                .Where(x => !ancestorInterfaces.Contains(x))
                .ToHashSet();

            return leafInterfaces;
        }

        protected IEnumerable<Type> GetImplementationTypes(Assembly implementationAssembly, HashSet<Type> interfaceTypes, Regex namespaceFilter = null)
        {
            return this.GetImplementationTypes(implementationAssembly.GetTypes(), interfaceTypes, namespaceFilter);
        }

        /// <summary>
        /// Gets the implementations that derive from the specified interfaces within the specified 
        /// implementation assembly and match the optional namespace filter.
        /// </summary>
        protected IEnumerable<Type> GetImplementationTypes(Type[] candidateTypes, HashSet<Type> interfaceTypes, Regex namespaceFilter = null)
        {
            return candidateTypes
                .Where(x =>
                // Reduce to classes.
                x.IsClass &&
                // Reduce to classes that derive from one of the provided interfaces.
                interfaceTypes.Overlaps(x.GetInterfaces()) &&
                // Reduce to types that match the namespace filter (if supplied).
                (namespaceFilter == null || namespaceFilter.IsMatch(x.Namespace))
                ).ToHashSet();
        }

        protected void RegisterDerivedInterfaceImplementations<TInterface>(IContainer container, Lifetime lifetime) where TInterface : class
        {
            this.RegisterDerivedInterfaceImplementations(typeof(TInterface), container, lifetime);
        }

        protected void RegisterDerivedInterfaceImplementations(Type interfaceType, IContainer container, Lifetime lifetime)
        {
            // Assume that leaf interface types will be either in the same assembly as the ancestor interface type
            // or in this assembly.
            var interfaceTypes = this.GetLeafInterfaceTypes(interfaceType, interfaceType.Assembly.GetTypes().Concat(this.GetType().Assembly.GetTypes()));
            // Only register implementation types in the assembly the derived registrations class is in.
            var implementationTypes = this.GetImplementationTypes(this.GetType().Assembly, interfaceTypes);

            this.RegisterInterfaceImplementations(container, interfaceTypes, implementationTypes, lifetime);
        }

        /// <summary>
        /// Gets implementations that match exactly one of the interfaces provided.
        /// </summary>
        protected IEnumerable<(Type InterfaceType, Type ImplementationType)> GetInterfaceImplementations(HashSet<Type> interfaceTypes, IEnumerable<Type> implementationTypes)
        {
            var interfaceImplementations = implementationTypes.Select(interfaceImplementation =>
            {
                // If multiple interfaces match, there is an ambiguity -- fail with an exception.
                var configInterfaceTypes = interfaceTypes.Intersect(interfaceImplementation.GetInterfaces());
                if (configInterfaceTypes.Count() != 1)
                {
                    throw new Exception($"[{configInterfaceTypes.Count()}] interfaces were matched to implementation type [{interfaceImplementation.FullName}] - only one may be matched. Matched interfaces: [{string.Join(", ", configInterfaceTypes.Select(x => x.Name))}].");
                }

                return (configInterfaceTypes.Single(), interfaceImplementation);
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
