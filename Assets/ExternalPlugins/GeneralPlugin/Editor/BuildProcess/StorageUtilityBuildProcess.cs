#if UNITY_IOS
using Modules.General.Utilities.StorageUtility;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Ios;
using UnityEditor.iOS.Xcode;


namespace Modules.General.Editor.BuildProcess
{
    internal class StorageUtilityBuildProcess : IBuildPostprocessor<IIosBuildPostprocessorContext>
    {
        private const string KeychainAccessGroupsEntitlementsKey = "keychain-access-groups";
        
        
        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            PlistElementArray entitlementsInfo = new PlistElementArray();
            entitlementsInfo.AddString($"$(AppIdentifierPrefix){IosKeystoreStorage.keychainAccessGroupId}");
            context.Entitlements.root.SetArray(KeychainAccessGroupsEntitlementsKey, entitlementsInfo);
            context.Entitlements.Save();
            
            context.PbxProject.AddCapability(
                context.PbxProject.GetUnityMainTargetGuid(), 
                PBXCapabilityType.KeychainSharing, 
                context.Entitlements.OutputPath);
        }
    }
}
#endif

