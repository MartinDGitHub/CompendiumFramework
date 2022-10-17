using Microsoft.Extensions.DependencyInjection;
using SimpleInjector.Advanced;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CF.Infrastructure.DI.Verification
{
    internal class GreediestConstructorBehavior : IConstructorResolutionBehavior
    {
        private readonly HashSet<Type> _serviceTypes;

        public GreediestConstructorBehavior(HashSet<Type> serviceTypes)
        {
            this._serviceTypes = serviceTypes;
        }

        public ConstructorInfo TryGetConstructor(Type implementationType, out string errorMessage)
        {
            errorMessage = null;

            // The .NET Core container will choose the constructor with the most matching parameters
            // if there are multiple constructors. Attempt to replicate that here. In case multiple 
            // constructors qualify, the first will be used.
            var constructors = implementationType.GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            // This is broken out into multiple statements for clarity and troubleshooting in case
            // we start choosing a constructor that .NET Core wouldn't.
            if (constructors.Length > 1)
            {
                var constructor = constructors.Aggregate((maxMatchedParametersConstructor, x) =>
                {
                    if (maxMatchedParametersConstructor == null)
                    {
                        return x;
                    }

                    var maxMatchedParameters = maxMatchedParametersConstructor.GetParameters().Where(this.MatchParameter).ToArray();
                    var xMatchedParameters = x.GetParameters().Where(this.MatchParameter).ToArray();

                    // Retain the earliest match if the constructors have the same number of resolvable parameters.
                    return (maxMatchedParameters.Length >= xMatchedParameters.Length)
                    ? maxMatchedParametersConstructor
                    : x;
                });

                var implementationTypeName = implementationType.Name;

                return constructor;
            }

            return constructors.Single();
        }

        private bool MatchParameter(ParameterInfo parameterInfo)
        {
            return
                this._serviceTypes.Contains(parameterInfo.ParameterType) ||
                // Match generic types against open generics registered in the container.
                (parameterInfo.ParameterType.IsGenericType && this._serviceTypes.Contains(parameterInfo.ParameterType.GetGenericTypeDefinition()));
        }
    }
}
