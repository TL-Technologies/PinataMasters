using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace Modules.Hive.Editor
{
    /*
     * Typical plugin hierarchy
     *      PluginName (plugin root path)
     *      |-- Runtime
     *      |   |-- Modules.Plugin (main assembly)
     *      |   |-- Platforms (native plugins, templates, etc...)
     *      |       |-- Android
     *      |       |-- iOS
     *      |       |-- GooglePlay
     *      |       ...
     *      |-- Editor
     *      |   |-- Modules.Plugin.Editor (editor assembly)
     *      |-- README.md
     *      |-- CHANGELOG.md
     *      |-- LICENSE.md
     *      |-- package.json
     *      
     * */

    /// <summary>
    /// Describes a folder asset of typical plugin.
    /// </summary>
    public abstract class PluginHierarchy
    {
        #region Fields

        public const string PackageJsonFileName = "package.json";
        private static readonly string PlatformsDirectory = UnityPath.Combine("Runtime", "Platforms");

        #endregion



        #region Properties

        /// <summary>
        /// Returns null in case of usual git submodule.
        /// </summary>
        public PackageInfo PackageInfo { get; }
        
        public string RootPath { get; }
        public string RootAssetPath { get; }

        public string PlatformsPath => UnityPath.Combine(RootPath, PlatformsDirectory);
        public string PlatformsAssetPath => UnityPath.Combine(RootAssetPath, PlatformsDirectory);
        
        public string PackageJsonPath => UnityPath.Combine(RootPath, PackageJsonFileName);
        public string PackageJsonAssetPath => UnityPath.Combine(PlatformsAssetPath, PackageJsonFileName);

        #endregion



        #region Class lifecycle

        protected PluginHierarchy(string mainAssemblyName)
        {
            string path = PathByMainAssemblyName(mainAssemblyName);

            UnityPath.AssertPathLocatedInsideProject(path);

            RootAssetPath = path;
            RootPath = UnityPath.GetFullPathFromAssetPath(RootAssetPath);
            
            PackageInfo = PackageInfo.FindForAssetPath(RootAssetPath);
        }

        #endregion



        #region Methods

        // TODO: implement getters for README, CHANGELOG, LICENSE
        // https://github.com/kmindi/special-files-in-repository-root/blob/master/README.md

        public string GetPlatformPath(PlatformAlias platformAlias) =>
            UnityPath.Combine(PlatformsPath, platformAlias.DirectoryName);


        public string GetPlatformAssetPath(PlatformAlias platformAlias) =>
            UnityPath.Combine(PlatformsAssetPath, platformAlias.DirectoryName);


        public IEnumerable<UnityAssemblyDefinition> EnumerateUnityAssemblyDefinitions()
        {
            return AssetDatabase.FindAssets("t:asmdef")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Where(path => path.StartsWith(RootAssetPath))
                .Select(UnityAssemblyDefinition.OpenAtPath);
        }


        public UnityAssemblyDefinition[] GetUnityAssemblyDefinitions() =>
            EnumerateUnityAssemblyDefinitions().ToArray();


        protected static string PathByClassInRoot<T>()
        {
            string path = UnityPath.GetScriptAssetPath<T>();
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException(
                    $"The type '{TypeNameHelper.GetTypeDisplayName<T>()}' is not a module build-in class or is in a non-standard path.");
            }

            return UnityPath.GetDirectoryName(UnityPath.GetDirectoryName(path));
        }


        protected static string PathByMainAssemblyName(string mainAssemblyName)
        {
            string path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(mainAssemblyName);
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException($"Assembly '{mainAssemblyName}' is not exists!");
            }

            return UnityPath.GetDirectoryName(UnityPath.GetDirectoryName(path));
        }

        #endregion
    }
}
