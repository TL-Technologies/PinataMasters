using Modules.Hive.Editor;


namespace UnityEditor.Purchasing 
{
    public class UnityIapPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static UnityIapPluginHierarchy instance;

        #endregion

        
        
        #region Instancing
        
        public static UnityIapPluginHierarchy Instance => 
            instance ?? (instance = new UnityIapPluginHierarchy("UnityEditor.Purchasing"));
        
        
        public UnityIapPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }
        
        #endregion



        #region Methods

        public string GetPathWithRoot(string pathInRoot) => UnityPath.Combine(RootAssetPath, pathInRoot);

        #endregion
    }
}