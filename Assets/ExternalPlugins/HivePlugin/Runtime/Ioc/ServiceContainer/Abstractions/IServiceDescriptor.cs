using System;


namespace Modules.Hive.Ioc
{
    public interface IServiceDescriptor
    {
        /// <summary>
        /// Gets a lifetime of the service in an <see cref="IServiceContainer"/>.
        /// </summary>
        ServiceLifetime Lifetime { get; }

        /// <summary>
        /// Gets a type of the service implementation object.
        /// </summary>
        Type ImplementationType { get; }

        /// <summary>
        /// Gets bindings of the service.
        /// </summary>
        ServiceBinding[] Bindings { get; }

        /// <summary>
        /// Gets a predefined instance of the service.
        /// </summary>
        object ImplementationInstance { get; }

        /// <summary>
        /// Gets a service implementation factory.
        /// </summary>
        Func<IServiceProvider, object> ImplementationFactory { get; }
    }
}
