using Modules.InAppPurchase.InternalRestore;
using UnityEditor;


namespace Modules.InAppPurchase.Editor.InternalRestore
{
    public class InternalRestorePrefsClear
    {
        private static InternalRestorePrefs restorePrefs;

        private static InternalRestorePrefs RestorePrefs => restorePrefs ?? (restorePrefs = InternalRestorePrefs.Open());


        [MenuItem("Modules/Internal restore/Clear all")]
        private static void Initialize()
        {
            RestorePrefs.RemoveAllStoreItem();
            RestorePrefs.Save();
        }
    }
}
