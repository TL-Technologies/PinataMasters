using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Describes a service container. 
    /// </summary>
    public interface IServiceContainer : IServiceProvider, IServiceScopeFactory, IDisposable
    {
        /// <summary>
        /// Gets the number of services (implementations) contained in the service container.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Adds the specified service to the service container.
        /// </summary>
        /// <param name="serviceDescriptor">The descriptor of service to add.</param>
        void AddService(IServiceDescriptor serviceDescriptor);

        /// <summary>
        /// Removes the specified service to the service container.
        /// </summary>
        /// <param name="serviceDescriptor">The descriptor of service to remove.</param>
        /// <returns>true if service is successfully removed; otherwise, false.
        /// This method also returns false if service was not found in the service container.</returns>
        bool RemoveService(IServiceDescriptor serviceDescriptor);
    }
}
