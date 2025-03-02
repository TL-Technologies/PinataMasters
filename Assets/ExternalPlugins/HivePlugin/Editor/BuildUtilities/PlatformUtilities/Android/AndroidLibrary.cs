using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class AndroidLibrary : IDisposable
    {
        private CompositeDisposable disposables;


        public string OutputPath { get; private set; }

        /// <summary>
        /// Gets a manifest of the android library.
        /// </summary>
        public AndroidManifest AndroidManifest { get; private set; }


        #region Instancing

        /// <summary>
        /// Creates a new android library at specified path.
        /// if directory at path is already exist it will be overriden.
        /// </summary>
        /// <param name="path">A path to store the new android library.</param>
        /// <param name="package">A package name of the new android library.</param>
        public static AndroidLibrary Create(string path, string package = null)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            return new AndroidLibrary(path, package, true);
        }


        /// <summary>
        /// Opens an android library at specified path.
        /// </summary>
        /// <param name="path">A path to the android library to open.</param>
        /// <returns></returns>
        public static AndroidLibrary Open(string path)
        {
            return new AndroidLibrary(path, null, false);
        }


        private AndroidLibrary(string path, string package, bool createIfNotExists)
        {
            OutputPath = path;

            // Library folder
            if (!Directory.Exists(path))
            {
                if (!createIfNotExists)
                {
                    throw new ArgumentException($"Cannot find Android library at path: '{path}'", nameof(path));
                }

                Directory.CreateDirectory(path);
            }

            // project.properties
            string file = UnityPath.Combine(path, "project.properties");
            if (!File.Exists(file))
            {
                if (!createIfNotExists)
                {
                    throw new ArgumentException($"Cannot find Android library at path: '{path}'", nameof(path));
                }

                FileStream fs = File.Create(file);
                StreamWriter w = new StreamWriter(fs);
                w.WriteLine("android.library=true");
                w.Close();
            }

            // Android manifest
            file = UnityPath.Combine(path, AndroidManifestUtilities.GetManifestTemplateName(ManifestType.Main));
            AndroidManifest = File.Exists(file) ? AndroidManifest.Open(file) : AndroidManifest.Create(file, package);

            if (File.Exists(file))
            {
                AndroidManifest = AndroidManifest.Open(file);
            }
            else if (!createIfNotExists)
            {
                throw new ArgumentException($"Cannot find Android library at path: '{path}'", nameof(path));
            }
            else
            {
                AndroidManifest = AndroidManifest.Create(file, package);
            }

            // Keep disposable objects
            disposables = new CompositeDisposable
            {
                AndroidManifest
            };
        }


        public void Dispose()
        {
            disposables?.Dispose();
            disposables = null;

            AndroidManifest = null;
            OutputPath = null;
        }
        
        
        /// <summary>
        /// Save all changes.
        /// </summary>
        public void Save()
        {
            AndroidManifest.Save();
        }

        #endregion



        #region Resources management

        /// <summary>
        /// Gets path to a resource inside android library.
        /// </summary>
        /// <param name="resourceType"></param>
        /// <param name="filename"></param>
        /// <param name="resourceSuffix"></param>
        /// <returns></returns>
        public string GetResourcePath(string resourceType, string filename = null, string resourceSuffix = null)
        {
            string rs = UnityPath.Combine(OutputPath, "res");
            if (string.IsNullOrEmpty(resourceType))
            {
                return rs;
            }

            rs = UnityPath.Combine(rs, resourceType);
            if (!string.IsNullOrEmpty(resourceSuffix))
            {
                rs += "-" + resourceSuffix;
            }

            if (!string.IsNullOrEmpty(filename))
            {
                rs = UnityPath.Combine(rs, filename);
            }

            return rs;
        }


        /// <summary>
        /// Gets or creates a values resource in the library.
        /// </summary>
        /// <param name="filename">A name of the resource file.</param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        public ValuesResource GetValuesResource(string filename, bool createIfNotExists = true)
        {
            string destination = GetResourcePath(AndroidResourceType.Values, filename);
            string destinationDirectory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            ValuesResource res = null;
            if (File.Exists(destination))
            {
                res = ValuesResource.Open(destination);
                disposables.Add(res);
            }
            else if (createIfNotExists)
            {
                res = ValuesResource.Create(destination);
                disposables.Add(res);
            }

            return res;
        }


        /// <summary>
        /// Gets or creates a backup rules in the library.
        /// </summary>
        /// <param name="filename">A name of the resource file.</param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        public BackupRulesResource GetBackupRulesResource(string filename, bool createIfNotExists = true)
        {
            string destination = GetResourcePath(AndroidResourceType.Xml, filename);
            string destinationDirectory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            BackupRulesResource res = null;
            if (File.Exists(destination))
            {
                res = BackupRulesResource.Open(destination);
                disposables.Add(res);
            }
            else if (createIfNotExists)
            {
                res = BackupRulesResource.Create(destination);
                disposables.Add(res);
            }

            return res;
        }


        /// <summary>
        /// Gets or creates a network security config in the library.
        /// </summary>
        /// <param name="filename">A name of the resource file.</param>
        /// <param name="createIfNotExists"></param>
        /// <returns></returns>
        public NetworkSecurityConfigResource GetNetworkSecurityConfigResource(string filename, bool createIfNotExists = true)
        {
            string destination = GetResourcePath(AndroidResourceType.Xml, filename);
            string destinationDirectory = Path.GetDirectoryName(destination);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            NetworkSecurityConfigResource res = null;
            if (File.Exists(destination))
            {
                res = NetworkSecurityConfigResource.Open(destination);
                disposables.Add(res);
            }
            else if (createIfNotExists)
            {
                res = NetworkSecurityConfigResource.Create(destination);
                disposables.Add(res);
            }

            return res;
        }


        /// <summary>
        /// Adds a texture asset to resources as png with specified file name.
        /// </summary>
        /// <param name="texture">Source texture.</param>
        /// <param name="filename">Output file name.</param>
        /// <param name="resourceType">A type of the resource.</param>
        /// <param name="resourceSuffix">A suffix of the resource. </param>
        public void AddResource(
            Texture2D texture, 
            string filename, 
            string resourceType = AndroidResourceType.Mipmap, 
            string resourceSuffix = null)
        {
            byte[] bytes = null;
            try
            {
                bytes = texture.EncodeToPNG();
            }
            catch
            {
                TextureImporter importer = AssetDatabaseUtilities.GetAssetImporter<TextureImporter>(texture);
                bytes = AssetDatabaseUtilities.EncodeTextureToPng(importer);
            }

            string path = GetResourcePath(resourceType, filename, resourceSuffix);
            WriteBytesToFile(path, bytes);
        }


        /// <summary>
        /// Adds a texture asset to resources as png with specified file name.
        /// </summary>
        /// <param name="importer">Source texture.</param>
        /// <param name="filename">Output file name.</param>
        /// <param name="resourceType">A type of the resource.</param>
        /// <param name="resourceSuffix">A suffix of the resource. </param>
        public void AddResource(
            TextureImporter importer, 
            string filename,
            string resourceType = AndroidResourceType.Mipmap, 
            string resourceSuffix = null)
        {
            byte[] bytes = AssetDatabaseUtilities.EncodeTextureToPng(importer);

            string path = GetResourcePath(resourceType, filename, resourceSuffix);
            WriteBytesToFile(path, bytes);
        }

        #endregion



        #region Private tools

        private static void CreateDirectoryForFile(string path)
        {
            string destinationDirectory = Path.GetDirectoryName(path);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
        }


        private static void WriteBytesToFile(string path, byte[] bytes)
        {
            CreateDirectoryForFile(path);
            using (Stream stream = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(bytes);
            }
        }

        #endregion
    }
}
