using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    public sealed class ProjectSnapshot
    {
        #region Fields
        
        private static readonly string SnapshotPath = UnityPath.Combine(UnityPath.ProjectTempPath, "ProjectSnapshot");
        private static readonly string StashPath = UnityPath.Combine(SnapshotPath, "Stash");
        private static readonly string PluginsAndroidPath = UnityPath.Combine(UnityPath.ProjectPath, "Assets", "Plugins", "Android");
        private static readonly string PluginsIosPath = UnityPath.Combine(UnityPath.ProjectPath, "Assets", "Plugins", "iOS");
        private static readonly string SavedDirectoriesFileName = UnityPath.Combine(SnapshotPath, "saved.directories");
        private static readonly string DeletingFileName = UnityPath.Combine(SnapshotPath, "deleting.marks");

        private static ProjectSnapshot currentSnapshot = null;
        
        #endregion
        


        #region Instancing

        public static ProjectSnapshot CurrentSnapshot
        {
            get
            {
                if (currentSnapshot == null && Directory.Exists(SnapshotPath))
                {
                    currentSnapshot = new ProjectSnapshot();
                }

                return currentSnapshot;
            }
        }


        private ProjectSnapshot()
        {
            if (!Directory.Exists(SnapshotPath))
            {
                Directory.CreateDirectory(SnapshotPath);
            }
        }

        #endregion



        #region Snapshot management

        public static void MakeSnapshot()
        {
            if (CurrentSnapshot != null)
            {
                return;
            }

            // Save and refresh before access to list of assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentSnapshot = new ProjectSnapshot();
            EnsureDirectoryImmutability(PluginsAndroidPath);
            EnsureDirectoryImmutability(PluginsIosPath);
            EnsureDirectoryImmutability(HivePluginHierarchy.Instance.GetEditorPlatformTemplatesAssetPath(PlatformAlias.Android));
            EnsureDirectoryImmutability(HivePluginHierarchy.Instance.GetPlatformAssetsPath(PlatformAlias.Android));
            EnsureDirectoryImmutability(HivePluginHierarchy.Instance.GetPlatformAssetsPath(PlatformAlias.Ios));

            void EnsureDirectoryImmutability(string directoryPath)
            {
                if (Directory.Exists(directoryPath))
                {
                    currentSnapshot.SaveDirectoryStructure(directoryPath);
                }
                else
                {
                    Directory.CreateDirectory(directoryPath);
                    currentSnapshot.MarkAssetToDelete(UnityPath.GetAssetPathFromFullPath(directoryPath));
                }
            }
        }


        public static void RestoreSnapshot()
        {
            if (CurrentSnapshot == null)
            {
                return;
            }

            // Save and refresh before restore stashed assets
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentSnapshot.RestoreDirectoryStructure();
            currentSnapshot.DeleteMarkedFiles();
            currentSnapshot.RestoreFromStash();

            DeleteSnapshot();
            AssetDatabase.Refresh();
        }


        public static void DeleteSnapshot()
        {
            if (CurrentSnapshot == null)
            {
                return;
            }

            if (Directory.Exists(SnapshotPath))
            {
                Directory.Delete(SnapshotPath, true);
            }

            currentSnapshot = null;
        }

        #endregion



        #region Save/restore directory structure
        
        public void SaveDirectoryStructure(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                if (File.Exists(directoryPath))
                {
                    Debug.LogWarning($"{directoryPath} is a file's path, not directory! Ignoring.");
                }
                return;
            }
            
            string directoryLocalPath = UnityPath.RemoveStart(directoryPath, UnityPath.ProjectPath);
            string targetPath = UnityPath.Combine(SnapshotPath, directoryLocalPath);
            if (Directory.Exists(targetPath))
            {
                Debug.LogWarning($"Saving directory in path {targetPath}, that already presented in snapshot!");
            }
            
            CopyDirectory(directoryPath, targetPath, true, false);
            string metaFilePath = $"{directoryPath}.meta";
            if (File.Exists(metaFilePath))
            {
                CopyFile(metaFilePath, $"{targetPath}.meta", true, false);
            }
            
            FileMode fileMode = File.Exists(SavedDirectoriesFileName) ? FileMode.Append : FileMode.Create;
            using (FileStream stream = new FileStream(SavedDirectoriesFileName, fileMode))
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(directoryLocalPath);
            }
        }
        
        
        private void RestoreDirectoryStructure()
        {
            if (!File.Exists(SavedDirectoriesFileName))
            {
                return;
            }
            
            using (FileStream stream = new FileStream(SavedDirectoriesFileName, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string path = reader.ReadString();
                    string projectPath = UnityPath.Combine(UnityPath.ProjectPath, path);
                    string snapshotPath = UnityPath.Combine(SnapshotPath, path);
                    string metaFilePath = $"{snapshotPath}.meta";
                    
                    // This check necessary in case, when we save structure of subdirectory
                    // to previously saved directory. While restoring, we correctly restore
                    // firstly saved directory (simultaneously with secondly saved subdirectory), 
                    // and secondly saved directory will not exist, when we read it from file.
                    if (Directory.Exists(snapshotPath))
                    {
                        if (Directory.Exists(projectPath))
                        {
                            Directory.Delete(projectPath, true);
                        }
                        CopyDirectory(snapshotPath, projectPath, true, true);
                    
                        if (File.Exists(metaFilePath))
                        {
                            CopyFile(metaFilePath, $"{projectPath}.meta", true, true);
                        }
                    }
                }
            }
        }

        #endregion



        #region Stash management

        public bool CopyFileToStash(string assetPath, bool overwrite = true)
        {
            return Stash(assetPath, overwrite, false, false);
        }


        public bool MoveFileToStash(string assetPath, bool overwrite = true)
        {
            return Stash(assetPath, overwrite, true, false);
        }


        public bool CopyAssetToStash(string assetPath, bool overwrite = true)
        {
            return Stash(assetPath, overwrite, false, true);
        }


        public bool MoveAssetToStash(string assetPath, bool overwrite = true)
        {
            return Stash(assetPath, overwrite, true, true);
        }


        public bool CopyAssetToStash(Object asset, bool overwrite = true)
        {
            return Stash(asset, overwrite, false);
        }


        public bool MoveAssetToStash(Object asset, bool overwrite = true)
        {
            return Stash(asset, overwrite, true);
        }


        private void RestoreFromStash()
        {
            if (!Directory.Exists(StashPath))
            {
                return;
            }

            CopyDirectory(StashPath, UnityPath.ProjectPath, true, true);

            if (Directory.Exists(StashPath))
            {
                Directory.Delete(StashPath, true);
            }
        }


        private static bool Stash(Object asset, bool overwrite, bool move)
        {
            if (!AssetDatabase.IsMainAsset(asset) || !EditorUtility.IsPersistent(asset))
            {
                Debug.LogError("Failed to stash asset because it's not a main/persistent asset.");
                return false;
            }

            string assetPath = AssetDatabase.GetAssetPath(asset);
            return Stash(assetPath, overwrite, move, true);
        }


        private static bool Stash(string assetPath, bool overwrite, bool move, bool includeMeta)
        {
            string source = UnityPath.Combine(UnityPath.ProjectPath, assetPath);
            bool isFile = File.Exists(source);
            bool isDirectory = !isFile && Directory.Exists(source);

            bool result = false;
            if (isFile || isDirectory)
            {
                result = true;
                
                // Make sure a target directory exists
                string destination = UnityPath.Combine(StashPath, assetPath);
                string destinationDirectory = UnityPath.GetDirectoryName(destination);
                if (!Directory.Exists(destinationDirectory))
                {
                    Directory.CreateDirectory(destinationDirectory);
                }

                // Copy or move asset
                if (isFile)
                {
                    CopyFile(source, destination, overwrite, move);
                }
                else if (isDirectory)
                {
                    CopyDirectory(source, destination, overwrite, move);
                }

                // Copy or move meta
                if (includeMeta)
                {
                    string sourceMeta = source + ".meta";
                    if (File.Exists(sourceMeta))
                    {
                        CopyFile(sourceMeta, destination + ".meta", overwrite, move);
                    }
                }
            }

            return result;
        }


        private static void CopyFile(string source, string destination, bool overwrite, bool move)
        {
            if (!move)
            {
                File.Copy(source, destination, overwrite);
            }
            else
            {
                bool exists = File.Exists(destination);
                if (exists && overwrite)
                {
                    File.Delete(destination);
                }

                if (!exists || overwrite)
                {
                    File.Move(source, destination);
                }
            }
        }


        private static void CopyDirectory(string source, string destination, bool overwrite, bool move)
        {
            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            string[] files = Directory.GetFiles(source);
            for (int i = 0; i < files.Length; i++)
            {
                string sourceFile = files[i];
                string destinationFile = UnityPath.GetFileName(sourceFile);
                destinationFile = UnityPath.Combine(destination, destinationFile);
                CopyFile(sourceFile, destinationFile, overwrite, move);
            }

            string[] dirs = Directory.GetDirectories(source);
            for (int i = 0; i < dirs.Length; i++)
            {
                string sourceDirectory = dirs[i];
                string destinationDirectory = UnityPath.GetFileName(sourceDirectory);
                destinationDirectory = UnityPath.Combine(destination, destinationDirectory);
                CopyDirectory(sourceDirectory, destinationDirectory, overwrite, move);
            }

            if (move)
            {
                Directory.Delete(source, true);
            }
        }

        #endregion



        #region Deleting marks

        public void MarkFileToDelete(params string[] assetPaths)
        {
            if (assetPaths == null || assetPaths.Length == 0)
            {
                return;
            }

            MarkAssetToDelete(assetPaths);
        }


        public void MarkAssetToDelete(params string[] assetPaths)
        {
            if (assetPaths == null || assetPaths.Length == 0)
            {
                return;
            }

            MarkAssetToDelete(assetPaths.SelectMany(p => new[] { p, $"{p}.meta" }));
        }


        private void MarkAssetToDelete(IEnumerable<string> assetPaths)
        {
            FileStream stream = File.Exists(DeletingFileName) ?
                File.OpenWrite(DeletingFileName) :
                File.Create(DeletingFileName);

            stream.Seek(0, SeekOrigin.End);

            using (stream)
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                foreach (string assetPath in assetPaths)
                {
                    if (UnityPath.IsPathLocatedInsideProject(assetPath))
                    {
                        writer.Write(assetPath);
                    }
                    else
                    {
                        Debug.Log($"Cannot mark path '{assetPath}' to delete. It's outside of the project.");
                    }
                }
            }
        }


        private void DeleteMarkedFiles()
        {
            if (!File.Exists(DeletingFileName))
            {
                return;
            }

            using (FileStream stream = File.OpenRead(DeletingFileName))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                while (reader.PeekChar() > 0)
                {
                    string assetPath = reader.ReadString();
                    FileSystemUtilities.DeleteEntryAndEmptyParentsDirectories(UnityPath.GetFullPathFromAssetPath(assetPath));
                }
            }

            File.Delete(DeletingFileName);
        }

        #endregion
    }
}
