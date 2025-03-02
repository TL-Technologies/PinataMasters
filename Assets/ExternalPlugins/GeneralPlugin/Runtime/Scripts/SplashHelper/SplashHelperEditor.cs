#if UNITY_EDITOR
using Modules.Hive.Reflection;
using Modules.Hive.Utilities;
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Modules.General.Utilities
{
    public class SplashHelperEditor : ISplashHelper
    {
        private const string IosPlayerSettingsTypeString = "Modules.Hive.Editor.Reflection.PlayerSettingsHelper+Ios, Modules.Hive.Editor";
        private const string LaunchScreenTypePropertyName = "IphoneLaunchScreenType";
        
        private const string PlayerSettingsHelperTypeString = "Modules.Hive.Editor.Reflection.PlayerSettingsHelper, Modules.Hive.Editor";
        private const string GetPlatformSplashesMethodName = "GetPlatformSplashes";


        public Vector2 SplashSize { get; private set; }


        public Texture2D LoadSplash()
        {
            BuildTargetGroup targetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            if (targetGroup != BuildTargetGroup.iOS && targetGroup != BuildTargetGroup.Android)
            {
                throw new NotImplementedException("Splash loading logic implemented only for iOS and Android platforms!");
            }
            
            PlatformSplashOrientation orientation = Screen.width > Screen.height ?
                PlatformSplashOrientation.Landscape :
                PlatformSplashOrientation.Portrait;
            
            if (targetGroup == BuildTargetGroup.iOS)
            {
                Type iosPlayerSettingsType = Type.GetType(IosPlayerSettingsTypeString);
                iOSLaunchScreenType launchScreenType = ReflectionHelper.GetPropertyValue<iOSLaunchScreenType>(
                    iosPlayerSettingsType,
                    null,
                    LaunchScreenTypePropertyName);
                
                if (launchScreenType != iOSLaunchScreenType.ImageAndBackgroundConstant &&
                    launchScreenType != iOSLaunchScreenType.ImageAndBackgroundRelative)
                {
                    throw new NotImplementedException(
                        $"GetSplashData method does not support iosLaunchScreenType == {launchScreenType}!");
                }
            }

            Texture2D result = null;
            Texture2D universalResult = null;
            Type playerSettingHelperType = Type.GetType(PlayerSettingsHelperTypeString);
            Func<BuildTargetGroup, PlatformSplash[]> getPlatformSplashesDelegate = ReflectionHelper.CreateDelegateToMethod<Func<BuildTargetGroup, PlatformSplash[]>>(
                playerSettingHelperType,
                GetPlatformSplashesMethodName,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                true);
            foreach (PlatformSplash platformSplash in getPlatformSplashesDelegate(targetGroup))
            {
                if (platformSplash.Orientation == PlatformSplashOrientation.Universal && universalResult == null)
                {
                    universalResult = platformSplash.Texture;
                }
                if (platformSplash.Orientation == orientation)
                {
                    result = platformSplash.Texture;
                    break;
                }
            }
            
            if (result == null)
            {
                result = universalResult;
            }
            
            if (result != null)
            {
                // Additional texture setup
                result.hideFlags = HideFlags.DontSave;
                result.wrapMode = TextureWrapMode.Clamp;
                
                if (targetGroup == BuildTargetGroup.iOS)
                {
                    SplashSize = SplashHelperUtility.CalculateSplashSize(
                        new Vector2(result.width, result.height));
                }
                else if (targetGroup == BuildTargetGroup.Android)
                {
                    SplashSize = SplashHelperUtility.CalculateSplashSize(
                        new Vector2(result.width, result.height),
                        (int)PlayerSettings.Android.splashScreenScale);
                }
            }
            
            return result;
        }


        public void UnloadSplash(Texture2D texture) { }
    }
}
#endif
