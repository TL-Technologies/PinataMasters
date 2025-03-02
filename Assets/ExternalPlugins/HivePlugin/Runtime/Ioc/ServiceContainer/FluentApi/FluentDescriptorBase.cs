using System;
using System.Collections.Generic;


namespace Modules.Hive.Ioc
{
    internal abstract class FluentDescriptorBase : IServiceDescriptor
    {
        private readonly IServiceContainer container;
        protected readonly List<ServiceBinding> abstractions;

        
        public ServiceLifetime Lifetime { get; protected set; }
        public ServiceBinding[] Bindings { get => abstractions.ToArray(); }
        public Type ImplementationType { get; protected set; }
        public object ImplementationInstance { get; protected set; }
        public Func<IServiceProvider, object> ImplementationFactory { get; protected set; }

        
        public FluentDescriptorBase(IServiceContainer container)
        {
            this.container = container;
            abstractions = new List<ServiceBinding>();
            Lifetime = ServiceLifetime.Singleton;
        }

        
        protected abstract void Build();

        
        public void Apply()
        {
            Build();
            container.AddService(this);
        }
    }
}
