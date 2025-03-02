using Modules.Hive.Editor;
using UnityEngine;
using UnityEngine.Serialization;


namespace HivePlugin.Editor.BuildUtilities.PlatformUtilities.Android
{
    [CreateAssetMenu(fileName = "AndroidSignatureKeys",
        menuName = "BuildSettings/AndroidSignatureBuildSettings", order = 1)]
    internal class AndroidSignatureSettings : ScriptableObject
    {
        
        public bool AmazonSettingsValid =>
            AmazonKeyaliasName.NotNullAndEmpty() &&
            AmazonKeyaliasPass.NotNullAndEmpty() &&
            AmazonKeystoreName.NotNullAndEmpty() &&
            AmazonKeystorePass.NotNullAndEmpty();
       
        public bool GooglePlaySettingsValid =>
            GooglePlayKeyaliasName.NotNullAndEmpty() &&
            GooglePlayKeyaliasPass.NotNullAndEmpty() &&
            GooglePlayKeystoreName.NotNullAndEmpty() &&
            GooglePlayKeystorePass.NotNullAndEmpty();
        
        public bool HuaweiSettingsValid =>
            HuaweiKeyaliasName.NotNullAndEmpty() &&
            HuaweiKeyaliasPass.NotNullAndEmpty() &&
            HuaweiKeystoreName.NotNullAndEmpty() &&
            HuaweiKeystorePass.NotNullAndEmpty();


        public string GooglePlayKeystoreName
        {
            get => googlePlayKeystoreName;
            set => googlePlayKeystoreName = AbsolutePathToRelative(value);
        }
        
        public string GooglePlayKeystorePass;
        public string GooglePlayKeyaliasName;
        public string GooglePlayKeyaliasPass;


        public string AmazonKeystoreName
        {
            get => amazonKeystoreName;
            set => amazonKeystoreName = AbsolutePathToRelative(value);
        }
        
        public string AmazonKeystorePass;
        public string AmazonKeyaliasName;
        public string AmazonKeyaliasPass;


        public string HuaweiKeystoreName
        {
            get => huaweiPlayKeystoreName;
            set => huaweiPlayKeystoreName = AbsolutePathToRelative(value);
        }
        
        public string HuaweiKeystorePass;
        public string HuaweiKeyaliasName;
        public string HuaweiKeyaliasPass;

        public bool PreferCliSettings = false;
        
        [SerializeField] private string googlePlayKeystoreName;
        [SerializeField] private string amazonKeystoreName;
        [SerializeField] private string huaweiPlayKeystoreName;
        
        
        
        private string AbsolutePathToRelative(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            if (!value.Contains(Application.dataPath))
            {
                Debug.LogError($"Given path {value} is outside unity project.");
                return string.Empty;
            }

            return value.Replace(Application.dataPath, "");
        }
        
    }

    internal static class StringHelper
    {
        public static bool NotNullAndEmpty(this string s) => !string.IsNullOrEmpty(s);
    }
    
}
