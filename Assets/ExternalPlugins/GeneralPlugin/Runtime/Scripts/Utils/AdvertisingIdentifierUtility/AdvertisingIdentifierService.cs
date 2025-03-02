using Modules.General.Abstraction;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive;
using Modules.Hive.Ioc;
using System;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.General.Utilities
{
    [InitQueueService(
        -20000,
        typeof(IAdvertisingIdentifier))]
    public class AdvertisingIdentifierService : IAdvertisingIdentifier, IInitializableService
    {
        #region Fields

        private const string DefaultAdvertisingId = "00000000-0000-0000-0000-000000000000";
        private const string GetAmazonAdvertisingIdMethodName = "UtilitiesGetAmazonAdvertisingId";
        
        private string advertisingIdentifier = string.Empty;

        #endregion
        
        
        
        #region IAdvertisingIdentifier

        public void GetAdvertisingIdentifier(Action<string> callback, bool shouldResetCachedId = false)
        {
            if (shouldResetCachedId)
            {
                advertisingIdentifier = string.Empty;
            }
            
            if (!string.IsNullOrEmpty(advertisingIdentifier))
            {
                callback?.Invoke(advertisingIdentifier);
                return;
            }
            
            Action<string> invokeCallbackAction = id =>
            {
                advertisingIdentifier = id;
                callback?.Invoke(advertisingIdentifier);
            };
            
            bool isRequestSupportedByPlatform = Application.RequestAdvertisingIdentifierAsync((advertisingId, trackingEnabled, error) =>
            {
                invokeCallbackAction(advertisingId);
            });
            
            // If the request is not supported by the platform, Unity doesn't call the inner delegate.
            if (!isRequestSupportedByPlatform)
            {
                // Unity method doesn't support Amazon platform, but idfa can be retrieved using native code
                if (PlatformInfo.AndroidTarget == AndroidTarget.Amazon)
                {
                    string id = LLAndroidJavaSingletone<Utilities>.CallStatic<string>(GetAmazonAdvertisingIdMethodName);
                    invokeCallbackAction(id);
                }
                else if (PlatformInfo.AndroidTarget == AndroidTarget.Huawei)
                {
                    var huaweiService = Services.GetService<IHuaweiServices>();
                    string id = huaweiService?.GetAdvertisingIdentifier() ?? DefaultAdvertisingId; 
                    invokeCallbackAction(id);
                }
                else
                {
                    invokeCallbackAction(DefaultAdvertisingId);
                }
            }
        }


        public async Task<string> GetAdvertisingIdentifierAsync(int delay = 250, bool shouldResetCachedId = false)
        {
            bool isCallbackCalled = false;
            GetAdvertisingIdentifier(id =>
            {
                isCallbackCalled = true;
            }, shouldResetCachedId);

            while (!isCallbackCalled)
            {
                await Task.Delay(delay);
            }

            return advertisingIdentifier;
        }

        #endregion



        #region IInitializableService

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            GetAdvertisingIdentifier(null);
            
            onCompleteCallback?.Invoke();
        }

        #endregion



        #region Code stripping resolver

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            #if UNITY_IOS && !UNITY_EDITOR && UNITY_2019_3_OR_NEWER
                // Do not delete! Need for fix stripped code bug in Application.RequestAdvertisingIdentifierAsync method
                // https://forum.unity.com/threads/application-requestadvertisingidentifierasync-returns-empty-idfa-on-ios.856207/
                Debug.Log($"AdvertisingId: {UnityEngine.iOS.Device.advertisingIdentifier}");
            #endif
        }

        #endregion
    }
}
