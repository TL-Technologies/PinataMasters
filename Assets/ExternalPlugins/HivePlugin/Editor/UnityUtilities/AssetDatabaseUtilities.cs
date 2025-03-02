using Modules.Hive.Editor.Reflection;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor
{
    public class AssetDatabaseUtilities
    {
        /// <summary>
        /// Returns an array of scenes in current project.
        /// Each item is a GUID of scene asset.
        /// </summary>
        /// <returns></returns>
        public static string[] GetAllScenesGuids()
        {
            return AssetDatabase.FindAssets("t:scene");
        }


        /// <summary>
        /// Returns asset importer for asset object 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="assetObject"></param>
        /// <returns></returns>
        public static T GetAssetImporter<T>(UnityEngine.Object assetObject) where T : AssetImporter
        {
            string path = AssetDatabase.GetAssetPath(assetObject);
            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            return AssetImporter.GetAtPath(path) as T;
        }


        public static byte[] EncodeTextureToPng(TextureImporter importer)
        {
            var platformTextureSettings = TextureImporterHelper.GetActualPlatformTextureSettings(importer);
            bool needToReimport = false;

            // Precache values
            var cachedIsReadable = importer.isReadable;
            var cachedPixelFormat = platformTextureSettings.format;
            var cachedTextureCompression = platformTextureSettings.textureCompression;

            // Change importer settings 
            if (importer.isReadable != true)
            {
                importer.isReadable = true;
                needToReimport = true;
            }

            if (platformTextureSettings.format != TextureImporterFormat.RGBA32)
            {
                // Need to avoid exceptions on macos
                platformTextureSettings.format = TextureImporterFormat.RGBA32;
                needToReimport = true;
            }

            if (platformTextureSettings.textureCompression != TextureImporterCompression.Uncompressed)
            {
                // Need to avoid exceptions on macos
                platformTextureSettings.textureCompression = TextureImporterCompression.Uncompressed;
                needToReimport = true;
            }

            // Reimport teture if required
            if (needToReimport)
            {
                importer.SetPlatformTextureSettings(platformTextureSettings);
                importer.SaveAndReimport();
            }

            // Export rexture data
            byte[] bytes = null;
            try
            {
                Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(importer.assetPath);
                bytes = texture.EncodeToPNG();
            }
            finally
            {
                // Restore importer settings if required
                if (needToReimport)
                {
                    importer.isReadable = cachedIsReadable;

                    platformTextureSettings.format = cachedPixelFormat;
                    platformTextureSettings.textureCompression = cachedTextureCompression;
                    importer.SetPlatformTextureSettings(platformTextureSettings);
                    importer.SaveAndReimport();
                }
            }

            return bytes;
        }
    }
}
