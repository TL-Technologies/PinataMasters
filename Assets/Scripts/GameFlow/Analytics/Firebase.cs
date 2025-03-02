using Modules.AppsFlyer;
using Modules.General;
using Modules.General.HelperClasses;
using System.Collections.Generic;


namespace PinataMasters
{
    public static class Firebase
    {
        #region Fields

        const string IsAppsflyerIdSendedKey = "is_appsflyer_id_sended";

        #endregion
        
        
        
        #region Properties

        static bool IsAppsflyerIdSended
        {
            get
            {
                return CustomPlayerPrefs.GetBool(IsAppsflyerIdSendedKey, false);
            }
            set
            {
                CustomPlayerPrefs.SetBool(IsAppsflyerIdSendedKey, value);
            }
        }

        #endregion
        
        
        
        #region Public methods

        public static void TrySendAppsflyerIDEvent()
        {
            AppsFlyerAnalyticsServiceImplementor appsFlyerAnalyticsServiceImplementor = Services.AnalyticsProcessor.GetService<AppsFlyerAnalyticsServiceImplementor>();

            if (!IsAppsflyerIdSended && appsFlyerAnalyticsServiceImplementor != null)
            {
                
//                Services.AnalyticsManager.SendEvent(typeof(FirebaseAnalyticsServiceImplementor), "appsflyer_id_did_fetch", new Dictionary<string, string>
//                { 
//                    { "appsflyer_id" , appsFlyerAnalyticsServiceImplementor.LLAppsFlyerGetAppsFlyerUID() }
//                });

                IsAppsflyerIdSended = true;
            }
        }
        
        #endregion
    }
}
