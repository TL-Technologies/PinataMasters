using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Represents a fluent descriptor that can be used to configure a new service 
    /// and add it to <see cref="IServiceContainer"/>.
    /// </summary>
    public interface IServiceFluentDescriptor :
        IFluentGeneral<IServiceFluentDescriptor>,
        IFluentImplementationType<IServiceFluentDescriptor>,
        IFluentImplementationInstance<IServiceFluentDescriptor, object>,
        IFluentImplementationFactory<IServiceFluentDescriptor, object> { }


    internal class ServiceFluentDescriptor : FluentDescriptorBase, IServiceFluentDescriptor
    {
        public ServiceFluentDescriptor(IServiceContainer container) : base(container) { }

        
        /// <inheritdoc />
        public IServiceFluentDescriptor SetImplementationType(Type implementationType)
        {
            ImplementationType = implementationType;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor SetImplementationType<TImplementation>()
        {
            ImplementationType = typeof(TImplementation);
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor SetImplementationFactory(Func<IServiceProvider, object> implementationFactory)
        {
            ImplementationFactory = implementationFactory;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor SetImplementationInstance(object implementationInstance)
        {
            ImplementationInstance = implementationInstance;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor SetLifetime(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor BindTo(Type abstractionType, ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = abstractionType, Options = bindingOptions });
            return this;
        }
        

        /// <inheritdoc />
        public IServiceFluentDescriptor BindTo<TAbstraction>(ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = typeof(TAbstraction), Options = bindingOptions });
            return this;
        }
        

        protected override void Build()
        {
            if (ImplementationType == null && ImplementationInstance != null)
            {
                ImplementationType = ImplementationInstance.GetType();
            }
        }
    }
}
