using System;
using System.Collections.Generic;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Extension methods for getting services from an <see cref="IServiceProvider" />.
    /// </summary>
    public static class ServiceProviderExtension
    {
        #region Gets one service

        /// <summary>
        /// Gets a service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public static T GetService<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(T)) as T;
        }

        
        /// <summary>
        /// Gets a service of specified type from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A service object of specified type.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type <paramref name="serviceType"/>.</exception>
        public static object GetRequiredService(this IServiceProvider serviceProvider, Type serviceType)
        {
            object service = serviceProvider.GetService(serviceType);

            return service ?? throw new InvalidOperationException(ServiceProviderUtilities.MissingServiceExceptionMessage(
                    TypeNameHelper.GetTypeDisplayName(serviceType)));
        }

        
        /// <summary>
        /// Gets a service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="T"/>.</exception>
        public static T GetRequiredService<T>(this IServiceProvider serviceProvider) where T : class
        {
            T service = serviceProvider.GetService(typeof(T)) as T;

            return service ?? throw new InvalidOperationException(ServiceProviderUtilities.MissingServiceExceptionMessage(
                    TypeNameHelper.GetTypeDisplayName<T>()));
        }

        #endregion
        
        

        #region Gets a collection of services

        /// <summary>
        /// Gets a collection of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A collection of services of type <paramref name="serviceType"/></returns>
        public static object GetServices(this IServiceProvider serviceProvider, Type serviceType)
        {
            Type collectionType = typeof(IReadOnlyList<>).MakeGenericType(serviceType);
            return serviceProvider.GetService(collectionType);
        }

        
        /// <summary>
        /// Gets a collection of services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A collection of services of type <typeparamref name="T"/></returns>
        public static IReadOnlyList<T> GetServices<T>(this IServiceProvider serviceProvider) where T : class
        {
            return serviceProvider.GetService(typeof(IReadOnlyList<T>)) as IReadOnlyList<T>;
        }

        
        /// <summary>
        /// Gets a collection of services of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>A collection of services of type <paramref name="serviceType"/></returns>
        /// <exception cref="InvalidOperationException">There is no service of type <paramref name="serviceType"/>.</exception>
        public static object GetRequiredServices(this IServiceProvider serviceProvider, Type serviceType)
        {
            Type collectionType = typeof(IReadOnlyList<>).MakeGenericType(serviceType);
            object services = serviceProvider.GetService(collectionType);

            return services ?? throw new InvalidOperationException(ServiceProviderUtilities.MissingServiceExceptionMessage(
                    TypeNameHelper.GetTypeDisplayName(serviceType)));
        }

        
        /// <summary>
        /// Gets a collection of services of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to retrieve the service object from.</param>
        /// <returns>A collection of services of type <typeparamref name="T"/></returns>
        /// <exception cref="InvalidOperationException">There is no service of type <typeparamref name="T"/>.</exception>
        public static IReadOnlyList<T> GetRequiredServices<T>(this IServiceProvider serviceProvider) where T : class
        {
            var services = serviceProvider.GetService(typeof(IReadOnlyList<T>)) as IReadOnlyList<T>;

            return services ?? throw new InvalidOperationException(ServiceProviderUtilities.MissingServiceExceptionMessage(
                    TypeNameHelper.GetTypeDisplayName<T>()));
        }

        #endregion
    }
}
