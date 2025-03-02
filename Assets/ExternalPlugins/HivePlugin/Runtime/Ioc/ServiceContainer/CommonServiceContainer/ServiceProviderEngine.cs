using System;
using System.Collections.Generic;
using System.Linq;

namespace Modules.Hive.Ioc
{
    internal class ServiceProviderEngine : IServiceContainer
    {
        private CallSiteFactory callSiteFactory = new CallSiteFactory();
        private CallSiteResolver callSiteResolver = new CallSiteResolver();
        private Dictionary<Type, ServiceEntity> implementations = new Dictionary<Type, ServiceEntity>();
        private Dictionary<Type, ServiceEntityCollection> bindings = new Dictionary<Type, ServiceEntityCollection>();
        private bool isDisposed;

        
        internal ServiceContainerOptions Options { get; }
        internal ServiceProviderEngineScope RootScope { get; }

        
        public ServiceProviderEngine(ServiceContainerOptions options)
        {
            Options = options;
            RootScope = new ServiceProviderEngineScope(this);

            AddServiceEntity<IServiceProvider>(new ConstantCallSite(this));
            AddServiceEntity<IServiceContainer>(new ConstantCallSite(this));
            AddServiceEntity<IServiceScopeFactory>(new ConstantCallSite(this));
        }
        

        public void Dispose()
        {
            isDisposed = true;
            RootScope.Dispose();
        }

        
        #region IServiceContainer implementation

        public int Count => implementations.Count;

        
        public object GetService(Type serviceType) => GetService(serviceType, RootScope);

        
        internal object GetService(Type serviceType, ServiceProviderEngineScope scope)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IServiceContainer));
            }

            return callSiteResolver.Resolve(serviceType, scope);
        }

        
        public IServiceScope CreateScope()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IServiceContainer));
            }

            return new ServiceProviderEngineScope(this);
        }

        
        public void AddService(IServiceDescriptor descriptor)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IServiceContainer));
            }

            ServiceEntity entity = new ServiceEntity(descriptor);
            entity.Validate();

            lock (implementations)
            {
                Type implementationType = descriptor.ImplementationType;
                if (implementations.ContainsKey(implementationType))
                {
                    throw new InvalidOperationException(ServiceProviderUtilities.CannotAddServiceExceptionMessage(
                                                            TypeNameHelper.GetTypeDisplayName(implementationType)));
                }

                AddServiceEntity(entity);
            }
        }

        public bool RemoveService(IServiceDescriptor descriptor)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IServiceContainer));
            }

            lock (implementations)
            {
                Type implementationType = descriptor.ImplementationType;
                if (!implementations.TryGetValue(implementationType, out ServiceEntity entity))
                {
                    return false;
                }

                if (!entity.EqualsTo(descriptor))
                {
                    return false;
                }

                // Remove bindings
                for (int i = 0; i < entity.Bindings.Length; i++)
                {
                    ServiceBinding binding = entity.Bindings[i];

                    if (bindings.TryGetValue(binding.Type, out ServiceEntityCollection sec))
                    {
                        sec = sec.Remove(entity);
                        if (sec.Empty)
                        {
                            bindings.Remove(binding.Type);
                        }
                        else
                        {
                            bindings[binding.Type] = sec;
                        }
                    }
                }

                // Remove service implementation
                implementations.Remove(implementationType);

                return true;
            }
        }

        #endregion
        
        

        #region Entities internal management

        internal ServiceEntityCollection GetServiceEntityCollection(Type abstractionType)
        {
            lock (implementations)
            {
                // Try to get ServiceEntityCollection from collection of bindings
                if (bindings.TryGetValue(abstractionType, out ServiceEntityCollection sec))
                {
                    return sec;
                }

                // Create a new ServiceEntityCollection
                sec = new ServiceEntityCollection();
                var enumerator = implementations
                    .Where(p => p.Value.ContainsServiceAbstraction(abstractionType))
                    .Select(p => p.Value);

                foreach (var entity in enumerator)
                {
                    sec = sec.CheckAndAdd(entity, abstractionType);
                }

                // Add the ServiceEntityCollection to collection of bindings
                bindings[abstractionType] = sec;
                return sec;
            }
        }

        
        internal ICallSite GetServiceCallSite(ServiceEntity entity, ServiceProviderEngineScope scope, CallSiteChain chain)
        {
            // OPTIMIZATION: Potentially, this is a very frequent operation.
            // Using locking inside can cause performance issues.
            // For optimization, use a more efficient approach than lock().

            ICallSite callSite = entity.CallSite;
            if (callSite == null)
            {
                lock (entity)
                {
                    if (entity.CallSite == null)
                    {
                        entity.CallSite = callSiteFactory.CreateCallSite(entity, scope, chain);
                    }

                    callSite = entity.CallSite;
                }
            }

            return callSite;
        }

        
        private void AddServiceEntity<TService>(ICallSite callSite)
        {
            ServiceBinding binding = new ServiceBinding
            {
                Type = typeof(TService),
                Options = ServiceBindingOptions.Default
            };

            ServiceEntity entity = new ServiceEntity(
                ServiceLifetime.Singleton,
                new[] { binding },
                typeof(TService),
                null,
                null);
            entity.CallSite = callSite;

            AddServiceEntity(entity);
        }

        
        private void AddServiceEntity(ServiceEntity entity)
        {
            int count = entity.Bindings.Length;
            ServiceEntityCollection[] entityCollections = new ServiceEntityCollection[count];

            // Checks constraints before adding service to the container.
            // It's highly important to do it before changing something in the container.
            for (int i = 0; i < count; i++)
            {
                ServiceBinding binding = entity.Bindings[i];

                // Adding a new entity to sec can throw an exception 
                bindings.TryGetValue(binding.Type, out var sec);
                entityCollections[i] = sec.CheckAndAdd(entity, binding);
            }

            // Add all abstractions of the service to collection
            for (int i = 0; i < count; i++)
            {
                ServiceBinding binding = entity.Bindings[i];
                bindings[binding.Type] = entityCollections[i];
            }

            // add service implementation
            implementations.Add(entity.ImplementationType, entity);
        }

        #endregion
    }
}
