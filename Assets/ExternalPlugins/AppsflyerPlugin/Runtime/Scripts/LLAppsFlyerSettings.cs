using Modules.General.HelperClasses;
using UnityEngine;


namespace Modules.AppsFlyer
{
    [CreateAssetMenu(fileName = "LLAppsFlyerSettings")]
    public class LLAppsFlyerSettings : ScriptableSingleton<LLAppsFlyerSettings>
    {
        #region Variables

        [Header("App ID")] public string appID = "";

        [Header("Dev Key")] public string devKey = "";

        #endregion



        #region Public methods

        public static string AppID()
        {
            string result = LLAppsFlyerSettings.Instance.appID;
            return result;
        }


        public static string DevKey()
        {
            string result = LLAppsFlyerSettings.Instance.devKey;
            return result;
        }


        public static AppsFlyerSettings GetAppsFlyerSettings(bool? isDebug = null) =>
            new AppsFlyerSettings(Instance.appID, Instance.devKey, isDebug);

        #endregion
    }
}
