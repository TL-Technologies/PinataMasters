using Modules.Hive.Editor;


namespace Modules.InAppPurchase.Editor
{
    internal sealed class InAppPurchasePluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static InAppPurchasePluginHierarchy instance;

        #endregion



        #region Instancing
        
        public static InAppPurchasePluginHierarchy Instance =>
            instance ?? (instance = new InAppPurchasePluginHierarchy("Modules.InAppPurchase"));
        

        public InAppPurchasePluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}
