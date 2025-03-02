using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor
{
    public static class UnityPath
    {
        #region Helper classes
        
        public enum PathPatternType
        {
            File = 1,
            Directory = 2
        }
        
        #endregion
        

        #region Fields
        
        public const char PathSeparator = '/';

        public const string AssetsDirectoryName = "Assets";
        public const string ResourcesDirectoryName = "Resources";
        public const string StreamingAssetsDirectoryName = "StreamingAssets";
        public static readonly string ProjectPath = FixPathSeparator(new DirectoryInfo(Application.dataPath).Parent.FullName);
        public static readonly string ProjectSettingsPath = Combine(ProjectPath, "ProjectSettings", "ProjectSettings.asset");
        public static readonly string StreamingAssetsAssetPath = Combine(AssetsDirectoryName, StreamingAssetsDirectoryName);
        public static readonly string PluginsAssetPath = Combine(AssetsDirectoryName, "Plugins");
        public static readonly string ExternalPluginsSettingsAssetPath = Combine(AssetsDirectoryName, "ExternalPlugins");
        public static readonly string AndroidPluginsAssetPath = Combine(PluginsAssetPath, "Android");
        public static readonly string IosPluginsAssetPath = Combine(PluginsAssetPath, "iOS");

        public const string ProjectTempDirectory = "Temp";
        public static readonly string ProjectTempPath = Combine(ProjectPath, ProjectTempDirectory);
        public const string StagingAreaDirectory = "StagingArea";
        public static readonly string StagingAreaPath = Combine(ProjectPath, StagingAreaDirectory);        

        private const string PlaybackEnginesDirectory = "PlaybackEngines";
        private const string AndroidPlaybackEngineDirectory = "AndroidPlayer";
        private const string IosPlaybackEngineDirectory = "iOSSupport";
        
        private const string PackagesAssetPathDirectoryName = "Packages";
        private static readonly string PackagesProjectPath = Combine("Library", "PackageCache");
        private static readonly Regex PackageVersionRegex = new Regex("(@.*?)(?![^/])");
        private static readonly Regex PackageNameRegex = new Regex("(^.*?)(?![^/])");
        private static readonly Regex MetaGuidRegex = new Regex(@"\b(?<=^guid:\s+)\w+\b");

        private static string androidPlaybackEnginePath = null;
        private static string iosPlaybackEnginePath = null;
        
        #endregion
        


        #region Main tools

        public static string Combine(params string[] paths)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(paths[0]);

            for (int i = 1; i < paths.Length; i++)
            {
                if (!string.IsNullOrEmpty(paths[i]))
                {
                    if (sb[sb.Length - 1] != PathSeparator)
                    {
                        sb.Append(PathSeparator);
                    }

                    sb.Append(paths[i].Trim('/', '\\'));
                }
            }

            return sb.ToString();
        }


        public static string FixPathSeparator(string path)
        {
            return path.Replace('\\', PathSeparator);
        }


        public static string RemoveStart(string path, string value)
        {
            path = FixPathSeparator(path);
            value = FixPathSeparator(value);

            string result = path;
            if (path.StartsWith(value))
            {
                result = path.Substring(value.Length).TrimStart(PathSeparator);
            }
            
            return result;
        }


        public static string RemoveEnd(string path, string value)
        {
            path = FixPathSeparator(path);
            value = FixPathSeparator(value);

            string result = path;
            if (path.EndsWith(value))
            {
                result = path.Remove(path.Length - value.Length).TrimEnd(PathSeparator);
            }
            
            return result;
        }


        public static string GetDirectoryName(string path)
        {
            return FixPathSeparator(Path.GetDirectoryName(FixPathSeparator(path)));
        }
        
        
        public static string GetFileName(string path)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(path))
            {
                string directoryName = GetDirectoryName(path);
                result = !string.IsNullOrEmpty(directoryName) ? path.Substring(directoryName.Length) : path;
                result = result.Trim('/', '\\');
            }
            
            return result;
        }
        
        
        public static string FindPathByPattern(string pathPattern, PathPatternType pathPatternType)
        {
            string result = string.Empty;
            
            pathPattern = FixPathSeparator(pathPattern);
            pathPattern = pathPattern.Trim('/', '\\');
            
            if (!string.IsNullOrEmpty(pathPattern))
            {
                DirectoryInfo rootDirectoryInfo = new DirectoryInfo(ProjectPath);
            
                if (pathPatternType == PathPatternType.File)
                {
                    int index = pathPattern.LastIndexOf(Path.DirectorySeparatorChar);
                    string fileName = Path.GetFileName(pathPattern);
                    string pathToFile = (index > 0) ? pathPattern.Substring(0, index) : string.Empty;
                    
                    FileInfo[] fileInfos = rootDirectoryInfo.GetFiles(fileName, SearchOption.AllDirectories);
                    for (int i = 0; i < fileInfos.Length; i++)
                    {
                        if (IsDirectoryInDirectoriesPattern(fileInfos[i].Directory, pathToFile))
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result = fileInfos[i].FullName;
                            }
                            else
                            {
                                Debug.LogWarning("Found multiple paths with pattern: " + pathPattern + ". It may leads to inconsistent results!");
                                break;
                            }
                        }
                    }
                }
                else if (pathPatternType == PathPatternType.Directory)
                {
                    string directoryName = Path.GetFileName(pathPattern);
                    string pathToDirectory = pathPattern;
                    
                    DirectoryInfo[] directoryInfos = rootDirectoryInfo.GetDirectories(directoryName, SearchOption.AllDirectories);
                    for (int i = 0; i < directoryInfos.Length; i++)
                    {
                        if (IsDirectoryInDirectoriesPattern(directoryInfos[i], pathToDirectory))
                        {
                            if (string.IsNullOrEmpty(result))
                            {
                                result = directoryInfos[i].FullName;
                            }
                            else
                            {
                                Debug.LogWarning("Found multiple paths with pattern: " + pathPattern + ". It may leads to inconsistent results!");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("Unknown path pattern type!");
                }
            }
            
            return result;
            
            
            bool IsDirectoryInDirectoriesPattern(DirectoryInfo directoryInfo, string directoriesPattern)
            {
                bool output = false;
        
                if (!string.IsNullOrEmpty(directoriesPattern))
                {
                    string[] directoriesNames = directoriesPattern.Split(Path.DirectorySeparatorChar);
                    DirectoryInfo currentDirectoryInfo = directoryInfo;
            
                    for (int i = directoriesNames.Length - 1; i >= 0; i--)
                    {
                        if (i < directoriesNames.Length - 1)
                        {
                            currentDirectoryInfo = currentDirectoryInfo.Parent;
                        }
                
                        if (currentDirectoryInfo == null || !currentDirectoryInfo.Name.Equals(directoriesNames[i]))
                        {
                            break;
                        }
                
                        if (i == 0)
                        {
                            output = true;
                            break;
                        }
                    }
                }
                else
                {
                    output = true;
                }
        
                return output;
            }
        }

        #endregion



        #region Path asserts and conversions

        /// <summary>
        /// Returns a value that indicates whether a path is located inside of the project.
        /// </summary>
        /// <param name="path">The path to test.</param>
        /// <returns>true if path contains a root; otherwise, false</returns>
        public static bool IsPathLocatedInsideProject(string path)
        {
            bool result = true;
            if (Path.IsPathRooted(path))
            {
                result = FixPathSeparator(path).StartsWith(ProjectPath);
            }

            return result;
        }


        /// <summary>
        /// Asserts that <paramref name="path"/> is located inside the Unity project.
        /// Otherwise, throws an exception.
        /// </summary>
        /// <param name="path">The path to test.</param>
        public static void AssertPathLocatedInsideProject(string path)
        {
            if (!IsPathLocatedInsideProject(path))
            {
                throw new ArgumentException($"Unable to access path outside of the project: '{path}'.", nameof(path));
            }
        }


        /// <summary>
        /// Returns an asset path from absolute path.
        /// </summary>
        /// <param name="fullPath">Absolute path.</param>
        /// <returns>Asset path.</returns>
        public static string GetAssetPathFromFullPath(string fullPath)
        {
            fullPath = FixPathSeparator(fullPath);

            string result = fullPath;
            if (Path.IsPathRooted(fullPath))
            {
                if (!fullPath.StartsWith(ProjectPath))
                {
                    throw new InvalidOperationException($"Path '{fullPath}' is outside of the project.");
                }
                result = RemoveStart(fullPath, ProjectPath);
                if (result.StartsWith(PackagesAssetPathDirectoryName))
                {
                    result = GetEmbeddedAssetPathFromFullPath(fullPath);
                }
                else
                {
                    // Check if it's path to file or directory in package
                    if (!result.StartsWith(AssetsDirectoryName) && !result.StartsWith(PackagesAssetPathDirectoryName))
                    {
                        // Remove version specifier
                        result = PackageVersionRegex.Replace(result, string.Empty);

                        // Replace leading "Library/PackageCache" to "Packages"
                        result = RemoveStart(result, PackagesProjectPath);
                        result = Combine(PackagesAssetPathDirectoryName, result);
                    }
                }
            }
            
            return result;
        }


        /// <summary>
        /// Returns an absolute path from asset path.
        /// </summary>
        /// <param name="assetPath">Asset path.</param>
        /// <returns>Absolute path.</returns>
        public static string GetFullPathFromAssetPath(string assetPath)
        {
            string result = assetPath;
            if (!Path.IsPathRooted(assetPath))
            {
                result = Path.GetFullPath(assetPath);
                result = FixPathSeparator(result);
            }

            return result;
        }


        private static string GetEmbeddedAssetPathFromFullPath(string fullPath)
        {
            string result = fullPath;
            string packagesAssetPath = Combine(ProjectPath, PackagesAssetPathDirectoryName,PathSeparator.ToString());
            if (Path.IsPathRooted(fullPath) && fullPath.StartsWith(packagesAssetPath))
            {
                string subDirectory = default;
                string fullDirectory = default;
                if (Directory.Exists(fullPath))
                {
                    fullDirectory = fullPath;
                }
                else
                {
                    subDirectory = GetFileName(fullPath);
                    fullDirectory = GetDirectoryName(fullPath);
                }

                result = GetEmbeddedFullPathFromPackagesDirectory(fullDirectory, subDirectory);
            }
            result = RemoveStart(result, ProjectPath);

            return result;
        }


        private static string GetEmbeddedFullPathFromPackagesDirectory(string fullDirectory,string subDirectory)
        {
            string result = fullDirectory;
            string packagesAssetPath = Combine(ProjectPath, PackagesAssetPathDirectoryName);
            int subDirectoryCounts = RemoveStart(fullDirectory, ProjectPath).Split(PathSeparator).Length;
            for (int i = 0; i < subDirectoryCounts; i++)
            {
                string packagePath = FileSystemUtilities.FindFileEndsWith(
                    fullDirectory,
                    ".meta",
                    SearchOption.TopDirectoryOnly);

                if (!string.IsNullOrEmpty(packagePath))
                {
                    var Lines = File.ReadAllLines(FixPathSeparator(packagePath));
                    fullDirectory = GetDirectoryName(fullDirectory);
                    foreach (var item in Lines)
                    {
                        string guid = MetaGuidRegex.Match(item).Value;
                        if (!string.IsNullOrEmpty(guid))
                        {
                            fullDirectory = GetDirectoryName(AssetDatabase.GUIDToAssetPath(guid));
                            break;
                        }
                    }
                    result = Combine(fullDirectory, subDirectory);
                    break;
                }
                else
                {
                    subDirectory = Combine(GetFileName(fullDirectory), subDirectory);
                    fullDirectory = GetDirectoryName(fullDirectory);
                }
                if (fullDirectory == packagesAssetPath)
                {
                    Debug.LogWarning($"Package wasn’t found.");
                    break;
                }
            }

            return result;
        }

        #endregion



        #region Path of script

        /// <summary>
        /// Get an asset path of the specified type of script.
        /// </summary>
        /// <param name="type">The script type.</param>
        /// <returns>The path if the script is found; otherwise, null.</returns>
        public static string GetScriptAssetPath(Type type)
        {
            var item = AssetDatabase.FindAssets("t:script")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => (path, script: AssetDatabase.LoadAssetAtPath<MonoScript>(path)))
                .FirstOrDefault(p => p.script.GetClass() == type);

            return item.path;
        }


        /// <summary>
        /// Get an asset path of the specified type of script.
        /// </summary>
        /// <typeparam name="T">The script type.</typeparam>
        /// <returns>The path if the script is found; otherwise, null.</returns>
        public static string GetScriptAssetPath<T>() => GetScriptAssetPath(typeof(T));

        #endregion



        #region Complex paths of Unity Editor

        /// <summary>
        /// Returns an enumerable collection of paths to PlaybackEngines directory of Unity Editor.
        /// </summary>
        /// <returns>An enumerable collection of paths.</returns>
        private static IEnumerable<string> EnumeratePlaybackEnginePaths()
        {
            HashSet<string> foundPaths = new HashSet<string>();

            // Try to access path directly in Unity Contents
            string playbackEnginesPath = Combine(EditorApplication.applicationContentsPath, PlaybackEnginesDirectory);
            if (Directory.Exists(playbackEnginesPath))
            {
                foundPaths.Add(playbackEnginesPath);
                yield return playbackEnginesPath;
            }

            // Try to access path directly near Unity application
            playbackEnginesPath = Combine(GetDirectoryName(EditorApplication.applicationPath), PlaybackEnginesDirectory);
            if (Directory.Exists(playbackEnginesPath))
            {
                foundPaths.Add(playbackEnginesPath);
                yield return playbackEnginesPath;
            }

            // Try to find playback engines folder in Unity Contents by name
            foreach (var path in Directory.EnumerateDirectories(EditorApplication.applicationContentsPath, PlaybackEnginesDirectory, SearchOption.AllDirectories))
            {
                playbackEnginesPath = FixPathSeparator(path);
                if (!foundPaths.Contains(playbackEnginesPath))
                {
                    foundPaths.Add(playbackEnginesPath);
                    yield return playbackEnginesPath;
                }
            }

            if (foundPaths.Count == 0)
            {
                throw new NotSupportedException("Failed to find PlaybackEngines directory on Unity Editor.");
            }
        }


        /// <summary>
        /// Gets path to content of Android playback engine.
        /// </summary>
        public static string AndroidPlaybackEnginePath
        {
            get
            {
                if (androidPlaybackEnginePath != null)
                {
                    return androidPlaybackEnginePath;
                }

                androidPlaybackEnginePath = EnumeratePlaybackEnginePaths()
                    .Select(path => Combine(path, AndroidPlaybackEngineDirectory))
                    .FirstOrDefault(Directory.Exists);

                if (string.IsNullOrEmpty(androidPlaybackEnginePath))
                {
                    throw new NotSupportedException("Failed to find a playback engine folder for Android platform. Perhaps the Android module is not installed.");
                }

                return androidPlaybackEnginePath;
            }
        }


        /// <summary>
        /// Gets path to content of iOS playback engine.
        /// </summary>
        public static string IosPlaybackEnginePath
        {
            get
            {
                if (iosPlaybackEnginePath != null)
                {
                    return iosPlaybackEnginePath;
                }

                iosPlaybackEnginePath = EnumeratePlaybackEnginePaths()
                    .Select(path => Combine(path, IosPlaybackEngineDirectory))
                    .FirstOrDefault(Directory.Exists);

                if (string.IsNullOrEmpty(iosPlaybackEnginePath))
                {
                    throw new NotSupportedException("Failed to find a playback engine folder for iOS platform. Perhaps the iOS module is not installed.");
                }

                return iosPlaybackEnginePath;
            }
        }

        #endregion
    }
}
