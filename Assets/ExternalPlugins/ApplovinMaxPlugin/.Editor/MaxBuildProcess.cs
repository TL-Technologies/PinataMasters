using Modules.Hive;
using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Android;
using Modules.Hive.Editor.BuildUtilities.Ios;
//using UnityEditor.iOS.Xcode.Extensions;
using System;
using System.IO;


namespace Modules.Max.Editor
{
    public class MaxBuildProcess : IBuildPreprocessor<IIosBuildPreprocessorContext>, IBuildPostprocessor<IIosBuildPostprocessorContext>, IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        private const string XcodeSdkFolder = "MaxSDK";
        private const string AdaptersFrameworksFolderName = "MaxAdaptersFrameworks";
        private const string AdapterName = "AdMob";
        

        public void OnPreprocessBuild(IIosBuildPreprocessorContext context)
        {
            if (!LLMaxSettings.Instance.EnabledAdapters.Contains("FacebookAdapter"))
            {
                string path = UnityPath.Combine(ApplovinMaxPluginHierarchy.Instance.RootPath,
                    "ExternalDependencies/AudienceNetwork");
                ProjectSnapshot.CurrentSnapshot.SaveDirectoryStructure(path);
                Directory.Delete(path, true);
            }
        }
        
        
        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            var appId = AppLovinSettings.Instance.AdMobIosAppId;
            if (string.IsNullOrEmpty(appId))
            {
                throw new Exception("[AppLovin MAX] AdMob App ID is not set. Please enter a valid app ID for iOs.");
            }
            
            context.InfoPlist.SetString("GADApplicationIdentifier", appId);
            context.PbxProject.SetBuildProperty("EMBEDDED_CONTENT_CONTAINS_SWIFT", "YES", PbxProjectTargetType.Main);
            context.PbxProject.AddLinkerFlags(PbxProjectTargetType.Main, LinkerFlag.AllLoad);

            if (LLMaxSettings.Instance.EnabledAdapters.Contains("GoogleAdManagerAdapter"))
            {
                context.InfoPlist.SetBoolean("GADIsAdManagerApp", true);
            }
            
            if (LLMaxSettings.Instance.EnabledAdapters.Contains("FacebookAdapter"))
            {
                context.InfoPlist.AddApplicationQueriesSchemes("fb");
            }
            
            string pathInIosSdkFolder = UnityPath.Combine(AdaptersFrameworksFolderName, AdapterName);
            string destinationPath = UnityPath.Combine(context.GetDestinationPath(XcodeSdkFolder), pathInIosSdkFolder);

            #if FIREBASE_CORE
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "GoogleAppMeasurement.framework"));
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "GoogleAppMeasurement.xcframework"));
                            
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "GoogleUtilities.framework"));
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "GoogleUtilities.xcframework"));
                            
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "nanopb.framework"));
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "nanopb.xcframework"));
                            
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "PromisesObjC.framework"));
                FileSystemUtilities.Delete(UnityPath.Combine(destinationPath, "PromisesObjC.xcframework"));
            #endif
        }


        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            var appId = AppLovinSettings.Instance.AdMobAndroidAppId;
            if (string.IsNullOrEmpty(appId))
            {
                throw new Exception($"[AppLovin MAX] AdMob App ID is not set. Please enter a valid app ID for {PlatformInfo.AndroidTarget} platform");
            }
            
            context.AndroidManifest.AddApplicationMetaDataElement("com.google.android.gms.ads.APPLICATION_ID", appId);
            
            // For Facebook
            context.NetworkSecurityConfig.SetCleartextTrafficPermitted(NetworkSecurityConfigSection.Domain, true);

            if (LLMaxSettings.Instance.EnabledAdapters.Contains("AdColonyAdapter"))
            {
                context.NetworkSecurityConfig.SetCleartextTrafficPermitted(NetworkSecurityConfigSection.Base, true);
            }
            
            context.NetworkSecurityConfig.AddDomain("127.0.0.1", true);
            // For AdColony and Smaato
            context.NetworkSecurityConfig.AddTrustedCertificates(NetworkSecurityConfigSection.Base, "system");
        }


        private void AddFileToEmbedFrameworks(IIosBuildPostprocessorContext context, string libPath)
        {
            var fileGuid = context.PbxProject.AddFile(libPath, libPath);
            var unityMainTargetGuid = context.PbxProject.GetUnityMainTargetGuid();
            context.PbxProject.AddFileToEmbedFrameworks(unityMainTargetGuid, fileGuid);
        }
    }
}