using Modules.Hive.Utilities;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Reflection
{
    public static class PlayerSettingsHelper
    {
        private static SerializedObject serializedObject;
        private static bool batched = false;
        

        #region Editor properties
        
        private static SerializedObject SettingsSerializedObject
        {
            get
            {
                if (serializedObject == null)
                {
                    MethodInfo mi = typeof(PlayerSettings).GetMethod("GetSerializedObject", BindingFlags.NonPublic | BindingFlags.Static);
                    serializedObject = mi.Invoke(null, null) as SerializedObject;
                }
                return serializedObject;
            }
        }

        
        private static SerializedProperty GetProperty(string name)
        {
            SerializedObject obj = SettingsSerializedObject;
            if (obj == null)
            {
                Debug.LogErrorFormat("failed to find property '{0}' in PlayerSettings asset", name);
                return null;
            }

            if (!batched)
            {
                obj.Update();
            }

            return string.IsNullOrEmpty(name) ? obj.GetIterator() : obj.FindProperty(name);
        }

        
        private static void UpdateSerializedObject()
        {
            SerializedObject obj = SettingsSerializedObject;
            if (obj != null && !batched)
            {
                obj.Update();
            }
        }

        
        private static void ApplyChanges()
        {
            SerializedObject obj = SettingsSerializedObject;
            if (obj != null && !batched)
            {
                obj.ApplyModifiedProperties();
            }
        }

        
        public static bool GetBoolean(string name, bool defaultValue = false)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Boolean)
            {
                return property.boolValue;
            }

            return defaultValue;
        }

        
        public static void SetBoolean(string name, bool value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Boolean)
            {
                property.boolValue = value;
                ApplyChanges();
            }
        }

        
        public static int GetInt(string name, int defaultValue = 0)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Integer)
            {
                return property.intValue;
            }

            return defaultValue;
        }

        
        public static void SetInt(string name, int value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = value;
                ApplyChanges();
            }
        }

        
        public static int GetEnum(string name, int defaultValue)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Enum)
            {
                return property.enumValueIndex;
            }

            return defaultValue;
        }

        
        public static void SetEnum(string name, int value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Enum)
            {
                property.enumValueIndex = value;
                ApplyChanges();
            }
        }
        

        public static float GetFloat(string name, float defaultValue = 0f)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Float)
            {
                return property.floatValue;
            }

            return defaultValue;
        }

        
        public static void SetFloat(string name, float value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Float)
            {
                property.floatValue = value;
                ApplyChanges();
            }
        }

        
        public static Texture2D GetTexture2D(string name, Texture2D defaultValue = null)
        {
            Texture2D tex = null;
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                tex = property.objectReferenceValue as Texture2D;
            }

            return tex != null ? tex : defaultValue;
        }

        
        public static void SetTexture2D(string name, Texture2D value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                property.objectReferenceValue = value;
                ApplyChanges();
            }
        }

        
        public static Color GetColor(string name, Color defaultValue)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Color)
            {
                return property.colorValue;
            }

            return defaultValue;
        }

        
        public static void SetColor(string name, Color value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.Color)
            {
                property.colorValue = value;
                ApplyChanges();
            }
        }

        
        public static string GetString(string name, string defaultValue = null)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.String)
            {
                return property.stringValue;
            }

            return defaultValue;
        }

        
        public static void SetString(string name, string value)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.String)
            {
                property.stringValue = value;
                ApplyChanges();
            }
        }

        
        private static Object GetObject(string name)
        {
            SerializedProperty property = GetProperty(name);
            if (property != null && property.propertyType == SerializedPropertyType.ObjectReference)
            {
                return property.objectReferenceValue;
            }

            return null;
        }

        
        /// <summary>
        /// Turn to editor mode
        /// WARNING! Do not use PlayerSettings directly inside BeginEdit/EndEdit block!
        /// </summary>
        public static void BeginEdit()
        {
            UpdateSerializedObject();
            batched = true;
        }

        
        /// <summary>
        /// Apply all changes and close editor mode
        /// WARNING! Do not use PlayerSettings directly inside BeginEdit/EndEdit block!
        /// </summary>
        public static void EndEdit()
        {
            if (batched)
            {
                batched = false;
                ApplyChanges();
            }
        }
        
        
        // Works only for one-line properties
        private static string GetPropertyDirectlyFromFile(string propertyName)
        {
            ApplyChanges();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            string content;
            using (FileStream fileStream = new FileStream(UnityPath.ProjectSettingsPath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                content = streamReader.ReadToEnd();
            }
            
            return Regex.Match(content, $"(?<={propertyName}:).*").Value.Trim();
        }
        
        
        // Works only for one-line properties
        private static void SetPropertyDirectlyToFile(string propertyName, string propertyValue)
        {
            ApplyChanges();
            
            // It's necessary before modifying it as usual file
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            string content;
            using (FileStream fileStream = new FileStream(UnityPath.ProjectSettingsPath, FileMode.Open))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                content = streamReader.ReadToEnd();
            }
            
            content = Regex.Replace(
                content, 
                $"{propertyName}.*", 
                $"{propertyName}: {propertyValue}");
                        
            using (FileStream fileStream = new FileStream(UnityPath.ProjectSettingsPath, FileMode.Create))
            using (StreamWriter streamWriter = new StreamWriter(fileStream))
            {
                streamWriter.Write(content);
            }
            
            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            
            UpdateSerializedObject();
        }

        #endregion
        
        

        #region Log properties

        internal static void LogProperties(TextWriter writer)
        {
            MethodInfo mi = typeof(PlayerSettings).GetMethod("GetSerializedObject", BindingFlags.NonPublic | BindingFlags.Static);
            SerializedObject so = mi.Invoke(null, null) as SerializedObject;

            SerializedProperty iter = so.GetIterator();
            while (iter.NextVisible(true))
            {
                writer.WriteLine("{0}\t{1}", iter.type, iter.propertyPath);
            }
        }

        
        internal static string LogPropertiesToString()
        {
            StringBuilder sb = new StringBuilder();
            using (StringWriter sw = new StringWriter(sb))
            {
                LogProperties(sw);
            }

            return sb.ToString();
        }
        

        internal static void LogPropertiesToFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = UnityPath.Combine(Application.dataPath, "PlayerSettings.txt");
            }

            using (var sw = File.CreateText(path))
            {
                LogProperties(sw);
            }
        }

        
        // [MenuItem("Modules/Hive/PlayerSettings/Dump properties to file")]
        internal static void LogPropertiesToFile()
        {
            LogPropertiesToFile(null);
        }

        #endregion
        
        

        #region Platform splashes

        public static PlatformSplash[] GetPlatformSplashes(BuildTargetGroup platform)
        {
            PlatformSplash[] splashes = GetPlatformSplashesTemplates(platform);

            List<PlatformSplash> result = new List<PlatformSplash>(splashes.Length);
            foreach (PlatformSplash splash in splashes)
            {
                Texture2D texture = GetTexture2D(splash.SerializedPropertyName, Texture2D.blackTexture);
                if (texture != null)
                {
                    splash.Texture = texture;
                    result.Add(splash);
                }
            }

            return result.ToArray();
        }


        public static void SetPlatformSplashes(BuildTargetGroup platform, PlatformSplash[] platformSplashes)
        {
            PlatformSplash[] splashes = GetPlatformSplashesTemplates(platform);
            foreach (PlatformSplash splash in splashes)
            {
                // Make changes for only splashes that exists in templates collection
                if (platformSplashes.Any(p => p.SerializedPropertyName == splash.SerializedPropertyName))
                {
                    SetTexture2D(splash.SerializedPropertyName, splash.Texture);
                }
                else
                {
                    Debug.LogError($"The platform splash '{splash}' is not supported by build target '{platform}'.");
                }
            }
        }


        private static PlatformSplash[] GetPlatformSplashesTemplates(BuildTargetGroup platform)
        {
            switch (platform)
            {
                case BuildTargetGroup.iOS:
                    List<PlatformSplash> result = new List<PlatformSplash>()
                    {
                        new PlatformSplash("iOSLaunchScreenPortrait", "Default portrait", (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait),
                        new PlatformSplash("iOSLaunchScreenLandscape", "Default landscape", (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Landscape),
                            
                        new PlatformSplash("iOSLaunchScreeniPadImage", "Default universal", (int)iOSTargetDevice.iPadOnly, PlatformSplashOrientation.Universal)
                    };
                    
                    #if UNITY_2019_3
                        iOSLaunchScreenType launchScreenType = Ios.IphoneLaunchScreenType;
                        if (launchScreenType != iOSLaunchScreenType.ImageAndBackgroundRelative &&
                            launchScreenType != iOSLaunchScreenType.ImageAndBackgroundConstant)
                        {
                            result.Clear();
                            
                            // iPhones
                            result.Add(new PlatformSplash("iPhoneSplashScreen", "iPhone 3,5\"", 320, 480, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhoneHighResSplashScreen", "iPhone 3,5\" 2x", 640, 960, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhoneTallHighResSplashScreen", "iPhone 4,0\"", 640, 1136, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhone47inSplashScreen", "iPhone 4,7\"", 750, 1334, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhone55inPortraitSplashScreen", "iPhone 5,5\"", 1242, 2208, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhone55inLandscapeSplashScreen", "iPhone 5,5\"", 2208, 1242, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Landscape));
                            result.Add(new PlatformSplash("iPhone58inPortraitSplashScreen", "iPhone 5,8\"", 1125, 2436, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPhone58inLandscapeSplashScreen", "iPhone 5,8\"", 2436, 1125, (int)iOSTargetDevice.iPhoneOnly, PlatformSplashOrientation.Landscape));
                            
                            // iPads
                            result.Add(new PlatformSplash("iPadPortraitSplashScreen", "iPad", 768, 1024, (int)iOSTargetDevice.iPadOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPadLandscapeSplashScreen", "iPad", 1024, 768, (int)iOSTargetDevice.iPadOnly, PlatformSplashOrientation.Landscape));
                            result.Add(new PlatformSplash("iPadHighResPortraitSplashScreen", "iPad", 1536, 2048, (int)iOSTargetDevice.iPadOnly, PlatformSplashOrientation.Portrait));
                            result.Add(new PlatformSplash("iPadHighResLandscapeSplashScreen", "iPad", 2048, 1536, (int)iOSTargetDevice.iPadOnly, PlatformSplashOrientation.Landscape));
                        }
                    #endif
                    
                    return result.ToArray();

                case BuildTargetGroup.Android:
                    return new[]
                    {
                        new PlatformSplash("androidSplashScreen", "Default splash", 0)
                    };

                default:
                    Debug.LogError($"Failed to get platform splashes. This feature is not implemented for build target '{platform}'.");
                    return null;
            }
        }

        #endregion


        
        #region iOS Specific

        /// <summary>
        /// iOS specific part
        /// </summary>
        public sealed class Ios
        {
            public static iOSLaunchScreenType IphoneLaunchScreenType
            {
                get => (iOSLaunchScreenType)GetEnum("iOSLaunchScreenType", (int)iOSLaunchScreenType.Default);
                set => SetEnum("iOSLaunchScreenType", (int)value);
            }
            

            public static Texture2D IphoneLaunchScreenPortrait
            {
                get => GetTexture2D("iOSLaunchScreenPortrait");
                set => SetTexture2D("iOSLaunchScreenPortrait", value);
            }
            

            public static Texture2D IphoneLaunchScreenLandscape
            {
                get => GetTexture2D("iOSLaunchScreenLandscape");
                set => SetTexture2D("iOSLaunchScreenLandscape", value);
            }
            

            public static Color IphoneLaunchScreenBackgroundColor
            {
                get => GetColor("iOSLaunchScreenBackgroundColor", Color.white);
                set => SetColor("iOSLaunchScreenBackgroundColor", value);
            }
            

            public static float IphoneLaunchScreenFillPercent
            {
                get => GetFloat("iOSLaunchScreenFillPct");
                set => SetFloat("iOSLaunchScreenFillPct", value);
            }
            

            public static float IphoneLaunchScreenSize
            {
                get => GetFloat("iOSLaunchScreenSize");
                set => SetFloat("iOSLaunchScreenSize", value);
            }
            

            public static string IphoneLaunchScreenCustomXibPath
            {
                get => GetString("iOSLaunchScreenCustomXibPath");
                set => SetString("iOSLaunchScreenCustomXibPath", value);
            }
            

            public static iOSLaunchScreenType IpadLaunchScreenType
            {
                get => (iOSLaunchScreenType)GetEnum("iOSLaunchScreeniPadType", (int)iOSLaunchScreenType.Default);
                set => SetEnum("iOSLaunchScreeniPadType", (int)value);
            }
            

            public static Texture2D IpadLaunchScreenImage
            {
                get => GetTexture2D("iOSLaunchScreeniPadImage");
                set => SetTexture2D("iOSLaunchScreeniPadImage", value);
            }
            

            public static Color IpadLaunchScreenBackgroundColor
            {
                get => GetColor("iOSLaunchScreeniPadBackgroundColor", Color.white);
                set => SetColor("iOSLaunchScreeniPadBackgroundColor", value);
            }
            

            public static float IpadLaunchScreenFillPercent
            {
                get => GetFloat("iOSLaunchScreeniPadFillPct");
                set => SetFloat("iOSLaunchScreeniPadFillPct", value);
            }
            

            public static float IpadLaunchScreenSize
            {
                get => GetFloat("iOSLaunchScreeniPadSize");
                set => SetFloat("iOSLaunchScreeniPadSize", value);
            }
            

            public static string IpadLaunchScreenCustomXibPath
            {
                get => GetString("iOSLaunchScreeniPadCustomXibPath");
                set => SetString("iOSLaunchScreeniPadCustomXibPath", value);
            }
        }

        #endregion
        
        
        

        #region Android Specific

        /// <summary>
        /// Android specific part
        /// </summary>
        public sealed class Android
        {
            public static bool IsBannerEnabled
            {
                get => GetBoolean("androidEnableBanner");
                set => SetBoolean("androidEnableBanner", value);
            }
            
            
            public static int TargetSdkVersion
            {
                // We can't use methods GetEnum/SetEnum, because it will not bypass
                // "value is in enum range" check
                // (in Unity 2019 maximum allowed value for AndroidTargetSdkVersion enum is 28)
                get => int.Parse(GetPropertyDirectlyFromFile("AndroidTargetSdkVersion"));
                set => SetPropertyDirectlyToFile("AndroidTargetSdkVersion", value.ToString());
            }


            public static AndroidBanner[] GetBanners()
            {
                SerializedProperty property = GetProperty("m_AndroidBanners");
                if (property == null || !property.isArray)
                {
                    return new AndroidBanner[0];
                }

                int count = property.arraySize;
                AndroidBanner[] banners = new AndroidBanner[count];

                for (int i = 0; i < count; i++)
                {
                    SerializedProperty bannerData = property.GetArrayElementAtIndex(i);
                    banners[i] = new AndroidBanner(
                        bannerData.FindPropertyRelative("width").intValue,
                        bannerData.FindPropertyRelative("height").intValue,
                        bannerData.FindPropertyRelative("banner").objectReferenceValue as Texture2D);
                }

                return banners;
            }


            public static void SetBanners(AndroidBanner[] banners)
            {
                if (banners == null || banners.Length == 0)
                {
                    return;
                }

                SerializedProperty property = GetProperty("m_AndroidBanners");
                if (property == null || !property.isArray)
                {
                    Debug.LogError("Failed to set android banners. The field 'm_AndroidBanners' not found in PlayerSettings.");
                    return;
                }

                int count = property.arraySize;
                for (int i = 0; i < count; i++)
                {
                    SerializedProperty bannerData = property.GetArrayElementAtIndex(i);
                    int width = bannerData.FindPropertyRelative("width").intValue;
                    int height = bannerData.FindPropertyRelative("height").intValue;

                    AndroidBanner banner = banners.FirstOrDefault(p => p.Width == width && p.Height == height);
                    if (banner != null)
                    {
                        bannerData.FindPropertyRelative("banner").objectReferenceValue = banner.Texture;
                    }
                    else
                    {
                        Debug.LogError($"Failed to set android banner. The corresponding field not found in PlayerSettings.");
                    }
                }

                ApplyChanges();
            }
        }

        #endregion
    }
}
