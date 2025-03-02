using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// The default IServiceProvider.
    /// </summary>
    public sealed class ServiceContainer : IServiceContainer, IDisposable
    {
        private readonly ServiceProviderEngine engine;

        /// <summary>
        /// Creates a new container with specified <paramref name="options"/>
        /// </summary>
        /// <param name="options">Options to configure the container. Can be null.</param>
        public ServiceContainer(ServiceContainerOptions options = null)
        {
            if (options == null)
            {
                options = ServiceContainerOptions.Default;
            }

            switch (options.Mode)
            {
                case ServiceContainerMode.RuntimeMultiThread:
                    engine = new ServiceProviderEngine(options);
                    break;

                default:
                    throw new NotSupportedException(nameof(options.Mode));
            }
        }

        
        /// <inheritdoc />
        public void Dispose() => engine.Dispose();

        
        /// <inheritdoc />
        public int Count => engine.Count;

        
        /// <inheritdoc />
        public IServiceScope CreateScope() => engine.CreateScope();

        
        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public object GetService(Type serviceType) => engine.GetService(serviceType);

        
        /// <inheritdoc />
        public void AddService(IServiceDescriptor descriptor) => engine.AddService(descriptor);

        
        /// <inheritdoc />
        public bool RemoveService(IServiceDescriptor descriptor) => engine.RemoveService(descriptor);
    }
}
