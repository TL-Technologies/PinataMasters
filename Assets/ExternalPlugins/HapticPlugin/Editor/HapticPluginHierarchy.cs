using Modules.Hive.Editor;


namespace Modules.Haptic.Editor
{
    public class HapticPluginHierarchy : PluginHierarchy
    {
        #region Fields

        private static HapticPluginHierarchy instance;

        #endregion



        #region Properties

        public static HapticPluginHierarchy Instance =>
            instance ?? (instance = new HapticPluginHierarchy("Modules.Haptic"));

        #endregion



        #region Class lifecycle

        private HapticPluginHierarchy(string mainAssemblyName) : base(mainAssemblyName) { }

        #endregion
    }
}