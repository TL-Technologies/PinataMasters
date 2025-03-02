using Modules.General.Abstraction;
using Modules.General.Abstraction.InAppPurchase;
using Modules.Hive.Ioc;

namespace Modules.General
{
    public static class Services
    {
        #region Fields

        private static ServiceContainer container = new ServiceContainer();

        private static IAdvertisingManager advertisingManager;
        private static ISoundManager soundManager;
        private static IAbTestService abTestService;
        private static IAdvertisingManagerSettings advertisingManagerSettings;
        private static IStatisticsManager statisticsManager;
        private static IAtTrackingManager atTrackingManager;
        private static IAnalyticsManager analyticsManager;
        private static IAnalyticsProcessor analyticsProcessor;
        private static IAnalyticsManagerSettings analyticsManagerSettings;
        private static INotificationManager notificationManager;
        private static IPrivacyManager privacyManager;
        private static ISystemPopupService systemPopupService;
        private static IEventsSamplingService eventsSamplingService;
        private static IStoreManager storeManager;

        #endregion



        #region Properties

        public static IAdvertisingManager AdvertisingManager =>
            advertisingManager ?? (advertisingManager = GetService<IAdvertisingManager>());

        public static ISoundManager SoundManager =>
            soundManager ?? (soundManager = GetService<ISoundManager>());

        public static IAbTestService AbTestService =>
            abTestService ?? (abTestService = GetService<IAbTestService>());

        public static IAdvertisingManagerSettings AdvertisingManagerSettings =>
            advertisingManagerSettings ?? (advertisingManagerSettings = GetService<IAdvertisingManagerSettings>());

        public static IStatisticsManager StatisticsManager =>
            statisticsManager ?? (statisticsManager = GetService<IStatisticsManager>());

        public static IAtTrackingManager AtTrackingManager =>
            atTrackingManager ?? (atTrackingManager = GetService<IAtTrackingManager>());

        public static IAnalyticsManager AnalyticsManager =>
            analyticsManager ?? (analyticsManager = GetService<IAnalyticsManager>());

        public static IAnalyticsProcessor AnalyticsProcessor =>
            analyticsProcessor ?? (analyticsProcessor = GetService<IAnalyticsProcessor>());

        public static IAnalyticsManagerSettings AnalyticsManagerSettings =>
            analyticsManagerSettings ?? (analyticsManagerSettings = GetService<IAnalyticsManagerSettings>());

        public static INotificationManager NotificationManager =>
            notificationManager ?? (notificationManager = GetService<INotificationManager>());

        public static IPrivacyManager PrivacyManager =>
            privacyManager ?? (privacyManager = GetService<IPrivacyManager>());

        public static ISystemPopupService SystemPopupService =>
            systemPopupService ?? (systemPopupService = GetService<ISystemPopupService>());

        public static IServiceContainer Container => container;

        public static IEventsSamplingService EventsSamplingService =>
            eventsSamplingService ?? (eventsSamplingService = GetService<IEventsSamplingService>());

        public static IStoreManager StoreManager => storeManager ?? (storeManager = GetService<IStoreManager>());
        #endregion



        #region Methods

        public static IUnityFluentDescriptor<T> CreateUnityService<T>() where T : UnityEngine.Object
        {
            return container.CreateUnityService<T>();
        }


        public static IUnityFluentDescriptor<TService> CreateUnityService<TInterface, TService>(TService instance)
            where TService : UnityEngine.Object, TInterface
        {
            return container.CreateUnityService<TService>()
                            .BindTo<TInterface>()
                            .SetInstantiateOnDemand(false)
                            .SetImplementationInstance(instance)
                            .SetUseInstance();
        }


        public static T GetService<T>() where T : class
        {
            return container.GetService<T>();
        }


        public static T GetRequiredService<T>() where T : class
        {
            return container.GetRequiredService<T>();
        }


        public static IServiceFluentDescriptor<TService> CreateService<TInterface, TService>() where TService : class
        {
            return container.CreateService<TService>().BindTo<TInterface>();
        }


        public static IServiceFluentDescriptor<TService> CreateService<TInterface, TService>(TService instance)
            where TService : class, TInterface
        {
            return container.CreateService<TService>().SetImplementationInstance(instance).BindTo<TInterface>();
        }


        public static IServiceFluentDescriptor<TService> CreateService<TInterface, TService>(TInterface instance)
            where TService : class, TInterface
        {
            TService service = instance as TService;
            if (service != null)
            {
                return CreateService<TInterface, TService>(service);
            }

            return null;
        }


        public static void CreateServiceScoped<TInterface, TService>() where TService : class
        {
            CreateService<TInterface, TService>().AsScoped();
        }


        public static void CreateServiceSingleton<TInterface, TService>() where TService : class
        {
            CreateService<TInterface, TService>().AsSingleton();
        }


        public static void CreateServiceSingleton<TInterface, TService>(TService instance)
            where TService : class, TInterface
        {
            CreateService<TInterface, TService>(instance)?.AsSingleton();
        }


        public static void CreateServiceSingleton<TInterface, TService>(TInterface instance)
            where TService : class, TInterface
        {
            CreateService<TInterface, TService>(instance)?.AsSingleton();
        }

        #endregion
    }
}
