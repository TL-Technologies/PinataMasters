using Newtonsoft.Json;
using System.IO;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class PackageSignOptions
    {
        [JsonProperty] public string KeyStorePath { get; private set; }
        [JsonProperty] internal string KeyStorePassword { get; private set; }
        [JsonProperty] public string KeyAliasName { get; private set; }
        [JsonProperty] internal string KeyAliasPassword { get; private set; }


        [JsonConstructor]
        private PackageSignOptions() { }


        public PackageSignOptions(
            string keyStorePath, 
            string keyStorePassword, 
            string keyAliasName, 
            string keyAliasPassword)
        {
            KeyStorePath = keyStorePath;
            KeyStorePassword = keyStorePassword;
            KeyAliasName = keyAliasName;
            KeyAliasPassword = keyAliasPassword;
        }


        public PackageSignOptions(string path)
        {
            if (!File.Exists(path))
            {
                throw new IOException($"Keystore not found at path {path}");
            }

            KeyStorePath = path;
            KeyStorePassword = GetStringFromFile(path, "keystorepass");
            KeyAliasName = GetStringFromFile(path, "keyalias");
            KeyAliasPassword = GetStringFromFile(path, "keyaliaspass");
        }


        private string GetStringFromFile(string path, string overrideExtension)
        {
            path = Path.ChangeExtension(path, overrideExtension);
            return File.ReadAllText(path).Trim(' ', '\t', '\r', '\n');
        }


        internal void Apply()
        {
            #if UNITY_2019_1_OR_NEWER
                PlayerSettings.Android.useCustomKeystore = true;
            #endif
            PlayerSettings.Android.keystoreName = KeyStorePath;
            PlayerSettings.Android.keystorePass = KeyStorePassword;
            PlayerSettings.Android.keyaliasName = KeyAliasName;
            PlayerSettings.Android.keyaliasPass = KeyAliasPassword;
        }
    }
}
