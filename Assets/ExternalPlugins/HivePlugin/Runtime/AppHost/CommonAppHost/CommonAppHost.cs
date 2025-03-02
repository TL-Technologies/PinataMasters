using Modules.Hive.Ioc;
using Modules.Hive.Storage;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Hive
{
    internal abstract class CommonAppHost : IAppHost
    {
        #region Fields

        public event Action AppLaunch;
        public event Action AppResume;
        public event Action AppPause;
        public event Action AppQuit;
        
        private CommonAppHostBehaviour appHostBehaviour = null;
        private readonly AppLifecycleDispatcher appLifecycleDispatcher;
        private readonly AppHostPluginsHub plugins;
        private IStorageService storageService = null;
        
        private bool isLaunched = false;
        private bool isPaused = true;
        
        #endregion
        
        
        
        #region Properties

        public IServiceContainer ServiceContainer { get; }
        public IEventAggregator EventAggregator { get; }
        public IStorageService StorageService => storageService ?? (storageService = ServiceContainer.GetRequiredService<IStorageService>());
        public IAppLifecycleDispatcher AppLifecycleDispatcher
        {
            get => appLifecycleDispatcher;
        }
        
        #endregion
        
        
        
        #region Class lifecycle

        internal CommonAppHost(
            IServiceContainer serviceContainer,
            IEventAggregator eventAggregator,
            AppLifecycleDispatcher appLifecycleDispatcher,
            AppHostPluginsHub plugins)
        {
            ServiceContainer = serviceContainer;
            EventAggregator = eventAggregator;
            this.appLifecycleDispatcher = appLifecycleDispatcher;
            this.plugins = plugins;

            // Setup service container
            ServiceContainer.CreateService()
                .SetImplementationInstance(this)
                .BindTo<IAppHost>(ServiceBindingOptions.Exclusive)
                .AsSingleton();
        }
        
        #endregion
        
        
        
        #region Async

        public async Task RunAsync()
        {
            // Do it first! Attach the instance to AppHost wrapper
            AppHost.AttachAppHost(this);

            // Create and setup app host behaviour
            appHostBehaviour = new GameObject("HiveAppHost").AddComponent<CommonAppHostBehaviour>();
            appHostBehaviour.appHost = this;
            UnityEngine.Object.DontDestroyOnLoad(appHostBehaviour.gameObject);

            // Setup and run native apphost plugin (stage 1)
            await RunAsyncImpl();

            // Enumerate all chunks to configure and start plugins
            foreach (IAppHostPluginsHubChunk chunk in plugins)
            {
                // Configure plugins
                foreach (IAppHostPlugin plugin in chunk)
                {
                    await plugin.ConfigureAsync(this);
                }

                // Run plugins
                foreach (IAppHostPlugin plugin in chunk)
                {
                    await plugin.RunAsync(this);
                }
            }

            // App lifecycle
            ProcessAppLaunch();
        }
        
        
        protected abstract Task RunAsyncImpl();
        
        #endregion

        
        
        #region Lifecycle

        internal void ProcessAppLaunch()
        {
            if (isLaunched)
            {
                return;
            }

            isLaunched = true;

            // Dispatch event
            appLifecycleDispatcher.EnumerateForward(handler => handler.OnAppLaunch());
            OnAppLaunch();

            // Process AppResume 
            ProcessAppResume();
        }

        
        internal void ProcessAppResume()
        {
            if (!isLaunched || !isPaused)
            {
                return;
            }

            isPaused = false;

            // Dispatch event
            appLifecycleDispatcher.EnumerateForward(handler => handler.OnAppResume());
            OnAppResume();
        }
        

        internal void ProcessAppPause()
        {
            if (!isLaunched || isPaused)
            {
                return;
            }

            isPaused = true;

            // Dispatch event
            OnAppPause();
            appLifecycleDispatcher.EnumerateBackward(handler => handler.OnAppPause());
        }
        

        internal void ProcessAppQuit()
        {
            if (!isLaunched)
            {
                return;
            }

            // Process AppPause
            ProcessAppPause();
            
            List<Exception> exceptions = new List<Exception>();
            // Dispatch event
            try
            {
                OnAppQuit();
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
            appLifecycleDispatcher.EnumerateBackwardSafely(handler => handler.OnAppQuit(), exceptions);
            
            isLaunched = false;
            AppHost.DetachAppHost(this);
            
            if (exceptions.Count > 0)
            {
                throw new AggregateException(exceptions);
            }
        }
        

        internal void ProcessUpdate()
        {
            // TODO: Scheduler
        }
        
        
        protected virtual void OnAppLaunch()
        {
            AppLaunch?.Invoke();
        }

        
        protected virtual void OnAppResume()
        {
            AppResume?.Invoke();
        }

        
        protected virtual void OnAppPause()
        {
            AppPause?.Invoke();
        }

        
        protected virtual void OnAppQuit()
        {
            AppQuit?.Invoke();
        }

        #endregion
    }
}
