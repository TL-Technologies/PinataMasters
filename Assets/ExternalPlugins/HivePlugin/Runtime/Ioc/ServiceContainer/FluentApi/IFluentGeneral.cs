using System;

namespace Modules.Hive.Ioc
{
    // TODO: IFluentGeneral.Apply() should returns an IServiceDescriptor that can be user to remove service

    public interface IFluentGeneral<out TFluentInterface>
    {
        /// <summary>
        /// Sets a lifetime of the service in an <see cref="IServiceContainer"/>.
        /// </summary>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface SetLifetime(ServiceLifetime lifetime);

        
        /// <summary>
        /// Binds the service to its abstraction type.
        /// </summary>
        /// <param name="abstractionType">An abstraction type to bind to.</param>
        /// <param name="bindingOptions">A binding options.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface BindTo(Type abstractionType, ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default);

        
        /// <summary>
        /// Binds the service to its abstraction type.
        /// </summary>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="bindingOptions">A binding options.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface BindTo<TAbstraction>(ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default);

        
        /// <summary>
        /// Applies configuration, makes a new <see cref="IServiceDescriptor"/> and adds it
        /// to <see cref="IServiceContainer"/>.
        /// </summary>
        void Apply();
    }


    public static class FluentGeneralExtensions
    {
        /// <summary>
        /// Applies configuration, makes a new <see cref="IServiceDescriptor"/> and adds it
        /// to <see cref="IServiceContainer"/> as a transient service.
        /// </summary>
        /// <typeparam name="TFluentInterface">An interface of fluent descriptor.</typeparam>
        /// <param name="descriptor">A fluent descriptor to work with.</param>
        public static void AsTransient<TFluentInterface>(this TFluentInterface descriptor)
            where TFluentInterface : IFluentGeneral<TFluentInterface>
        {
            descriptor
                .SetLifetime(ServiceLifetime.Transient)
                .Apply();
        }

        
        /// <summary>
        /// Applies configuration, makes a new <see cref="IServiceDescriptor"/> and adds it
        /// to <see cref="IServiceContainer"/> as a scoped service.
        /// </summary>
        /// <typeparam name="TFluentInterface">An interface of fluent descriptor.</typeparam>
        /// <param name="descriptor">A fluent descriptor to work with.</param>
        public static void AsScoped<TFluentInterface>(this TFluentInterface descriptor)
            where TFluentInterface : IFluentGeneral<TFluentInterface>
        {
            descriptor
                .SetLifetime(ServiceLifetime.Scoped)
                .Apply();
        }

        
        /// <summary>
        /// Applies configuration, makes a new <see cref="IServiceDescriptor"/> and adds it
        /// to <see cref="IServiceContainer"/> as a singleton service.
        /// </summary>
        /// <typeparam name="TFluentInterface">An interface of fluent descriptor.</typeparam>
        /// <param name="descriptor">A fluent descriptor to work with.</param>
        public static void AsSingleton<TFluentInterface>(this TFluentInterface descriptor)
            where TFluentInterface : IFluentGeneral<TFluentInterface>
        {
            descriptor
                .SetLifetime(ServiceLifetime.Singleton)
                .Apply();
        }
    }
}
