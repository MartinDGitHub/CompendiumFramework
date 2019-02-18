using SimpleInjector;
using System;

namespace CF.WebBootstrap.DI
{
    internal static class ContainerProvider
    {
        public static Lazy<Container> Container { get; } = new Lazy<Container>();
    }
}
