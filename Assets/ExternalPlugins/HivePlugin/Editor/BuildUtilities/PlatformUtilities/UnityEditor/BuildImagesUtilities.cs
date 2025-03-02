using Modules.Hive.Editor.Reflection;
using Modules.Hive.Utilities;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    public static class BuildImagesUtilities
    {
        #region Application icons

        /// <summary>
        /// Amends the texture for using as the application icon.
        /// </summary>
        /// <param name="texture">The texture to change and reimport.</param>
        /// <param name="textureType">The texture type value to apply.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool AmendIconTexture(Texture2D texture, TextureImporterType textureType = TextureImporterType.Default)
        {
            TextureImporter importer = AssetDatabaseUtilities.GetAssetImporter<TextureImporter>(texture);
            return AmendIconTexture(importer, textureType);
        }


        /// <summary>
        /// Amends the texture specified by texture importer for using as the application icon.
        /// </summary>
        /// <param name="textureImporter">The texture importer to work with.</param>
        /// <param name="textureType">The texture type value to apply.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool AmendIconTexture(TextureImporter textureImporter, TextureImporterType textureType = TextureImporterType.Default)
        {
            return AmendTextureImporter(textureImporter, textureType);
        }


        /// <summary>
        /// Amends all icon textures in player settings for specified build target group.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group.</param>
        public static void AmendAllIconTexturesInPlayerSettings(BuildTargetGroup buildTargetGroup)
        {
            var iconTextures = PlayerSettings.GetSupportedIconKindsForPlatform(buildTargetGroup)
                .SelectMany(kind => PlayerSettings.GetPlatformIcons(buildTargetGroup, kind))
                .SelectMany(icon => icon.GetTextures());

            foreach (Texture2D texture in iconTextures)
            {
                AmendIconTexture(texture);
            }
        }

        #endregion



        #region Application splashes

        /// <summary>
        /// Amends the texture for using as the application splash.
        /// </summary>
        /// <param name="texture">The texture to change and reimport.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool AmendSplashTexture(Texture2D texture)
        {
            TextureImporter importer = AssetDatabaseUtilities.GetAssetImporter<TextureImporter>(texture);
            return PrepareSplashTexture(importer);
        }


        /// <summary>
        /// Amends the texture specified by texture importer for using as the application splash.
        /// </summary>
        /// <param name="textureImporter">The texture importer to work with.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool PrepareSplashTexture(TextureImporter textureImporter)
        {
            return AmendTextureImporter(textureImporter, TextureImporterType.Default);
        }


        /// <summary>
        /// Amends all splash textures in player settings for specified build target group.
        /// </summary>
        /// <param name="buildTargetGroup">The build target group.</param>
        public static void AmendAllSplashTexturesInPlayerSettings(BuildTargetGroup buildTargetGroup)
        {
            foreach (PlatformSplash splash in PlayerSettingsHelper.GetPlatformSplashes(buildTargetGroup))
            {
                if (splash.Texture != null)
                {
                    AmendSplashTexture(splash.Texture);
                }
            }
        }

        #endregion



        #region Android banner

        /// <summary>
        /// Amends the texture for using as android banner.
        /// </summary>
        /// <param name="texture">The texture to change and reimport.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool AmendAndroidBannerTexture(Texture2D texture)
        {
            TextureImporter importer = AssetDatabaseUtilities.GetAssetImporter<TextureImporter>(texture);
            return AmendAndroidBannerTexture(importer);
        }


        /// <summary>
        /// Amends the texture specified by texture importer for using as android banner.
        /// </summary>
        /// <param name="textureImporter">The texture importer to work with.</param>
        /// <returns>True if the texture has been changed; otherwise, false.</returns>
        public static bool AmendAndroidBannerTexture(TextureImporter textureImporter)
        {
            return AmendTextureImporter(textureImporter, TextureImporterType.Default);
        }
        

        /// <summary>
        /// Amends all android banner textures in player settings.
        /// </summary>
        public static void AmendAllAndroidBannerTexturesInPlayerSettings()
        {
            foreach (AndroidBanner banner in PlayerSettingsHelper.Android.GetBanners())
            {
                if (banner.Texture != null)
                {
                    AmendAndroidBannerTexture(banner.Texture);
                }
            }
        }

        #endregion
        

        private static bool AmendTextureImporter(TextureImporter textureImporter, TextureImporterType? textureType = null)
        {
            if (textureImporter == null)
            {
                return false;
            }

            bool changed = false;

            if (textureType.HasValue && textureImporter.textureType != textureType)
            {
                textureImporter.textureType = textureType.Value;
                changed = true;
            }

            if (textureImporter.textureShape != TextureImporterShape.Texture2D)
            {
                textureImporter.textureShape = TextureImporterShape.Texture2D;
                changed = true;
            }

            if (textureImporter.spriteImportMode != SpriteImportMode.None)
            {
                textureImporter.spriteImportMode = SpriteImportMode.None;
                changed = true;
            }

            if (!textureImporter.sRGBTexture)
            {
                textureImporter.sRGBTexture = true;
                changed = true;
            }

            if (textureImporter.alphaSource != TextureImporterAlphaSource.FromInput)
            {
                textureImporter.alphaSource = TextureImporterAlphaSource.FromInput;
                changed = true;
            }

            if (!textureImporter.alphaIsTransparency)
            {
                textureImporter.alphaIsTransparency = true;
                changed = true;
            }

            if (textureImporter.npotScale != TextureImporterNPOTScale.None)
            {
                textureImporter.npotScale = TextureImporterNPOTScale.None;
                changed = true;
            }

            if (textureImporter.mipmapEnabled)
            {
                textureImporter.mipmapEnabled = false;
                changed = true;
            }

            if (textureImporter.maxTextureSize != 8192)
            {
                textureImporter.maxTextureSize = 8192;
                changed = true;
            }

            if (textureImporter.textureCompression != TextureImporterCompression.Uncompressed)
            {
                textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
                changed = true;
            }

            if (changed)
            {
                textureImporter.SaveAndReimport();
            }

            return true;
        }
    }
}
