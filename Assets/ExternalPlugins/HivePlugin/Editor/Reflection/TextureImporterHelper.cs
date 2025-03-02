using Modules.Hive.Reflection;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Reflection
{
    public static class TextureImporterHelper
    {
        private delegate void GetWidthAndHeightDelegate(TextureImporter importer, ref int width, ref int height);
        private static GetWidthAndHeightDelegate getWidthAndHeightDelegate;
        private static BuildPlatform[] availablePlatforms = null;

        /// <summary>
        /// Gets original texture size in pixels.
        /// </summary>
        /// <param name="importer">The texture importer that determine a texture to get original width and hegiht.</param>
        /// <param name="width">When this method returns, contains the value of the texture width.</param>
        /// <param name="height">When this method returns, contains the value of the texture height.</param>
        public static void GetOriginalTextureSize(TextureImporter importer, out int width, out int height)
        {
            if (importer == null)
            {
                throw new ArgumentNullException(nameof(importer));
            }

            if (getWidthAndHeightDelegate == null)
            {
                getWidthAndHeightDelegate = ReflectionHelper.CreateDelegateToMethod<GetWidthAndHeightDelegate>(
                    typeof(TextureImporter), "GetWidthAndHeight",
                    BindingFlags.NonPublic | BindingFlags.Instance, false);
            }

            width = 0;
            height = 0;
            getWidthAndHeightDelegate(importer, ref width, ref height);
        }


        /// <summary>
        /// Gets original texture size in pixels.
        /// </summary>
        /// <param name="assetPath">The path to a texture to get original width and hegiht.</param>
        /// <param name="width">When this method returns, contains the value of the texture width.</param>
        /// <param name="height">When this method returns, contains the value of the texture height.</param>
        public static void GetOriginalTextureSize(string assetPath, out int width, out int height)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null)
            {
                throw new Exception($"Failed to get Texture importer for asset at '{assetPath}'.");
            }

            GetOriginalTextureSize(importer, out width, out height);
        }


        /// <summary>
        /// Gets original texture size in pixels.
        /// </summary>
        /// <param name="texture">The texture to get original width and hegiht.</param>
        /// <param name="width">When this method returns, contains the value of the texture width.</param>
        /// <param name="height">When this method returns, contains the value of the texture height.</param>
        public static void GetOriginalTextureSize(Texture2D texture, out int width, out int height)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            var path = AssetDatabase.GetAssetPath(texture);
            GetOriginalTextureSize(path, out width, out height);
        }


        /// <summary>
        /// Gets all available platforms that provide the ability to override texture importer settings.
        /// </summary>
        public static BuildPlatform[] AvailablePlatforms
        {
            get
            {
                if (availablePlatforms == null)
                {
                    object[] buildPlatforms = new object[] { };
                    #if UNITY_2020_3_OR_NEWER
                        Type textureImporterInspectorType = Type.GetType("UnityEditor.BaseTextureImportPlatformSettings,UnityEditor.dll");
                        MethodInfo getBuildPlayerValidPlatformsMethod = textureImporterInspectorType.GetMethod("GetBuildPlayerValidPlatforms", BindingFlags.Static | BindingFlags.Public);
                        buildPlatforms = getBuildPlayerValidPlatformsMethod.Invoke(null, null) as object[];
                    #else
                        Type textureImporterInspectorType = Type.GetType("UnityEditor.TextureImporterInspector,UnityEditor.dll");
                        MethodInfo getBuildPlayerValidPlatformsMethod = textureImporterInspectorType.GetMethod("GetBuildPlayerValidPlatforms", BindingFlags.Static | BindingFlags.Public);
                        buildPlatforms = getBuildPlayerValidPlatformsMethod.Invoke(null, null) as object[];
                    #endif
                    Type buildPlatformType = Type.GetType("UnityEditor.Build.BuildPlatform,UnityEditor.dll");
                    FieldInfo buildPlatformNameField = buildPlatformType.GetField("name", BindingFlags.Instance | BindingFlags.Public);
                    FieldInfo buildPlatformTooltipField = buildPlatformType.GetField("tooltip", BindingFlags.Instance | BindingFlags.Public);
                    var buildPlatformSmallIconMember =
                    #if UNITY_2019_3_OR_NEWER
                        buildPlatformType.GetProperty("smallIcon", BindingFlags.Instance | BindingFlags.Public);
                    #else
                        buildPlatformType.GetField("smallIcon", BindingFlags.Instance | BindingFlags.Public);
                    #endif
                    FieldInfo buildPlatformTargetGroupField = buildPlatformType.GetField("targetGroup", BindingFlags.Instance | BindingFlags.Public);

                    availablePlatforms = buildPlatforms
                        .Select(p => new BuildPlatform
                        {
                            name = buildPlatformNameField.GetValue(p) as string,
                            tooltip = buildPlatformTooltipField.GetValue(p) as string,
                            smallIcon = buildPlatformSmallIconMember.GetValue(p) as Texture2D,
                            buildTargetGroup = (BuildTargetGroup)buildPlatformTargetGroupField.GetValue(p)
                        })
                        .ToArray();
                }

                return availablePlatforms;
            }
        }


        /// <summary>
        /// Returns current platform that provide the ability to override texture importer settings.
        /// </summary>
        /// <returns></returns>
        public static BuildPlatform GetCurrentPlatform() => 
            AvailablePlatforms.FirstOrDefault(
                p => p.buildTargetGroup == BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget));


        /// <summary>
        /// Gets platform specific texture settings for current build platform.
        /// </summary>
        /// <param name="importer">The texture importer to interact.</param>
        /// <returns></returns>
        public static TextureImporterPlatformSettings GetCurrentPlatformTextureSettings(TextureImporter importer)
        {
            var platform = GetCurrentPlatform();
            if (string.IsNullOrEmpty(platform.name))
            {
                return importer.GetDefaultPlatformTextureSettings();
            }

            return importer.GetPlatformTextureSettings(platform.name);
        }


        /// <summary>
        /// Gets platform specific texture settings for current build platform if it overrides default
        /// ("Override" option in inspector is checked for the platform).
        /// Otherwise, gets default platform texture settings.
        /// </summary>
        /// <param name="importer">The texture importer to interact.</param>
        /// <returns></returns>
        public static TextureImporterPlatformSettings GetActualPlatformTextureSettings(TextureImporter importer)
        {
            var platform = GetCurrentPlatform();
            if (!string.IsNullOrEmpty(platform.name))
            {
                var platformTextureSettings = importer.GetPlatformTextureSettings(platform.name);
                if (platformTextureSettings.overridden)
                {
                    return platformTextureSettings;
                }
            }

            return importer.GetDefaultPlatformTextureSettings();
        }
    }
}
