using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CF.Infrastructure.DI
{
    public static class RegistrationTypes
    {
        // Compendium Framework assemblies are prefixed with "CF."
        private const string AssembliesToScanMask = @"CF.*.dll";

        /// <summary>
        /// Gets the types that are candidates for registration in the registry.
        /// </summary>
        public static Type[] CFTypes { get; }

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
