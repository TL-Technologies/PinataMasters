using System;
using System.IO;
using UnityEngine;


namespace Modules.General.HelperClasses
{
    public static class PathUtils
    {
        #region Helpers
        
        public enum PathPatternType
        {
            File         = 1,
            Directory    = 2
        }
        
        #endregion
        
        
        
        #region Constants
    
        public const string CSV_EXTENSION = "csv";
        public const string PNG_EXTENSION = "png";
        public const string CAF_EXTENSION = "caf";
    
        public const char CHAR_DOT = '.';
    
    #if !UNITY_WSA
        public const string WWW_FILE_PREFIX = "file://";
    #else
        public const string WWW_FILE_PREFIX = "file:///";
    #endif
    
        public const string WWW_JAR_PREFIX = "jar:";
    
        const string FORMAT_PATH_ADDITION = "{0}{1}";
        const string FORMAT_PATH_EXTENSION = "{0}.{1}";
        const string FORMAT_PATH_BASHSHELL = "\"{0}\"";
        
        static readonly char[] PATH_SEPARATORS = { '/', '\\' };
    
        #endregion
    
    
    
        #region Unity Utils
    
        public static string GetDataRoot()
        {
            string result = Application.streamingAssetsPath;
    
            #if UNITY_EDITOR
                result = Application.streamingAssetsPath.RemoveLastPathComponent();
            #elif UNITY_ANDROID
                result = Application.streamingAssetsPath;
            #elif UNITY_IOS
                result = Application.streamingAssetsPath.RemoveLastPathComponent().RemoveLastPathComponent();
            #else
                result = Application.streamingAssetsPath.RemoveLastPathComponent();
            #endif
    
            return result;
        }
    
    
        /// <summary>
        /// Appends the StreamingAsset path to file.
        /// </summary>
        public static string GetStreamingAssetsFile(string fileName, string extension = null)
        {
            string result = Application.streamingAssetsPath.AppendPathComponent(fileName);
            if (extension != null)
            {
                result = result.AppendPathExtension(extension);
            }
            
            return result;
        }
    
    
        /// <summary>
        /// Appends the StreamingAsset path to file.
        /// </summary>
        public static string GetStreamingAssetsCsv(string fileName)
        {
            return GetStreamingAssetsFile(fileName, CSV_EXTENSION);
        }
    
    
        /// <summary>
        /// Appends the StreamingAsset path to file.
        /// </summary>
        public static string GetStreamingAssetsPng(string fileName)
        {
            return GetStreamingAssetsFile(fileName, PNG_EXTENSION);
        }
    
    
        /// <summary>
        /// Appends the StreamingAsset path to file.
        /// </summary>
        public static string GetStreamingAssetsCaf(string fileName)
        {
            return GetStreamingAssetsFile(fileName, CAF_EXTENSION);
        }
    
        #endregion  
    
    
    
        #region String Utils
    
        public static string AppendPathComponent(this string str, string pathComponent)
        {
            return Path.Combine(str, pathComponent);
        }
    
    
        public static string AppendPathExtension(this string str, string pathExtension)
        {
            return string.Format(FORMAT_PATH_EXTENSION, str, pathExtension);
        }
    
    
        /// <summary>
        /// Appends the WWW prefix for work with WWW class.
        /// </summary>
        public static string AppendWwwFilePrefix(this string str)
        {
            if (!str.Contains(WWW_FILE_PREFIX))
            {
                str = string.Format(FORMAT_PATH_ADDITION, WWW_FILE_PREFIX, str);
            }
            
            return str;
        }
    
    
        /// <summary>
        /// Appends the JAR prefix for StreamingAsset on Android.
        /// </summary>
        public static string AppendWwwJarPrefix(this string str)
        {
            if (!str.Contains(WWW_JAR_PREFIX))
            {
                str = string.Format(FORMAT_PATH_ADDITION, WWW_JAR_PREFIX, str);
            }
            return str;
        }
    
    
        public static string RemoveLastPathComponent(this string str)
        {
            #if UNITY_EDITOR_WIN
                // Fix for the most often issue causing build process to fail on Windows
                return Path.GetDirectoryName(str.TrimEnd('\\', '/'));
            #else
                if (str.Length >= 2)
                {
                    int index = str.LastIndexOf(Path.DirectorySeparatorChar, str.Length - 2);
                    if (index > 0)
                    {
                        return str.Remove(index);
                    }
                }
    
                return string.Empty;
            #endif
        }
    
    
        public static string RemovePathExtension(this string str)
        {
            if (str.Length >= 2)
            {
                int index = str.LastIndexOf(CHAR_DOT, str.Length - 2);
                if (index > 0)
                {
                    return str.Remove(index);
                }
            }
    
            return string.Empty;
        }
    
    
        public static string ShellString(this string str)
        {
            return string.Format(FORMAT_PATH_BASHSHELL, str);
        }
    
    
        public static string ValidatePathSeparators(this string s)
        {
            string[] temp = s.Split(PATH_SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
            return string.Join(Path.DirectorySeparatorChar.ToString(), temp);
        }
        
        
        [Obsolete("Use UnityPath.FindPathByPattern from HivePlugin instead of this method.")]
        public static string FindPathByPattern(string pathPattern, PathPatternType pathPatternType)
        {
            string result = string.Empty;
            
            pathPattern = ValidatePathSeparators(pathPattern);
            pathPattern = pathPattern.Trim(PATH_SEPARATORS);
            
            if (!string.IsNullOrEmpty(pathPattern))
            {
                DirectoryInfo rootDirectoryInfo = new DirectoryInfo(Application.dataPath);
            
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
                                CustomDebug.LogWarning("Found multiple paths with pattern: " + pathPattern + ". It may leads to inconsistent results!");
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
                                CustomDebug.LogWarning("Found multiple paths with pattern: " + pathPattern + ". It may leads to inconsistent results!");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    CustomDebug.LogWarning("Unknown path pattern type!");
                }
            }
            
            return result;
        }
    
        #endregion
        
        
        
        #region Private methods
        
        static bool IsDirectoryInDirectoriesPattern(DirectoryInfo directoryInfo, string directoriesPattern)
        {
            bool result = false;
            
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
                        result = true;
                        break;
                    }
                }
            }
            else
            {
                result = true;
            }
            
            return result;
        }
        
    #endregion
    }
}
