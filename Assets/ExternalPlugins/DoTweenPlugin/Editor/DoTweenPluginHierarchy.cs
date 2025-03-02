using Modules.Hive.Editor;


namespace Modules.DoTween.Editor
{
    public class DoTweenPluginHierarchy : PluginHierarchy
    {
        private static DoTweenPluginHierarchy instance;
        
        
        public static DoTweenPluginHierarchy Instance =>
            instance ?? (instance = new DoTweenPluginHierarchy("Modules.DOTween"));
        
        
        public static string DoTweenSettingsDirectoryAssetPath => 
            UnityPath.Combine(UnityPath.ExternalPluginsSettingsAssetPath, "DoTweenSettings");
        
        
        private DoTweenPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }
    }
}
