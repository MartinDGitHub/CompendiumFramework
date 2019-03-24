using System;

namespace CF.Infrastructure.DI
{
    /// <summary>
    /// Provides service locator functionality for a container.
    /// </summary><remarks>
    /// IMPORTANT! This interface should be used very sparingly. A service locator is typically considered an anti-pattern which 
    /// should only be used when it is not possible to perform standard constructor injection.
    /// </remarks>
    public interface IServiceLocatorContainer
    {
        TService GetInstance<TService>() where TService : class;

        object GetInstance(Type serviceType);
    }
}
