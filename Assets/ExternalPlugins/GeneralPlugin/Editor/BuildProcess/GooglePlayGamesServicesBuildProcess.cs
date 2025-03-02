using Modules.General.Abstraction.GooglePlayGameServices;
using Modules.General.GooglePlayGameServices;
using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Android;
using System;


namespace Modules.General.Editor.Android
{
    internal class GooglePlayGamesServicesBuildProcess : IBuildPreprocessor<IGooglePlayBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IGooglePlayBuildPreprocessorContext context)
        {
            IGpgsSettings settings = LLGPGSSettings.DoesInstanceExist ? LLGPGSSettings.Instance : null;
            if (settings == null || !settings.IsGpgsEnabled)
            {
                return;
            }
            
            string gpgsAppId = settings.AppId;
            if (string.IsNullOrWhiteSpace(gpgsAppId))
            {
                throw new Exception("The value of 'App ID' in Google Play Games Services is null or empty.");
            }

            context.AndroidManifest.AddApplicationMetaDataElement(
                "com.google.android.gms.games.APP_ID", 
                gpgsAppId);
            context.AndroidManifest.AddApplicationMetaDataElement(
                "com.google.android.gms.version", 
                "@integer/google_play_services_version");
            context.AndroidManifest.AddPermissionElement(Permission.Internet);
            context.AndroidManifest.AddPermissionElement(Permission.AccessNetworkState);

            context.GradleScript.AddDependency(
                new AndroidXDependency("androidx.fragment:fragment"),
                new GoogleMobileServicesDependency("play-services-auth"), // Does it really matter?????
                new GoogleMobileServicesDependency("play-services-games"));
            
            // Enable aar with native code
            string androidPlatformAssetPath = GeneralPluginHierarchy.Instance.GetPlatformAssetPath(PlatformAlias.GooglePlay);
            UnityPluginUtilities.EnablePluginAsset(
                UnityPath.Combine(androidPlatformAssetPath, "LLSocialGPGS.aar"),
                context.BuildTarget);
        }
    }
}
