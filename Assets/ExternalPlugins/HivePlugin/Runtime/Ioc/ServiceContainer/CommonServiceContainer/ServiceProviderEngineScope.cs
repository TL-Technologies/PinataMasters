using System;
using System.Collections.Generic;


namespace Modules.Hive.Ioc
{
    internal class ServiceProviderEngineScope : IServiceScope
    {
        private bool isDisposed;
        private List<IDisposable> disposables;

        
        internal Dictionary<object, object> ResolvedServices { get; } // Sync object
        internal ServiceProviderEngine Engine { get; }
        public IServiceContainer ServiceContainer => Engine;

        
        internal ServiceProviderEngineScope(ServiceProviderEngine engine)
        {
            Engine = engine;
            ResolvedServices = new Dictionary<object, object>();
        }
        

        public void Dispose()
        {
            lock (ResolvedServices)
            {
                if (isDisposed)
                {
                    return;
                }

                isDisposed = true;

                if (disposables != null)
                {
                    for (int i = disposables.Count - 1; i >= 0; i--)
                    {
                        disposables[i].Dispose();
                    }

                    disposables.Clear();
                }

                ResolvedServices.Clear();
            }
        }

        
        internal object CaptureDisposable(object service)
        {
            if (!ReferenceEquals(this, service) && service is IDisposable disposable)
            {
                lock (ResolvedServices)
                {
                    if (disposables == null)
                    {
                        disposables = new List<IDisposable>();
                    }

                    disposables.Add(disposable);
                }
            }

            return service;
        }

        
        public object GetService(Type serviceType)
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(nameof(IServiceProvider));
            }

            return Engine.GetService(serviceType, this);
        }
    }
}
