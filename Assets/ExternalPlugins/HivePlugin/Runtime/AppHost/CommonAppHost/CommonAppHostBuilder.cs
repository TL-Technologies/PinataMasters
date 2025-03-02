using Modules.Hive.Ioc;
using System;


namespace Modules.Hive
{
    public class CommonAppHostBuilder : IAppHostBuilder
    {
        #region Fields
        
        private IServiceContainer serviceContainer = null;
        private IEventAggregator eventAggregator = null;
        private AppLifecycleDispatcher appLifecycleDispatcher = new AppLifecycleDispatcher();
        private AppHostPluginsHub plugins = new AppHostPluginsHub();
        
        #endregion
        
        
        
        #region Properties

        public IServiceContainer ServiceContainer
        {
            get => serviceContainer ?? (serviceContainer = new ServiceContainer());
        }
        
        
        public IEventAggregator EventAggregator
        {
            get => eventAggregator ?? (eventAggregator = new EventAggregator());
        }
        
        #endregion
        
        
        
        #region Methods

        public IAppHostBuilder SetServiceContainer(IServiceContainer container)
        {            
            if (ServiceContainer.Count > 0)
            {
                throw new InvalidOperationException("Failed to change a ServiceContainer implementation because current implementation is already in use. To avoid this issue you should call SetServiceContainer on top before any other methods of IAppHostBuilder.");
            }

            serviceContainer = container;
            return this;
        }
        

        public IAppHostBuilder SetEventAggregator(IEventAggregator aggregator)
        {
            if (EventAggregator.SubscribersCount > 0)
            {
                throw new InvalidOperationException("Failed to change a EventAggregator implementation because current implementation is already in use. To avoid this issue you should call SetEventAggregator on top before any other methods of IAppHostBuilder.");
            }

            eventAggregator = aggregator;
            return this;
        }
        

        public IAppHostBuilder AddAppLifecycleHandler(IAppLifecycleHandler appLifecycleHandler)
        {
            if (appLifecycleHandler != null)
            {
                appLifecycleDispatcher.Add(appLifecycleHandler);
            }

            return this;
        }
        

        public IAppHostBuilder AddPlugin(IAppHostPlugin appHostPlugin)
        {
            if (appHostPlugin != null)
            {
                plugins.AddPlugin(appHostPlugin);
                appHostPlugin.Setup(this);
            }

            return this;
        }

        
        public T GetPlugin<T>() where T : IAppHostPlugin
        {
            return plugins.GetPlugin<T>();
        }
        
        
        public IAppHost Build()
        {
            #if UNITY_EDITOR
                return new CommonAppHostDummy(
            #elif UNITY_ANDROID
                return new CommonAppHostAndroid(
            #elif UNITY_IOS
                return new CommonAppHostIos(
            #else
                return new CommonAppHostDummy(
            #endif
                ServiceContainer,
                EventAggregator,
                appLifecycleDispatcher,
                plugins);
        }
        
        #endregion
    }
}
