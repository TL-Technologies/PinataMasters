using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Represents a fluent descriptor that can be used to configure a new service 
    /// and add it to <see cref="IServiceContainer"/>.
    /// </summary>
    public interface IServiceFluentDescriptor<in T> :
        IFluentGeneral<IServiceFluentDescriptor<T>>,
        IFluentImplementationInstance<IServiceFluentDescriptor<T>, T>,
        IFluentImplementationFactory<IServiceFluentDescriptor<T>, T> { }


    internal class ServiceFluentDescriptor<T> : FluentDescriptorBase, IServiceFluentDescriptor<T>
        where T : class
    {
        public ServiceFluentDescriptor(IServiceContainer container) : base(container) { }

        
        /// <inheritdoc />
        public IServiceFluentDescriptor<T> SetImplementationFactory(Func<IServiceProvider, T> implementationFactory)
        {
            ImplementationFactory = implementationFactory;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor<T> SetImplementationInstance(T implementationInstance)
        {
            ImplementationInstance = implementationInstance;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor<T> SetLifetime(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor<T> BindTo(Type abstractionType, ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = abstractionType, Options = bindingOptions });
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor<T> BindTo<TAbstraction>(ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = typeof(TAbstraction), Options = bindingOptions });
            return this;
        }
        

        protected override void Build()
        {
            if (ImplementationType == null)
            {
                ImplementationType = typeof(T);
            }
        }
    }
}
