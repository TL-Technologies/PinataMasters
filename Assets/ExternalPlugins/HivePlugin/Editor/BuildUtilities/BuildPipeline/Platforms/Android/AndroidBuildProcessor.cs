using Modules.Hive.Editor.BuildUtilities.Android;
using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Editor.Reflection;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.Internal)]
    internal class AndroidBuildProcessor : 
        IBuildPreprocessor<IAndroidBuildPreprocessorContext>,
        IGradleBuildPreprocessor<IGradleBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            // Add default repositories
            context.GradleScript.AddRepository(GradleRepositoryPlacement.BuildScript, GradleRepository.Google, GradleRepository.JCenter);
            context.GradleScript.AddRepository(GradleRepositoryPlacement.AllProjects, GradleRepository.Google, GradleRepository.JCenter);
            
            //Add unity version dependant gradle modifications
            #if UNITY_2020_1_OR_NEWER
                //Add aatp option 
                context.GradleScript.AddAaptOption(new GradleAaptOption("noCompress", "['.ress', '.resource', '.obb'] + unityStreamingAssets.tokenize(', ')"));
            #elif UNITY_2019_4_OR_NEWER 
                //Unity 2019 requires concrete gradle version usage
                context.GradleScript.AddDependency(GradleDependencyPlacement.BuildScript, new GradleDependency("com.android.tools.build:gradle:3.4.3", GradleDependencyType.ClassPath));
            #endif
            

            // Add dependencies
            context.GradleScript.AddDependency(
                new AndroidXDependency("androidx.appcompat:appcompat"),
                new GradleDependency("com.google.code.gson:gson:2.8.5"));

            // Disable split screen feature.
            context.AndroidManifest.ResizeableActivity = false;
            
            // Disable auto backup, because it's not really necessary in our apps and
            // produces unexpected behavior in case of app reinstall (app preferences might not be cleared)
            context.IsAllowBackup = false;
            
            // Maximum aspect ratio of screen.
            // Value 2.4 (12:5) recommended by Google https://developer.android.com/guide/practices/screens-distribution
            context.AndroidManifest.MaxAspectRatio = 2.4f;

            // Add meta-data tag to avoid issues with icons on Samsung devices
            context.AndroidManifest.AddApplicationMetaDataElement("com.samsung.android.icon_container.has_icon_container", "true");
            
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.ReadPhoneState);
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.ReadExternalStorage);
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.WriteExternalStorage);
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.AccessFineLocation);
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.AccessCoarseLocation);
            context.AndroidManifest.ForceRemovingPermissionElement(Permission.QueryAllPackages);

            // Disable Android TV compatibility
            PlayerSettings.Android.androidTVCompatibility = false;
            PlayerSettingsHelper.Android.IsBannerEnabled = false;
            
            // Force target sdk version as required here
            // https://developer.android.com/distribute/best-practices/develop/target-sdk.html
            PlayerSettingsHelper.Android.TargetSdkVersion = 30;

            // Amend application icons, splashes and other build images to avoid blur effect
            BuildImagesUtilities.AmendAllIconTexturesInPlayerSettings(context.BuildTargetGroup);
            BuildImagesUtilities.AmendAllSplashTexturesInPlayerSettings(context.BuildTargetGroup);
            BuildImagesUtilities.AmendAllAndroidBannerTexturesInPlayerSettings();

            // Disable minification (this feature is not supported by a number of plugins)
            #if UNITY_2020_1_OR_NEWER
                PlayerSettings.Android.minifyDebug = false;
                PlayerSettings.Android.minifyRelease = false;
                PlayerSettings.Android.minifyWithR8 = false;
            #else
                EditorUserBuildSettings.androidDebugMinification = AndroidMinification.None;
                EditorUserBuildSettings.androidReleaseMinification = AndroidMinification.None;
            #endif
            
            // Override setting in CI build or apply build settings configured in unity otherwise 
            if (context.IsCiBuild)
            {
                if (!context.IsDevelopmentBuild)
                {
                    PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);
                    PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
                    // Next method doesn't work on Android platform
                    // PlayerSettings.SetArchitecture(BuildTargetGroup.Android, (int)(AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64));
                }
                
                // Override settings according custom build settings
                if (context.BuildSettings.TryGetValue(BuildSetting.BuildNumber, out string buildNumber))
                {
                    PlayerSettings.Android.bundleVersionCode = int.Parse(buildNumber);
                }
                
                EditorUserBuildSettings.androidCreateSymbolsZip = context.BuildSettings.ContainsKey(BuildSetting.SymbolsFileNecessity);
            }
            
            UnpackHiveFoundationLibrary(context);
        }
        
        
        public void OnPreprocessGradleBuild(IGradleBuildPreprocessorContext context)
        {
            // Should be used until Unity adds this properties into its template
            context.GradleProperties.IsAndroidXEnabled = true;
            context.GradleProperties.IsJetifierEnabled = true;
        }
        
        
        /// We need to move androidlib from package to Assets/Plugins/Android
        /// but Packages is immutable folder to Unity. This causes error because of
        /// duplicate .meta files which cannot be resolved by Unity. So
        /// the solution was to store library in zip archive
        private static void UnpackHiveFoundationLibrary(IAndroidBuildPreprocessorContext context)
        {
            string source = HivePluginHierarchy.Instance.GetPlatformAssetPath(PlatformAlias.Android);
            string pathToHiveLibrary = UnityPath.Combine(source, "HiveFoundation.zip");
            pathToHiveLibrary = UnityPath.GetFullPathFromAssetPath(pathToHiveLibrary);
            string hiveLibraryDestination = UnityPath.AndroidPluginsAssetPath;

            FileSystemUtilities.ExtractArchive(pathToHiveLibrary, hiveLibraryDestination, false);

            UnityPluginUtilities.EnablePluginAsset(
                UnityPath.Combine(UnityPath.AndroidPluginsAssetPath, "HiveFoundation.androidlib"),
                context.BuildTarget);
        }
    }
}
