using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleScriptUtilities
    {
        #region Fields

        public static readonly string GradleTemplatesUnityDirectory =
            HivePluginHierarchy.Instance.GetEditorPlatformTemplatesAssetPath(PlatformAlias.Android);
        private static readonly string HiveTemplatesDirectoryPath = UnityPath.GetFullPathFromAssetPath(HivePluginHierarchy.Instance.GetPlatformTemplatesAssetPath(PlatformAlias.Android));
        public static readonly string ProjectTemplatesDirectoryPath = UnityPath.GetFullPathFromAssetPath(UnityPath.AndroidPluginsAssetPath);
        
        private const string GradlePackageIdentifier = "com.android.tools.build:gradle:";
        private const GradleType GradlePackageIdentifierType = GradleType.BaseProject;
        
        private static readonly Dictionary<GradleType, string> GradleFileNameByType = new Dictionary<GradleType, string>()
        {
            { GradleType.BaseProject, "baseProjectTemplate.gradle" },
            { GradleType.Launcher, "launcherTemplate.gradle" },
            { GradleType.Lib, "libTemplate.gradle" },
            { GradleType.Main, "mainTemplate.gradle" },
            { GradleType.Settings, "settingsTemplate.gradle" }
        };
        
        #endregion
        
        
        
        #region Methods

        public static IEnumerable<GradleType> EnumerateGradleTemplateTypes()
        {
            return GradleFileNameByType.Keys;
        }
        
        
        public static string GetGradleTemplateName(GradleType gradleType)
        {
            if (!GradleFileNameByType.TryGetValue(gradleType, out string result))
            {
                throw new ArgumentException($"Can't get template name for gradle type {gradleType}!");
            }
            
            return result;
        }
        
        
        /// <summary>
        /// Gets path to existed gradle template of the project.
        /// </summary>
        public static string GetProjectTemplatePath(GradleType gradleType) => 
            UnityPath.Combine(
                ProjectTemplatesDirectoryPath, 
                GetGradleTemplateName(gradleType));
        
        
        /// <summary>
        /// Gets path to existed gradle template of Hive plugin.
        /// </summary>
        public static string GetHiveTemplatePath(GradleType gradleType) => 
            UnityPath.Combine(
                HiveTemplatesDirectoryPath, 
                GetGradleTemplateName(gradleType));
        
        
        /// <summary>
        /// Get path to gradle template that Unity uses by default.
        /// </summary>
        public static string GetUnityTemplatePath(GradleType gradleType) =>
            UnityPath.Combine(
                GradleTemplatesUnityDirectory,
                GetGradleTemplateName(gradleType));
        

        /// <summary>
        /// Gets a dependency to gradle package from template in directory.
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static GradleDependency GetGradlePackageDependency(string directoryPath)
        {
            GradleDependency result = null;
            
            if (GradleFileNameByType.TryGetValue(GradlePackageIdentifierType, out string templateName))
            {
                string filePath = UnityPath.Combine(directoryPath, templateName);
                if (File.Exists(filePath))
                {
                    using (StreamReader stream = File.OpenText(filePath))
                    {
                        while (true)
                        {
                            string line = stream.ReadLine();
                            if (line == null)
                            {
                                break;
                            }

                            if (line.Contains(GradlePackageIdentifier))
                            {
                                var reference = line
                                    .Split(new[] { ' ', '\'', '"' }, StringSplitOptions.RemoveEmptyEntries)
                                    .FirstOrDefault(p => p.Contains(GradlePackageIdentifier));

                                result = new GradleDependency(reference, GradleDependencyType.ClassPath);
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new ArgumentException("Can't find template name with gradle package dependency!");
            }
            
            return result;
        }
        
        #endregion
    }
}
