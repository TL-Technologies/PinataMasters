using System;
using System.Diagnostics;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Internal immutable copy of ServiceDescriptor with additional data and logic
    /// </summary>
    [DebuggerDisplay("Lifetime = {Lifetime}, ImplementationType = {ImplementationType}")]
    internal sealed class ServiceEntity
    {
        public ServiceLifetime Lifetime { get; }
        public ServiceBinding[] Bindings { get; }
        public Type ImplementationType { get; }
        public object ImplementationInstance { get; }
        public Func<IServiceProvider, object> ImplementationFactory { get; }

        internal ICallSite CallSite { get; set; }

        
        public ServiceEntity(
            ServiceLifetime lifetime,
            ServiceBinding[] bindings,
            Type implementationType,
            object implementationInstance,
            Func<IServiceProvider, object> implementationFactory)
        {
            Lifetime = lifetime;
            Bindings = bindings;
            ImplementationType = implementationType;
            ImplementationInstance = implementationInstance;
            ImplementationFactory = implementationFactory;
        }
        

        public ServiceEntity(IServiceDescriptor descriptor)
        {
            Lifetime = descriptor.Lifetime;
            Bindings = descriptor.Bindings;
            ImplementationType = descriptor.ImplementationType;
            ImplementationInstance = descriptor.ImplementationInstance;
            ImplementationFactory = descriptor.ImplementationFactory;
        }
        

        public bool ContainsServiceAbstraction(Type abstractionType)
        {
            for (int i = Bindings.Length - 1; i >= 0; i--)
            {
                if (Bindings[i].Type == abstractionType)
                {
                    return true;
                }
            }

            return false;
        }

        
        public bool TryGetServiceAbstraction(Type abstractionType, out ServiceBinding abstraction)
        {
            for (int i = Bindings.Length - 1; i >= 0; i--)
            {
                if (Bindings[i].Type == abstractionType)
                {
                    abstraction = Bindings[i];
                    return true;
                }
            }

            abstraction = default;
            return false;
        }

        
        public ServiceBinding GetServiceAbstraction(Type abstractionType)
        {
            for (int i = Bindings.Length - 1; i >= 0; i--)
            {
                if (Bindings[i].Type == abstractionType)
                {
                    return Bindings[i];
                }
            }

            throw new InvalidCastException(ServiceProviderUtilities.InvalidServiceAbstractionTypeExceptionMessage(
                                               TypeNameHelper.GetTypeDisplayName(ImplementationType),
                                               TypeNameHelper.GetTypeDisplayName(abstractionType)));
        }

        
        public bool EqualsTo(IServiceDescriptor descriptor)
        {
            if (Lifetime != descriptor.Lifetime)
            {
                return false;
            }

            if (ImplementationType != descriptor.ImplementationType)
            {
                return false;
            }

            if (ImplementationInstance != descriptor.ImplementationInstance)
            {
                return false;
            }

            if (ImplementationFactory != descriptor.ImplementationFactory)
            {
                return false;
            }

            if (Bindings.Length != descriptor.Bindings.Length)
            {
                return false;
            }

            for (int i = 0; i < Bindings.Length; i++)
            {
                var a = Bindings[i];
                var b = descriptor.Bindings[i];

                if (a.Type != b.Type || a.Options != b.Options)
                {
                    return false;
                }
            }

            return true;
        }
        

        [Conditional("DEBUG")]
        public void Validate()
        {
            if (ImplementationType == null)
            {
                throw new ArgumentNullException("ImplementationType");
            }

            if (Bindings == null || Bindings.Length == 0)
            {
                throw new ArgumentException("Bindings is null or empty.");
            }

            // Check implementation instance
            if (ImplementationInstance != null)
            {
                Type implementationInstanceType = ImplementationInstance.GetType();
                if (!ImplementationType.IsAssignableFrom(implementationInstanceType))
                {
                    throw new InvalidCastException(ServiceProviderUtilities.InvalidServiceImplementationTypeExceptionMessage(
                                                       TypeNameHelper.GetTypeDisplayName(ImplementationType),
                                                       TypeNameHelper.GetTypeDisplayName(implementationInstanceType)));
                }

                if (ImplementationFactory != null)
                {
                    throw new InvalidOperationException("Ambiguous between ImplementationInstance and ImplementationFactory.");
                }

                if (Lifetime != ServiceLifetime.Singleton)
                {
                    throw new InvalidOperationException("A service implementation defined by ImplementationInstance can only be a singleton.");
                }
            }
            else if (ImplementationFactory != null) // Check implementation factory
            {
                // Unfortunately, there is no way to explicitly check a type of object that implementation factory returns.
                // Also we cannot instantiate one right now because the container initialization maybe not completed yet.
            }
            else if (!ImplementationType.IsClass || // check whether the service can be activated
                ImplementationType.IsAbstract ||
                ImplementationType.IsGenericTypeDefinition)
            {
                throw new ArgumentException(ServiceProviderUtilities.TypeCannotBeActivatedExceptionMessage(ImplementationType));
            }

            // Check bindings
            foreach (var binding in Bindings)
            {
                if (!binding.Type.IsAssignableFrom(ImplementationType))
                {
                    throw new InvalidCastException(ServiceProviderUtilities.InvalidServiceAbstractionTypeExceptionMessage(
                                                       TypeNameHelper.GetTypeDisplayName(ImplementationType),
                                                       TypeNameHelper.GetTypeDisplayName(binding.Type)));
                }
            }
        }
    }
}
