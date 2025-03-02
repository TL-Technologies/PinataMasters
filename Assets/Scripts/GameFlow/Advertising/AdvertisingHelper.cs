using Modules.Advertising;
using Modules.Analytics;
using System;
    

namespace PinataMasters
{
    public static class AdvertisingHelper
    {
        #region Fields
        
        private static CustomAdvertisingManager cachedAdvertisingManager = null;

        #endregion



        #region Properties

        static CustomAdvertisingManager CachedAdvertisingManager
        {
            get
            {
                if (cachedAdvertisingManager == null)
                {
                    cachedAdvertisingManager = AdvertisingManager.Instance as CustomAdvertisingManager;
                }

                return cachedAdvertisingManager;
            }
        }
        
        #endregion



        #region Methods
        
        public static void ShowVideo(Action<bool> callback, string placement, string reward = null)
        {
            CachedAdvertisingManager.ShowVideo((bool result) =>
            {
                callback?.Invoke(result);
            }, placement);
        }


        public static void ShowInterstitial(string placement, Action callback = null)
        {
            CachedAdvertisingManager.ShowInterstitial(placement, callback);
        }


        public static void ShowBanner()
        {
            CachedAdvertisingManager.ShowBanner();
        }


        public static void HideBanner()
        {
            if (CachedAdvertisingManager != null)
            {
                CachedAdvertisingManager.HideBanner();
            }
        }

        #endregion
    }
}
