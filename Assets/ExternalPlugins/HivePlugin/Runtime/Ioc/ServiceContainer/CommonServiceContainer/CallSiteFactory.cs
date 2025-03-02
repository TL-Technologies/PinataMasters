using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Modules.Hive.Ioc
{
    internal class CallSiteFactory
    {
        public bool CheckCircularDependency(Type clientServiceType, Type serviceType, ServiceProviderEngineScope scope, CallSiteChain chain, bool throwIfServiceNotFound)
        {
            bool result =
                CheckServiceExact(serviceType, scope, chain) ||
                CheckServiceCollection(serviceType, scope, chain);

            if (throwIfServiceNotFound && !result)
            {
                throw new InvalidOperationException(ServiceProviderUtilities.CannotResolveServiceExceptionMessage(
                                                        serviceType,
                                                        clientServiceType));
            }

            return result;
        }
        

        private bool CheckServiceExact(Type serviceType, ServiceProviderEngineScope scope, CallSiteChain chain)
        {
            ServiceEntityCollection item = scope.Engine.GetServiceEntityCollection(serviceType);
            if (item.Empty)
            {
                return false;
            }

            scope.Engine.GetServiceCallSite(item.Last, scope, chain);
            return true;
        }

        
        private bool CheckServiceCollection(Type serviceType, ServiceProviderEngineScope scope, CallSiteChain chain)
        {
            if (!ServiceProviderUtilities.IsServiceCollectionType(serviceType))
            {
                return false;
            }

            serviceType = serviceType.GenericTypeArguments.Single();

            ServiceEntityCollection item = scope.Engine.GetServiceEntityCollection(serviceType);
            if (item.Empty)
            {
                return false;
            }

            int count = item.Count;
            for (int i = 0; i < count; count++)
            {
                scope.Engine.GetServiceCallSite(item[i], scope, chain);
            }

            return true;
        }

        
        public ICallSite CreateCallSite(ServiceEntity entity, ServiceProviderEngineScope scope, CallSiteChain chain)
        {
            ICallSite callSite;

            if (entity.ImplementationInstance != null)
            {
                callSite = new ConstantCallSite(entity.ImplementationInstance);
            }
            else if (entity.ImplementationFactory != null)
            {
                callSite = new FactoryCallSite(entity.ImplementationFactory);
            }
            else if (entity.ImplementationType != null)
            {
                callSite = CreateConstructorCallSite(entity.ImplementationType, scope, chain);
            }
            else
            {
                throw new InvalidOperationException("Invalid service descriptor.");
            }

            return ApplyLifetime(callSite, entity.ImplementationType, entity.Lifetime);
        }

        
        public ICallSite CreateConstructorCallSite(Type implementationType, ServiceProviderEngineScope scope, CallSiteChain chain)
        {
            var constructors = implementationType.GetTypeInfo()
                .DeclaredConstructors
                .Where(constructor => constructor.IsPublic)
                .ToList();

            // Check whether the implementationType has at least one public constructor
            if (constructors.Count == 0)
            {
                throw new InvalidOperationException(ServiceProviderUtilities.NoConstructorMatchExceptionMessage(
                                                        TypeNameHelper.GetTypeDisplayName(implementationType)));
            }

            // Check whether the implementationType has single constructor without parameters
            if (constructors.Count == 1)
            {
                var constructor = constructors[0];
                var parameters = constructor.GetParameters();
                if (parameters.Length == 0)
                {
                    return new ActivatorCallSite(implementationType);
                }
            }
            else
            {
                // Sort collection by amount of parameters to select most relevant constructor
                constructors.Sort((a, b) => b.GetParameters().Length.CompareTo(a.GetParameters().Length));
            }

            // Select most relevant constructor
            if (chain == null)
            {
                chain = new CallSiteChain();
            }

            chain.Add(implementationType);

            // OPTIMIZATION: Maybe this approach is expensive. It can be optimized by using [Inject] attribute to decrease size of constructors collection.
            bool requireServiceExistence = !scope.Engine.Options.AllowToInjectDefaults;
            ConstructorInfo bestConstructor = null;
            Type[] serviceTypes = null;
            HashSet<Type> bestConstructorParameterTypes = null;
            for (var i = 0; i < constructors.Count; i++)
            {
                var parameters = constructors[i].GetParameters();
                var currentServiceTypes = GetServiceTypes(
                    implementationType, 
                    parameters, 
                    scope, 
                    chain,
                    requireServiceExistence);

                if (currentServiceTypes != null)
                {
                    if (bestConstructor == null)
                    {
                        bestConstructor = constructors[i];
                        serviceTypes = currentServiceTypes;
                    }
                    else
                    {
                        // Since we're visiting constructors in decreasing order of number of parameters,
                        // we'll only see ambiguities or supersets once we've seen a 'bestConstructor'.

                        if (bestConstructorParameterTypes == null)
                        {
                            bestConstructorParameterTypes = new HashSet<Type>(serviceTypes);
                        }

                        if (!bestConstructorParameterTypes.IsSupersetOf(currentServiceTypes))
                        {
                            // Ambiguous match exception
                            var message = string.Join(
                                Environment.NewLine,
                                ServiceProviderUtilities.AmbiguousConstructorExceptionMessage(TypeNameHelper.GetTypeDisplayName(implementationType)),
                                bestConstructor,
                                constructors[i]);

                            throw new InvalidOperationException(message);
                        }
                    }
                }
            }

            // Check wherther the bestConstructor is defined
            if (bestConstructor == null)
            {
                throw new InvalidOperationException(ServiceProviderUtilities.UnableToActivateTypeExceptionMessage(
                                                        TypeNameHelper.GetTypeDisplayName(implementationType)));
            }

            chain.Remove(implementationType);
            return new ConstructorCallSite(bestConstructor, serviceTypes);
        }

        
        private Type[] GetServiceTypes(
            Type clientServiceType, 
            ParameterInfo[] parameters,
            ServiceProviderEngineScope scope,
            CallSiteChain chain,
            bool requireServiceExistence)
        {
            int count = parameters.Length;
            Type[] serviceTypes = new Type[count];

            for (int i = 0; i < count; i++)
            {
                Type serviceType = parameters[i].ParameterType;
                serviceTypes[i] = serviceType;

                // Check circular dependency and required service existence
                if (requireServiceExistence)
                {
                    if (!CheckCircularDependency(clientServiceType, serviceType, scope, chain, false))
                    {
                        serviceTypes = null; // Return null if service is not registered in container
                        break;
                    }
                }
                else
                {
                    CheckCircularDependency(clientServiceType, serviceType, scope, chain, false);
                }
            }

            return serviceTypes;
        }

        private ICallSite ApplyLifetime(ICallSite callSite, object cacheKey, ServiceLifetime descriptorLifetime)
        {
            // ConstantCallSite means a predefined singleton, that will be not disposed with the root scope.
            if (callSite.Kind == CallSiteKind.Constant)
            {
                return callSite;
            }

            switch (descriptorLifetime)
            {
                case ServiceLifetime.Transient:
                    return new TransientCallSite(callSite);
                case ServiceLifetime.Scoped:
                    return new ScopedCallSite(callSite, cacheKey);
                case ServiceLifetime.Singleton:
                    return new SingletonCallSite(callSite, cacheKey);
                default:
                    throw new ArgumentOutOfRangeException(nameof(descriptorLifetime));
            }
        }
    }
}
