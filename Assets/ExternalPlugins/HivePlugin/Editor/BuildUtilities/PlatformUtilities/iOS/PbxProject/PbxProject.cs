using Modules.Hive.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public class PbxProject : PBXProject, IDisposable
    {
        private Dictionary<PbxProjectTargetType, string> targetGuidByType = new Dictionary<PbxProjectTargetType, string>();

        /// <summary>
        /// Gets a path to save the pbx-file.
        /// </summary>
        public string OutputPath { get; private set; }


        public string XcodeProjectPath { get; }


        #region Instancing

        /// <summary>
        /// Opens a pbx-file by path.
        /// </summary>
        /// <param name="path">A path to pbx-file.</param>
        public static PbxProject Open(string path)
        {
            return new PbxProject(path);
        }


        /// <summary>
        /// Opens a pbx-file file located at specified folder.
        /// </summary>
        /// <param name="folderPath">A path to a folder contains *.xcodeproj</param>
        public static PbxProject OpenInFolder(string folderPath)
        {
            return new PbxProject(GetPBXProjectPath(folderPath));
        }


        private PbxProject(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}", path);
            }
            
            FixXcframeworksBug();

            OutputPath = UnityPath.FixPathSeparator(path);

            string projectPath = Path.GetDirectoryName(path);
            if (projectPath.EndsWith(".xcodeproj"))
            {
                projectPath = Path.GetDirectoryName(projectPath);
            }
            XcodeProjectPath = UnityPath.FixPathSeparator(projectPath);

            ReadFromFile(path);
            
            
            void FixXcframeworksBug()
            {
                // Class UnityEditor.iOS.Xcode.PBX.FileTypeUtils does not contain record for type *.xcframework,
                // so we have to add it, otherwise this type will be added to Resources phase instead of Frameworks one.
                Type fileTypeUtilsType = Type.GetType("UnityEditor.iOS.Xcode.PBX.FileTypeUtils, UnityEditor.iOS.Extensions.Xcode");
                Type fileTypeDescType = Type.GetType("UnityEditor.iOS.Xcode.PBX.FileTypeUtils+FileTypeDesc, UnityEditor.iOS.Extensions.Xcode");
                
                IDictionary typesDictionary = ReflectionHelper.GetFieldValue<IDictionary>(
                    fileTypeUtilsType,
                    null,
                    "types");
                if (!typesDictionary.Contains("xcframework"))
                {
                    Type pbxFileTypeEnumType = Type.GetType("UnityEditor.iOS.Xcode.PBX.PBXFileType, UnityEditor.iOS.Extensions.Xcode");
                    object pbxFileType = Enum.Parse(pbxFileTypeEnumType, "Framework");
                    
                    object fileTypeDescription = Activator.CreateInstance(
                        fileTypeDescType,
                        "wrapper.xcframework",
                        pbxFileType);
                    
                    typesDictionary.Add("xcframework", fileTypeDescription);
                }
                
                ReflectionHelper.SetFieldValue(
                    fileTypeUtilsType,
                    null,
                    "types",
                    typesDictionary);
            }
        }


        public void Dispose()
        {
            if (OutputPath == null)
            {
                return;
            }

            Save();
            OutputPath = null;
        }

        #endregion
        


        #region Pbx items management

        /// <summary>
        /// Adds specified system framework to Unity target of the project.
        /// </summary>
        /// <param name="framework">The system framework to add.</param>
        /// <param name="weak">True, if you want to add the system framework by weak reference.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddSystemFramework(
            Framework framework,
            bool weak = false,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            AddFrameworkToProject(GetTargetGuid(targetType), framework.Reference, weak);
        }


        /// <summary>
        /// Adds a custom framework to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to the framework.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="embedded">True, if the framework should be linked as embedded.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddFramework(
            string path, 
            string structPath = null, 
            bool embedded = false,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (structPath == null)
            {
                structPath = GetDefaultStructPath(path);
            }

            string guid = AddFile(path, structPath, PBXSourceTree.Source);

            AddFileToBuild(GetTargetGuid(targetType), guid);
            if (embedded)
            {
                this.AddFrameworkToEmbeddedBinaries(GetTargetGuid(targetType), guid);
            }

            path = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(path))
            {
                AddBuildProperty(GetTargetGuid(targetType), "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/" + path);
            }
        }


        /// <summary>
        /// Adds specified system dynamic library to Unity target of the project.
        /// </summary>
        /// <param name="dylib">The system dynamic library to add.</param>
        /// <param name="weak">True, if you want to add the system framework by weak reference.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddSystemDylib(
            DyLib dylib, 
            bool weak = false,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            string dylibName = dylib.Reference;
            string fileGuid = AddFile($"usr/lib/{dylibName}", $"Frameworks/{dylibName}", PBXSourceTree.Sdk);
            Action<PBXProject, string, string, bool, string> addBuildFileImplDelegate = 
                ReflectionHelper.CreateDelegateToMethod<Action<PBXProject, string, string, bool, string>>(
                    typeof(PBXProject), 
                    "AddBuildFileImpl", 
                    BindingFlags.NonPublic | BindingFlags.Instance, 
                    false);
            addBuildFileImplDelegate(this, GetTargetGuid(targetType), fileGuid, weak, null);
        }


        /// <summary>
        /// Adds a custom static library to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to the static library.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddLib(
            string path, 
            string structPath = null,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (structPath == null)
            {
                structPath = GetDefaultStructPath(path);
            }

            string guid = AddFile(path, structPath, PBXSourceTree.Source);
            AddFileToBuild(GetTargetGuid(targetType), guid);

            path = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(path))
            {
                AddBuildProperty(GetTargetGuid(targetType), "LIBRARY_SEARCH_PATHS", "$(PROJECT_DIR)/" + path);
            }
        }


        /// <summary>
        /// Adds a resources bundle to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to the bundle.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddBundle(
            string path, 
            string structPath = null,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (structPath == null)
            {
                structPath = GetDefaultStructPath(path);
            }

            string guid = AddFile(path, structPath, PBXSourceTree.Source);
            AddFileToBuild(GetTargetGuid(targetType), guid);
        }


        /// <summary>
        /// Adds a resource file to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to the resource file.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddResource(
            string path, 
            string structPath = null,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (structPath == null)
            {
                structPath = GetDefaultStructPath(path);
            }

            string guid = AddFile(path, structPath, PBXSourceTree.Source);
            AddFileToBuild(GetTargetGuid(targetType), guid);
        }


        /// <summary>
        /// Adds a source code file to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to the source code file.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddCodeFile(
            string path, 
            string structPath = null,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (structPath == null)
            {
                structPath = GetDefaultStructPath(path);
            }

            string guid = AddFile(path, structPath, PBXSourceTree.Source);
            AddFileToBuild(GetTargetGuid(targetType), guid);
        }


        /// <summary>
        /// Adds linker flags to Unity target of the project.
        /// </summary>
        /// <param name="targetType">Xcode project target type.</param>
        /// <param name="flags">Flags to add.</param>
        public void AddLinkerFlags(
            PbxProjectTargetType targetType,
            params LinkerFlag[] flags)
        {
            foreach (var flag in flags)
            {
                if (!flag.Scope.HasFlag(LinkerFlagScope.Project))
                {
                    throw new InvalidOperationException($"Cannot use a linker flag '{flag.Flag}' in project scope.");
                }

                AddBuildProperty(GetTargetGuid(targetType), "OTHER_LDFLAGS", flag.Flag);
            }
        }


        /// <summary>
        /// Sets build property in Unity target of the project.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="targetType">Xcode project target type.</param>
        public void SetBuildProperty(
            string name, 
            string value,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            SetBuildProperty(GetTargetGuid(targetType), name, value);
        }


        /// <summary>
        /// Adds a shell script to build phase for Unity target of the project.
        /// </summary>
        /// <param name="name">A name of shell script (equals to stage name).</param>
        /// <param name="shellScript">The shell script body (source code).</param>
        public void AddShellScriptBuildPhase(
            string name, 
            string shellScript)
        {
            AddShellScriptBuildPhase(name, null, shellScript);
        }


        /// <summary>
        /// Adds a shell script to build phase for Unity target of the project.
        /// </summary>
        /// <param name="name">A name of shell script (equals to stage name).</param>
        /// <param name="shellPath">A path to shell. By default, /bin/sh.</param>
        /// <param name="shellScript">The shell script body (source code).</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddShellScriptBuildPhase(
            string name, 
            string shellPath, 
            string shellScript,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            if (shellPath == null)
            {
                shellPath = "/bin/sh";
            }

            AddShellScriptBuildPhase(GetTargetGuid(targetType), name, shellPath, shellScript);
        }
        
        
        internal string GetTargetGuid(PbxProjectTargetType targetType)
        {
            if (!targetGuidByType.TryGetValue(targetType, out string result))
            {
                switch (targetType)
                {
                    case PbxProjectTargetType.Framework: 
                        result = GetUnityFrameworkTargetGuid();
                        break;
                    case PbxProjectTargetType.Main:
                        result = GetUnityMainTargetGuid();
                        break;
                    case PbxProjectTargetType.Test:
                        result = TargetGuidByName(GetUnityTestTargetName());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetType), targetType, null);
                }
                
                targetGuidByType[targetType] = result;
            }
            
            return result;
        }


        /// <summary>
        /// Transforms specified path to a default path to group in xcode project structure.
        /// </summary>
        /// <param name="path">A path to transform.</param>
        /// <returns>Default path to group in xcode project structure.</returns>
        private string GetDefaultStructPath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            // check whether the path is inside the project
            string fullPath = Path.GetFullPath(path);
            string projectPath = Path.GetFullPath(XcodeProjectPath);
            if (fullPath.StartsWith(projectPath, StringComparison.OrdinalIgnoreCase))
            {
                fullPath = fullPath.Remove(0, projectPath.Length).TrimStart('/', '\\');
                return fullPath;
            }

            // otherwise, remove root from path
            string root = Path.GetPathRoot(path);
            if (!string.IsNullOrEmpty(root))
            {
                path = path.Remove(0, root.Length);
            }

            return path;
        }

        #endregion



        #region Add files recursively 

        private class RecursionData
        {
            public string FileSystemPath { get; }
            public string StructPath { get; }
            public string ProjectRelativePath { get; }
            public bool IsEmbedded { get; }
            public PbxProjectTargetType TargetType { get; }


            public RecursionData(
                string path, 
                string xcodeProjectPath, 
                string structPath,
                PbxProjectTargetType targetType)
            {
                if (string.IsNullOrEmpty(path))
                {
                    throw new ArgumentNullException(nameof(path));
                }

                FileSystemPath = UnityPath.FixPathSeparator(path);
                StructPath = structPath;
                IsEmbedded = path.EndsWith(".embeddedframework", StringComparison.OrdinalIgnoreCase);
                TargetType = targetType;

                // Check whether the path is inside the project
                string projectRelativePath = Path.GetFullPath(path);
                string projectRootPath = Path.GetFullPath(xcodeProjectPath);
                if (projectRelativePath.StartsWith(projectRootPath, StringComparison.OrdinalIgnoreCase))
                {
                    projectRelativePath = projectRelativePath.Remove(0, projectRootPath.Length).TrimStart('/', '\\');
                }

                ProjectRelativePath = UnityPath.FixPathSeparator(projectRelativePath);
            }


            private RecursionData(RecursionData data, string name)
            {
                FileSystemPath = $"{data.FileSystemPath}/{name}";
                StructPath = $"{data.StructPath}/{name}";
                ProjectRelativePath = $"{data.ProjectRelativePath}/{name}";
                IsEmbedded = data.IsEmbedded || name.EndsWith(".embeddedframework", StringComparison.OrdinalIgnoreCase);
                TargetType = data.TargetType;
            }


            public RecursionData Select(string path)
            {
                return new RecursionData(this, path);
            }
        }


        /// <summary>
        /// Adds all pbx-items located at specified path to Unity target of the project.
        /// </summary>
        /// <param name="path">A path to file or directory.</param>
        /// <param name="structPath">A path to group in xcode project structure. By default it equals to <paramref name="path"/>.</param>
        /// <param name="targetType">Xcode project target type.</param>
        public void AddItemsRecursively(
            string path, 
            string structPath = null,
            PbxProjectTargetType targetType = PbxProjectTargetType.Default)
        {
            RecursionData data = new RecursionData(path, XcodeProjectPath, 
                structPath ?? GetDefaultStructPath(path), targetType);

            if (Directory.Exists(path))
            {
                AddDirectoryItemsRecursively(data);
            }
            else if (File.Exists(path))
            {
                AddFileItem(data);
            }
        }


        private void AddDirectoryItemsRecursively(RecursionData data)
        {
            if (AddDirectoryItem(data))
            {
                return;
            }

            foreach (string filePath in Directory.EnumerateFiles(data.FileSystemPath))
            {
                string name = Path.GetFileName(filePath);
                AddFileItem(data.Select(name));
            }

            foreach (string dirPath in Directory.EnumerateDirectories(data.FileSystemPath))
            {
                string name = Path.GetFileName(dirPath);
                AddDirectoryItemsRecursively(data.Select(name));
            }
        }


        private bool AddDirectoryItem(RecursionData data)
        {
            if (data.ProjectRelativePath.EndsWith("__MACOSX", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".framework", StringComparison.OrdinalIgnoreCase))
            {
                AddFramework(data.ProjectRelativePath, data.StructPath, data.IsEmbedded, data.TargetType);
                return true;
            }
            
            if (data.ProjectRelativePath.EndsWith(".xcframework", StringComparison.OrdinalIgnoreCase))
            {
                AddFramework(data.ProjectRelativePath, data.StructPath, data.IsEmbedded, data.TargetType);
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".bundle", StringComparison.OrdinalIgnoreCase))
            {
                AddResource(data.ProjectRelativePath, data.StructPath, data.TargetType);
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".xcassets", StringComparison.OrdinalIgnoreCase))
            {
                AddResource(data.ProjectRelativePath, data.StructPath, data.TargetType);
                return true;
            }

            // return false to step into the folder
            return false;
        }


        private bool AddFileItem(RecursionData data)
        {
            if (data.ProjectRelativePath.EndsWith(".DS_Store", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".h", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".c", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".m", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".mm", StringComparison.OrdinalIgnoreCase))
            {
                AddCodeFile(data.ProjectRelativePath, data.StructPath, data.TargetType);
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".a", StringComparison.OrdinalIgnoreCase))
            {
                AddLib(data.ProjectRelativePath, data.StructPath, data.TargetType);
                return true;
            }

            if (data.ProjectRelativePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".xib", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".db", StringComparison.OrdinalIgnoreCase) ||
                data.ProjectRelativePath.EndsWith(".plist", StringComparison.OrdinalIgnoreCase))
            {
                AddResource(data.ProjectRelativePath, data.StructPath, data.TargetType);
                return true;
            }

            Debug.LogFormat("PbxProject: unknown type of file '{0}'. Skipped.", data.FileSystemPath);
            return false;
        }

        #endregion
        

        public void Save()
        {
            WriteToFile(OutputPath);
        }


        public void Reload()
        {
            ReadFromFile(OutputPath);
        }
    }
}
