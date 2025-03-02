using Modules.General.HelperClasses;
using System;
using UnityEngine;
#if UNITY_ANDROID
using Modules.Hive;
#endif


[CreateAssetMenu(fileName = "TargetSettings")]
public class TargetSettings : ScriptableSingleton<TargetSettings>
{
    #region Helpers
    
    [Serializable]
    private class PlatformSettings
    {
        public string applicationIdentifier = null;
        public string applicationName = null;
        public string applicationCompany = null;
        public string unityProjectId = null;
        public string versionName = null;
    }
    
    #endregion
    
    

    #region Variables

    [SerializeField] PlatformSettings iosSettings;
    [SerializeField] PlatformSettings androidSettings;
    [SerializeField] PlatformSettings amazonSettings;
    [SerializeField] PlatformSettings huaweiSettings;
    
    #endregion
    


    #region Properties

    public static string ApplicationIdentifier => Instance.Settings.applicationIdentifier;


    public static string ApplicationName => Instance.Settings.applicationName;


    public static string ApplicationCompany => Instance.Settings.applicationCompany;


    public static string UnityProjectId => Instance.Settings.unityProjectId;


    public static string VersionName => Instance.Settings.versionName;
    
    
    private PlatformSettings Settings
    {
        get
        {
            PlatformSettings platformSettings;
            #if UNITY_IOS
                platformSettings = iosSettings;
            #elif UNITY_ANDROID
                AndroidTarget androidTarget = PlatformInfo.AndroidTarget;
                if (androidTarget == AndroidTarget.GooglePlay)
                {
                    platformSettings = androidSettings;
                }
                else if (androidTarget == AndroidTarget.Amazon)
                {
                    platformSettings = amazonSettings;
                }
                else if (androidTarget == AndroidTarget.Huawei)
                {
                    platformSettings = huaweiSettings;
                }
                else
                {
                    throw new NotImplementedException();
                }
            #else
                platformSettings = new PlatformSettings();
            #endif
            
            return platformSettings;
        }
    }
        
    #endregion
}
