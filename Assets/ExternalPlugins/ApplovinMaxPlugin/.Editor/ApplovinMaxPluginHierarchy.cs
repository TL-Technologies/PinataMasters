using Modules.Hive.Editor;


namespace Modules.Max.Editor
{
    public class ApplovinMaxPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static ApplovinMaxPluginHierarchy instance;

        #endregion



        #region Instancing

        public static ApplovinMaxPluginHierarchy Instance =>
            instance ?? (instance = new ApplovinMaxPluginHierarchy("Modules.Max"));


        public ApplovinMaxPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}
