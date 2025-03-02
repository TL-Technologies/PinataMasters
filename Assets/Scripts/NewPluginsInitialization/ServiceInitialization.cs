using Modules.Advertising;
using Modules.Analytics;
using Modules.AppsFlyer;
//using Modules.Firebase;
using Modules.General;
using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.InAppPurchase;
using Modules.Max;
using System;


public class ServiceInitialization
{
    #region Fields

    public event Action OnServicesInitialized;

    private readonly InGameAbTestData inGameAbTestData = new InGameAbTestData();
    private readonly RemoteAvailabilityAbTestData remoteAvailabilityAbTestData = new RemoteAvailabilityAbTestData();

    #endregion



    #region Methods

    public void Initialize()
    {
        AnalyticsManagerSettings analyticsSettings = new AnalyticsManagerSettings
        {
            Services = new IAnalyticsService[]
            {
                new AppsFlyerAnalyticsServiceImplementor(LLAppsFlyerSettings.GetAppsFlyerSettings()),
//                new FirebaseAnalyticsServiceImplementor(LLFirebaseSettings.Instance)
            }
        };
        
        AdvertisingNecessaryGameData advertisingNecessaryInfo = new AdvertisingNecessaryGameData();

        AppsFlyerCrossPromoTracker crossPromoTracker = new AppsFlyerCrossPromoTracker();

        AdvertisingManagerSettings advertisingSettings = new AdvertisingManagerSettings
        {
            AdServices = new IAdvertisingService[]
            {
                new MaxAdvertisingServiceImplementor(),
                new EditorAdvertisingServiceImplementor(AdvertisingEditorSettings.Instance)
            },

            AdvertisingInfo = advertisingNecessaryInfo,

            AbTestData = new IAdvertisingAbTestData[] { inGameAbTestData },
        };

        Services.CreateServiceSingleton<IAnalyticsManagerSettings, AnalyticsManagerSettings>(analyticsSettings);
        Services.CreateServiceSingleton<IAdvertisingManagerSettings, AdvertisingManagerSettings>(advertisingSettings);

        IPurchaseAnalyticsParameters purchaseAnalyticsParameters = new PurchaseAnalyticsParametersImplementor();
        purchaseAnalyticsParameters.SetParameter(
            "placement",
            () => DataStateService.Instance.Get("placement", SubscriptionPurchasePlacement.Default));
        Services.CreateServiceSingleton<IPurchaseAnalyticsParameters, PurchaseAnalyticsParametersImplementor>(purchaseAnalyticsParameters);

        if (InitializationQueueConfiguration.DoesInstanceExist)
        {
            InitializationQueue.Instance
                               .SetOnComplete(OnInitQueueComplete)
                               .SetOnError(Initialization_OnError)
                               .Apply(InitializationQueueConfiguration.Instance);
        }
    }

    #endregion



    #region Events handlers

    private void OnInitQueueComplete()
    {
        RefreshAdvertisingNecessaryInfo();

        OnServicesInitialized?.Invoke();
    }


    private static void Initialization_OnError(object registerable, InitializationStatus initializationStatus)
    {
        CustomDebug.LogError($"Error has occured while initialization: {initializationStatus}");
    }


    private void RefreshAdvertisingNecessaryInfo()
    {
        IAdvertisingManagerSettings advertisingManagerSettings = Services.GetService<IAdvertisingManagerSettings>();
        AdvertisingNecessaryGameData advertisingNecessaryInfo =
            (AdvertisingNecessaryGameData)advertisingManagerSettings.AdvertisingInfo;
        advertisingNecessaryInfo.InitListeners();
    }

    #endregion
}

