using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Describes a service with its implementation type, bindings, lifetime, and options.
    /// Also provides a factory used for creating service instances.
    /// </summary>
    public sealed class ServiceDescriptor : IServiceDescriptor
    {
        /// <inheritdoc />
        public ServiceLifetime Lifetime { get; }

        
        /// <inheritdoc />
        public ServiceBinding[] Bindings { get; }

        
        /// <inheritdoc />
        public Type ImplementationType { get; }

        
        /// <inheritdoc />
        public object ImplementationInstance { get; }

        
        /// <inheritdoc />
        public Func<IServiceProvider, object> ImplementationFactory { get; }

        
        /// <summary>
        /// Creates a service descriptor that describes a service of specified type, lifetime and bindings.
        /// </summary>
        /// <param name="lifetime">Lifetime of the service.</param>
        /// <param name="implementationType">A type of service implementation.</param>
        /// <param name="bindings">An array of bindings of the service.</param>
        public ServiceDescriptor(
            ServiceLifetime lifetime,
            Type implementationType,
            ServiceBinding[] bindings)
        {
            Lifetime = lifetime;
            Bindings = bindings;
            ImplementationType = implementationType;
        }
        

        /// <summary>
        /// Creates a service descriptor that describes a service of specified type, lifetime, bindings and
        /// implementation factory.
        /// <para>WARNING! Ensure that a factory creates objects of valid type.</para>
        /// </summary>
        /// <param name="lifetime">Lifetime of the service.</param>
        /// <param name="implementationType">A type of service implementation.</param>
        /// <param name="bindings">An array of bindings of the service.</param>
        /// <param name="implementationFactory">A service implementation factory.</param>
        public ServiceDescriptor(
            ServiceLifetime lifetime, 
            Type implementationType,
            ServiceBinding[] bindings, 
            Func<IServiceProvider, object> implementationFactory)
        {
            Lifetime = lifetime;
            Bindings = bindings;
            ImplementationType = implementationType;
            ImplementationFactory = implementationFactory;
        }
        

        /// <summary>
        /// Creates a service descriptor that describes a singleton service with specified instance and bindings.
        /// <para>WARNING! Ensure that an implementation instance inherits all its abstractions.</para>
        /// </summary>
        /// <param name="implementationInstance"></param>
        /// <param name="bindings"></param>
        public ServiceDescriptor(
            object implementationInstance,
            ServiceBinding[] bindings)
        {
            Lifetime = ServiceLifetime.Singleton;
            Bindings = bindings;
            ImplementationType = implementationInstance.GetType();
            ImplementationInstance = implementationInstance;
        }
    }

    
    /// <summary>
    /// Describes a service with its implementation type <typeparamref name="TImplementation"/>,
    /// bindings, lifetime, and options.
    /// Also provides a factory used for creating service instances.
    /// </summary>
    public sealed class ServiceDescriptor<TImplementation> : IServiceDescriptor
        where TImplementation : class
    {
        /// <inheritdoc />
        public ServiceLifetime Lifetime { get; }

        
        /// <inheritdoc />
        public ServiceBinding[] Bindings { get; }

        
        /// <inheritdoc />
        public Type ImplementationType => typeof(TImplementation);

        
        /// <summary>
        /// Gets a predefined instance of the service.
        /// </summary>
        public TImplementation ImplementationInstance { get; }
        object IServiceDescriptor.ImplementationInstance => ImplementationInstance;

        
        /// <summary>
        /// Gets a service implementation factory.
        /// </summary>
        public Func<IServiceProvider, TImplementation> ImplementationFactory { get; }
        Func<IServiceProvider, object> IServiceDescriptor.ImplementationFactory => ImplementationFactory;

        
        /// <summary>
        /// Creates a service descriptor that describes a service of type <typeparamref name="TImplementation"/>, lifetime and bindings.
        /// </summary>
        /// <param name="lifetime">Lifetime of the service.</param>
        /// <param name="bindings">An array of bindings of the service.</param>
        public ServiceDescriptor(
            ServiceLifetime lifetime,
            ServiceBinding[] bindings)
        {
            Lifetime = lifetime;
            Bindings = bindings;
        }

        
        /// <summary>
        /// Creates a service descriptor that describes a service of type <typeparamref name="TImplementation"/>, lifetime, bindings and
        /// implementation factory.
        /// <para>WARNING! Ensure that a factory creates objects of valid type.</para>
        /// </summary>
        /// <param name="lifetime">Lifetime of the service.</param>
        /// <param name="bindings">An array of bindings of the service.</param>
        /// <param name="implementationFactory">A service implementation factory.</param>
        public ServiceDescriptor(
            ServiceLifetime lifetime,
            ServiceBinding[] bindings, 
            Func<IServiceProvider, TImplementation> implementationFactory)
        {
            Lifetime = lifetime;
            Bindings = bindings;
            ImplementationFactory = implementationFactory;
        }

        
        /// <summary>
        /// Creates a service descriptor that describes a singleton service with specified instance and bindings.
        /// <para>WARNING! Ensure that an implementation instance inherits all its abstractions.</para>
        /// </summary>
        /// <param name="implementationInstance"></param>
        /// <param name="bindings"></param>
        public ServiceDescriptor(
            TImplementation implementationInstance,
            ServiceBinding[] bindings)
        {
            Lifetime = ServiceLifetime.Singleton;
            Bindings = bindings;
            ImplementationInstance = implementationInstance;
        }
    }
}
