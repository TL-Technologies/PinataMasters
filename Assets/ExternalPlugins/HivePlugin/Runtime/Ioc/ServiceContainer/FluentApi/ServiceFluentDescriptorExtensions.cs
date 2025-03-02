using System;


namespace Modules.Hive.Ioc
{
    public static class ServiceFluentDescriptorExtensions
    {
        /// <summary>
        /// Creates a fluent descriptor to configure a new service.
        /// </summary>
        /// <param name="container">A <see cref="IServiceContainer"/> to add the service to.</param>
        /// <returns>A new fluent descriptor.</returns>
        public static IServiceFluentDescriptor CreateService(this IServiceContainer container)
        {
            return new ServiceFluentDescriptor(container);
        }

        
        /// <summary>
        /// Creates a fluent descriptor to configure a new service of type <paramref name="implementationType"/>
        /// </summary>
        /// <param name="container">A <see cref="IServiceContainer"/> to add the service to.</param>
        /// <param name="implementationType">A type of the service implementation.</param>
        /// <returns>A new fluent descriptor.</returns>
        public static IServiceFluentDescriptor CreateService(this IServiceContainer container, Type implementationType)
        {
            return new ServiceFluentDescriptor(container)
                .SetImplementationType(implementationType);
        }

        
        /// <summary>
        /// Creates a fluent descriptor to configure a new service with specified implementation instance.
        /// </summary>
        /// <param name="container">A <see cref="IServiceContainer"/> to add the service to.</param>
        /// <param name="implementationInstance">An instance of the service.</param>
        /// <returns>A new fluent descriptor.</returns>
        public static IServiceFluentDescriptor CreateService(this IServiceContainer container, object implementationInstance)
        {
            return new ServiceFluentDescriptor(container)
                .SetImplementationInstance(implementationInstance);
        }

        /// <summary>
        /// Creates a fluent descriptor to configure a new service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">A type of the service implementation.</typeparam>
        /// <param name="container">A <see cref="IServiceContainer"/> to add the service to.</param>
        /// <returns>A new fluent descriptor.</returns>
        public static IServiceFluentDescriptor<T> CreateService<T>(this IServiceContainer container)
            where T : class
        {
            return new ServiceFluentDescriptor<T>(container);
        }

        
        /// <summary>
        /// Creates a fluent descriptor to configure a new service of type <typeparamref name="T"/>
        /// with specified implementation instance.
        /// </summary>
        /// <typeparam name="T">A type of the service implementation.</typeparam>
        /// <param name="container">A <see cref="IServiceContainer"/> to add the service to.</param>
        /// <param name="implementationInstance">An instance of the service.</param>
        /// <returns>A new fluent descriptor.</returns>
        public static IServiceFluentDescriptor<T> CreateService<T>(this IServiceContainer container, T implementationInstance)
            where T : class
        {
            return new ServiceFluentDescriptor<T>(container)
                .SetImplementationInstance(implementationInstance);
        }
    }
}
