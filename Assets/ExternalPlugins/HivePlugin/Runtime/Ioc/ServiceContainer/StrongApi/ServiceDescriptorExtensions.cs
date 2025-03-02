using System;


namespace Modules.Hive.Ioc
{
    public static class ServiceDescriptorExtensions
    {
        #region Transient

        /// <summary>
        /// Adds a transient service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddTransientService<TImplementation, TAbstraction>(
            this IServiceContainer container,
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Transient,
                new[] { abstraction }));
        }

        
        /// <summary>
        /// Adds a transient service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// Also provides an instance implementation factory.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="implementationFactory">A factory to create an instance of service.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddTransientService<TImplementation, TAbstraction>(
            this IServiceContainer container,
            Func<IServiceProvider, TImplementation> implementationFactory,
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Transient,
                new[] { abstraction },
                implementationFactory));
        }

        #endregion

        
        
        #region Scoped

        /// <summary>
        /// Adds a scoped service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddScopedService<TImplementation, TAbstraction>(
            this IServiceContainer container,
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Scoped,
                new[] { abstraction }));
        }
        

        /// <summary>
        /// Adds a scoped service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// Also provides an instance implementation factory.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="implementationFactory">A factory to create an instance of service.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddScopedService<TImplementation, TAbstraction>(
            this IServiceContainer container,
            Func<IServiceProvider, TImplementation> implementationFactory,
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Scoped,
                new[] { abstraction },
                implementationFactory));
        }

        #endregion
        
        

        #region Singleton

        /// <summary>
        /// Adds a singleton service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddSingletonService<TImplementation, TAbstraction>(
            this IServiceContainer container, 
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Singleton,
                new[] { abstraction }));
        }
        

        /// <summary>
        /// Adds a singleton service of type <typeparamref name="TImplementation"/> and specified
        /// <paramref name="implementationInstance"/>. Binds it to abstraction of type 
        /// <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="implementationInstance">An instance of service implementation.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddSingletonService<TImplementation, TAbstraction>(
            this IServiceContainer container, 
            TImplementation implementationInstance, 
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                implementationInstance,
                new[] { abstraction }));
        }
        

        /// <summary>
        /// Adds a singleton service of type <typeparamref name="TImplementation"/> and binds it to
        /// abstraction of type <typeparamref name="TAbstraction"/> with options <paramref name="bindingOptions"/>.
        /// Also provides an instance implementation factory.
        /// </summary>
        /// <typeparam name="TImplementation">A type of the service implementation.</typeparam>
        /// <typeparam name="TAbstraction">An abstraction type to bind to.</typeparam>
        /// <param name="container">A service container to add to.</param>
        /// <param name="implementationFactory">A factory to create an instance of service.</param>
        /// <param name="bindingOptions">A binding options.</param>
        public static void AddSingletonService<TImplementation, TAbstraction>(
            this IServiceContainer container,
            Func<IServiceProvider, TImplementation> implementationFactory, 
            ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
            where TImplementation : class, TAbstraction
        {
            var abstraction = new ServiceBinding
            {
                Type = typeof(TAbstraction),
                Options = bindingOptions
            };

            container.AddService(new ServiceDescriptor<TImplementation>(
                ServiceLifetime.Singleton,
                new[] { abstraction },
                implementationFactory));
        }

        #endregion
    }
}
