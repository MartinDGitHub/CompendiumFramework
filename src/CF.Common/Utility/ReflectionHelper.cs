using CF.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CF.Common.Utility
{
    public static class ReflectionHelper
    {
        public static HashSet<Type> GetLeafInterfaceTypes<TAssignableToType>(Assembly assembly, Regex namespaceFilter = null) where TAssignableToType : class
        {
            return ReflectionHelper.GetLeafInterfaceTypes(typeof(TAssignableToType), assembly, namespaceFilter);
        }

        public static HashSet<Type> GetLeafInterfaceTypes(Type assignableToType, Assembly assembly, Regex namespaceFilter = null)
        {
            return ReflectionHelper.GetLeafInterfaceTypes(assignableToType, assembly.GetTypes(), namespaceFilter);
        }

        /// <summary>
        /// Gets all leaf interface types that are assignable to (derive from) the specified interface within the specified
        /// assembly and match the optional namespace filter. If the specified interface is a leaf interface, it is returned.
        /// </summary>
        public static HashSet<Type> GetLeafInterfaceTypes(Type assignableToType, IEnumerable<Type> candidateTypes, Regex namespaceFilter = null)
        {
            // Get all candidate interfaces from the specified assembly.
            var candidateInterfaces = candidateTypes
                .Where(x =>
                // Reduce to interfaces.
                x.IsInterface &&
                // Reduce the interface type itself and interfaces that have the specified interface type as an ancestor.
                (x == assignableToType || x.GetInterfaces().Contains(assignableToType)) &&
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

        public static IEnumerable<Type> GetImplementationTypes(Assembly implementationAssembly, HashSet<Type> interfaceTypes, Regex namespaceFilter = null)
        {
            return ReflectionHelper.GetImplementationTypes(implementationAssembly.GetTypes(), interfaceTypes, namespaceFilter);
        }

        /// <summary>
        /// Gets the implementations that derive from the specified interfaces within the specified 
        /// implementation assembly and match the optional namespace filter.
        /// </summary>
        public static IEnumerable<Type> GetImplementationTypes(Type[] candidateTypes, HashSet<Type> interfaceTypes, Regex namespaceFilter = null)
        {
            return candidateTypes
                .Where(x =>
                // Reduce to classes.
                x.IsClass &&
                // Reduce to classes that derive from one of the provided interfaces.
                interfaceTypes.Overlaps(
                    x.GetInterfaces()
                    .Concat(
                        // In some cases the provided interfaces will include open generics (i.e. Foo<,> as opposed to Foo<Bar,Baz>).
                        // Permit matching to the open generic version of interfaces implemented by the candidate types.
                        x.GetInterfaces()
                        .Where(y => y.IsGenericType)
                        .Select(y => y.GetGenericTypeDefinition())
                        )
                    ) &&
                // Reduce to types that match the namespace filter (if supplied).
                (namespaceFilter == null || namespaceFilter.IsMatch(x.Namespace))
                ).ToHashSet();
        }
    }
}
