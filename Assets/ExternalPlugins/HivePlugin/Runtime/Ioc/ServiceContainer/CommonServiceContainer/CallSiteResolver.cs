using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ExceptionServices;


namespace Modules.Hive.Ioc
{
    internal class CallSiteResolver : ICallSiteVisitor
    {
        public object Resolve(Type serviceType, ServiceProviderEngineScope scope)
        {
            return
                TryResolveExact(serviceType, scope) ??
                TryResolveCollection(serviceType, scope);
        }

        
        private object TryResolveExact(Type serviceType, ServiceProviderEngineScope scope)
        {
            ServiceEntityCollection item = scope.Engine.GetServiceEntityCollection(serviceType);
            return item.Empty ? null : ResolveServiceImplementation(serviceType, scope, item.Last);
        }

        
        private object TryResolveCollection(Type serviceType, ServiceProviderEngineScope scope)
        {
            if (!ServiceProviderUtilities.IsServiceCollectionType(serviceType))
            {
                return null;
            }

            serviceType = serviceType.GenericTypeArguments.Single();

            ServiceEntityCollection item = scope.Engine.GetServiceEntityCollection(serviceType);
            if (item.Empty)
            {
                return null;
            }

            int count = item.Count;
            var array = Array.CreateInstance(serviceType, count);
            for (int i = 0; i < count; i++)
            {
                object obj = ResolveServiceImplementation(serviceType, scope, item[i]);
                array.SetValue(obj, i);
            }

            return array;
        }

        
        private object ResolveServiceImplementation(Type serviceType, ServiceProviderEngineScope scope, ServiceEntity entity)
        {
            ICallSite callSite = scope.Engine.GetServiceCallSite(entity, scope, null);
            object obj = callSite.Accept(this, scope);
            ValidateServiceImplementation(serviceType, obj);

            // Call optional injectors here

            return obj;
        }
        

        [Conditional("DEBUG")]
        private void ValidateServiceImplementation(Type serviceType, object implementation)
        {
            Type implementationType = implementation.GetType();
            if (implementation != null && !serviceType.IsAssignableFrom(implementationType))
            {
                throw new InvalidCastException(ServiceProviderUtilities.InvalidServiceImplementationTypeExceptionMessage(
                                                   TypeNameHelper.GetTypeDisplayName(serviceType),
                                                   TypeNameHelper.GetTypeDisplayName(implementationType)));
            }
        }

        
        #region Lifetime call sites

        public object VisitTransientCallSite(TransientCallSite callSite, ServiceProviderEngineScope scope)
        {
            object obj = callSite.CallSite.Accept(this, scope);

            // Capture disposable transient object if required
            if (scope.Engine.Options.CaptureDisposableTransientServices)
            {
                scope.CaptureDisposable(obj);
            }

            return obj;
        }

        
        public object VisitScopedCallSite(ScopedCallSite callSite, ServiceProviderEngineScope scope)
        {
            lock (scope.ResolvedServices)
            {
                if (!scope.ResolvedServices.TryGetValue(callSite.CacheKey, out var resolved))
                {
                    resolved = callSite.CallSite.Accept(this, scope);
                    scope.CaptureDisposable(resolved);
                    scope.ResolvedServices.Add(callSite.CacheKey, resolved);
                }

                return resolved;
            }
        }

        
        public object VisitSingletonCallSite(SingletonCallSite callSite, ServiceProviderEngineScope scope)
        {
            return VisitScopedCallSite(callSite, scope.Engine.RootScope);
        }

        #endregion
        
        

        #region Instance creation call sites

        public object VisitActivatorCallSite(ActivatorCallSite callSite, ServiceProviderEngineScope scope)
        {
            try
            {
                return Activator.CreateInstance(callSite.implementationType);
            }
            catch (Exception ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // The above line will always throw, but the compiler requires we throw explicitly.
                throw;
            }
        }

        public object VisitConstructorCallSite(ConstructorCallSite callSite, ServiceProviderEngineScope scope)
        {
            var argTypes = callSite.serviceTypes;
            var args = new object[argTypes.Length];
            for (int i = 0; i < argTypes.Length; i++)
            {
                args[i] = scope.Engine.GetService(argTypes[i], scope);
            }

            try
            {
                return callSite.constructorInfo.Invoke(args);
            }
            catch (Exception ex) when (ex.InnerException != null)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
                // The above line will always throw, but the compiler requires we throw explicitly.
                throw;
            }
        }

        
        public object VisitConstantCallSite(ConstantCallSite callSite, ServiceProviderEngineScope scope)
        {
            return callSite.implementationInstance;
        }

        
        public object VisitFactoryCallSite(FactoryCallSite callSite, ServiceProviderEngineScope scope)
        {
            return callSite.implementationFactory(scope);
        }

        #endregion
    }
}
