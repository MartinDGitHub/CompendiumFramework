using CF.Common.DI;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CF.Infrastructure.DI
{
    public class ContainerRegistry<TContainerImpl> where TContainerImpl : class
    {
        private static readonly object _lock = new object();

        private const string RegistrationsNamespaceTail = "DI";

        private static IContainer _container;
        public IContainer Container => _container;

        private static TContainerImpl _containerImpl;
        public TContainerImpl ContainerImpl => _containerImpl;

        /// <summary>
        /// Registers a container implementation. This will typically be done by an application bootstrapper that will
        /// need to perform configuration that is coupled to the application type. 
        /// </summary>
        /// <typeparam name="TContainerImpl">The type of container implementation.</typeparam>
        /// <param name="containerImpl">An instance of the container implementation.</param>
        /// <returns>An implementation-agnostic container for performing registrations, etc.</returns>
        public IContainer RegisterContainer(TContainerImpl containerImpl)
        {
            // Only Simple Injector is supported right now.
            var simpleInjectorContainer = containerImpl as SimpleInjector.Container;
            if (simpleInjectorContainer == null)
            {
                throw new Exception($"A container of type [{typeof(TContainerImpl).FullName}] was provided. Only [{typeof(SimpleInjector.Container).FullName}] is supported.");
            }

            // Use double-checked locking for thread safety to enforce that only one 
            // container is ever registered per application.
            if (_container == null)
            {
                lock (_lock)
                {
                    if (_container == null)
                    {
                        _container = new Container(simpleInjectorContainer);
                        _containerImpl = containerImpl;
                    }
                }
            }

            return _container;
        }

        /// <summary>
        /// Configures the container by applying a custom configuration action and registering
        /// application types.
        /// </summary>
        /// <param name="configure">An action for custom configuration.</param>
        public void ConfigureContainer(Action<TContainerImpl> configure = null)
        {
            // Perform custom configuration, when specified.
            configure?.Invoke(_containerImpl);

            // Wire up Compendium Framework assemblies.
            var registrationsTypes = new DirectoryInfo(
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .GetFiles("CF.*.dll") // Compendium Framework assemblies are prefixed with "CF."
                .SelectMany(file => Assembly.LoadFrom(file.FullName)
                    .GetTypes()
                    .Where(type => type.IsClass && typeof(IRegistrations).IsAssignableFrom(type)))
                .ToArray();
            registrationsTypes
                .Select(type => (IRegistrations)Activator.CreateInstance(type))
                .ToList()
                .ForEach(registrations => this.RegisterServices(registrations));
        }

        private void RegisterServices(IRegistrations registrations)
        {
            if (!registrations.GetType().Namespace.EndsWith(RegistrationsNamespaceTail, StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception($"Registrations file [{registrations.GetType().Name}] was found in the namespace [{registrations.GetType().Namespace}]. It must reside in a namspace ending in \"{RegistrationsNamespaceTail}\".");
            }

            registrations.RegisterServices(_container);
        }
    }
}