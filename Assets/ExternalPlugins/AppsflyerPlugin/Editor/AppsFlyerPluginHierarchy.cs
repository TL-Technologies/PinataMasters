using Modules.Hive.Editor;


namespace Modules.AppsFlyer.Editor
{
    public class AppsFlyerPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static AppsFlyerPluginHierarchy instance;

        #endregion



        #region Properties

        public static AppsFlyerPluginHierarchy Instance =>
            instance ?? (instance = new AppsFlyerPluginHierarchy("Modules.AppsFlyer"));

        #endregion



        #region Class lifecycle

        private AppsFlyerPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}
