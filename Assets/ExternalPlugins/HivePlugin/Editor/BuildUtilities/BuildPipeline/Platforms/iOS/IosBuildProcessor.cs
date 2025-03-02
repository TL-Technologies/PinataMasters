using Modules.Hive.Editor.Pipeline;
using System;
using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


namespace Modules.Hive.Editor.BuildUtilities
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.Internal)]
    internal class IosBuildProcessor :
        IBuildPreprocessor<IIosBuildPreprocessorContext>,
        IBuildPostprocessor<IIosBuildPostprocessorContext>
    {
        public void OnPreprocessBuild(IIosBuildPreprocessorContext context)
        {
            // Amend application icons, splashes and other build images to avoid blur effect
            BuildImagesUtilities.AmendAllIconTexturesInPlayerSettings(context.BuildTargetGroup);
            BuildImagesUtilities.AmendAllSplashTexturesInPlayerSettings(context.BuildTargetGroup);
            
            // Override settings according custom build settings
            if (context.BuildSettings.TryGetValue(BuildSetting.BuildNumber, out string buildNumber))
            {
                PlayerSettings.iOS.buildNumber = buildNumber;
            }
        }


        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            string unityPath = HivePluginHierarchy.Instance.GetPlatformPath(PlatformAlias.Ios);
            string xcodePath = context.GetDestinationPath("Hive");

            // Extract and add to xcode
            FileSystemUtilities.ExtractArchive(UnityPath.Combine(unityPath, "HiveFoundation.zip"), xcodePath);
            context.PbxProject.AddItemsRecursively(xcodePath);

            // Remove UIApplicationExitsOnSuspend deprecated key from Info.plist
            // https://apple.co/2wFqoWM
            context.InfoPlist.Remove("UIApplicationExitsOnSuspend");

            context.InfoPlist.AppUsesNonExemptEncryption = false;

            // Temporary disable bitcode
            context.PbxProject.SetBuildProperty("ENABLE_BITCODE", "NO");
            
            // Fix Unity (or Facebook?) bug - gettin ITMS-90206 when trying to upload build to TestFlight
            // https://forum.unity.com/threads/an-empty-folder-named-frameworks-in-unityframework-framework.751559/
            context.PbxProject.SetBuildProperty("ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            
            #if UNITY_2019_4_OR_NEWER
                // Workaround for Unity 2019.4 bug
                // Return value 2 means Universal architecture, which should NOT include
                // arm64 string to required device capabilities in Info.plist
                if (PlayerSettings.GetArchitecture(context.BuildTargetGroup) == 2)
                {
                    const string requiredDeviceCapabilitiesKey = "UIRequiredDeviceCapabilities";
                    const string arm64CapabilityKey = "arm64";
                    
                    PlistElementArray requiredCapabilities = context.InfoPlist.GetArray(requiredDeviceCapabilitiesKey);
                    requiredCapabilities.values.RemoveAll(element => 
                        element.AsString().Equals(arm64CapabilityKey, StringComparison.InvariantCulture));
                    context.InfoPlist.SetArray(requiredDeviceCapabilitiesKey, requiredCapabilities);
                }
            #endif
            
            ForceModernXcodeBuildSystem(context.BuildPath);

            // CFBundleLocalizations
            // if (!string.IsNullOrEmpty(BundleLocalizations))
            // {
            //     string[] locales = BundleLocalizations.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
            //     var bundleLocalizations = args.InfoPList.GetArray("CFBundleLocalizations", true);
            //     foreach (var locale in locales)
            //     {
            //         bundleLocalizations.AddString(locale);
            //     }
            // }

            // Reset aps-environment value to avoid archive build issues
            context.Entitlements.ApsEnvironment = null;
            
            
            void ForceModernXcodeBuildSystem(string buildPath)
            {
                const string buildSystemTypeKey = "BuildSystemType";
                foreach (FileInfo fileInfo in FileSystemUtilities.EnumerateFiles(
                    buildPath,
                    "*.xcsettings",
                    SearchOption.AllDirectories))
                {
                    PlistDocument projectSettings = new PlistDocument();
                    projectSettings.ReadFromFile(fileInfo.FullName);
            
                    if (projectSettings.root.values.Remove(buildSystemTypeKey))
                    {
                        projectSettings.WriteToFile(fileInfo.FullName);
                    }
                }
            }
        }
    }
}
