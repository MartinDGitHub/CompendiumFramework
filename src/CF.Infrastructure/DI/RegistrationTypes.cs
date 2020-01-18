using CF.Common.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CF.Infrastructure.DI
{
    public static class RegistrationTypes
    {
        private static readonly string AssembliesToScanMask = $"{FrameworkConstants.RootNamespace}.*.dll";

        /// <summary>
        /// Gets the types that are candidates for registration in the registry.
        /// </summary>
        public static IEnumerable<Type> CFTypes { get; }

        static RegistrationTypes()
        {
            // Only resolve the registration types once per application instance.
            CFTypes =
                new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                    .GetFiles(AssembliesToScanMask)
                    .SelectMany(file => Assembly.LoadFrom(file.FullName).GetTypes())
                    .ToArray();
        }
    }
}
