using Modules.Hive.Editor;


namespace Modules.Odin.Editor
{
    public class OdinPluginHierarchy : PluginHierarchy
    {
        private static OdinPluginHierarchy instance;
        
        
        public static OdinPluginHierarchy Instance =>
            instance ?? (instance = new OdinPluginHierarchy("Modules.Odin"));
        
        
        public static string OdinSettingsDirectoryAssetPath => 
            UnityPath.Combine(UnityPath.ExternalPluginsSettingsAssetPath, "OdinSettings", "Sirenix");
        
        
        public OdinPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }
    }
}