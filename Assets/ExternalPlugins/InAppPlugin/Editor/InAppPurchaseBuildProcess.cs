using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using UnityEditor.Purchasing;


namespace Modules.InAppPurchase.Editor
{
    internal class InAppPurchaseBuildProcess : IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            string binariesDirectoryPath = UnityPath.Combine(
                InAppPurchasePluginHierarchy.Instance.RootPath,
                "Runtime",
                "UnityPurchasing",
                "Bin");
            context.ProjectSnapshot.SaveDirectoryStructure(binariesDirectoryPath);
            
            UnityPurchasingEditor.TargetAndroidStore(StoreUtilities.AndroidAppStoreType);
        }
    }
}
