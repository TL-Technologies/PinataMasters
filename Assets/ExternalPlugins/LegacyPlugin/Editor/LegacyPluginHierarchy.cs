using Modules.Hive.Editor;


public class LegacyPluginHierarchy : PluginHierarchy
{
    private static LegacyPluginHierarchy instance;
        
    public static LegacyPluginHierarchy Instance => 
        instance ?? (instance = new LegacyPluginHierarchy("Modules.Legacy"));
        
    private LegacyPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }
}
