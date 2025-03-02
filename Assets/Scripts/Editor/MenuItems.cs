using Modules.General.HelperClasses;
using UnityEditor;


namespace PinataMasters
{
    public static class MenuItems
    {

        [MenuItem("PinataMasters/Clear Save")]
        public static void ResetPrefs()
        {
            if (EditorUtility.DisplayDialog("Clear save", "Clear player save?", "OK", "Cancel"))
            {
                CustomPlayerPrefs.DeleteAll();
            }

        }
    }
}
