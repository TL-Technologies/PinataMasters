using System;


namespace Modules.Hive.Ioc
{
    public interface IFluentImplementationType<out TFluentInterface>
    {
        /// <summary>
        /// Sets a type of the service implementation.
        /// </summary>
        /// <param name="implementationType">A type of the service implementation.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface SetImplementationType(Type implementationType);

        
        /// <summary>
        /// Sets a type of the service implementation.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface SetImplementationType<TImplementation>();
    }
}
