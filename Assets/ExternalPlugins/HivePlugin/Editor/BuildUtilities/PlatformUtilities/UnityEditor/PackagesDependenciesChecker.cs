using Modules.Hive.Reflection;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class PackagesDependenciesChecker
    {
        #region Nested type

        private class ModuleInfo
        {
            #region Fields

            private string name;
            private string version;
            private Dictionary<string, string> dependencies = new Dictionary<string, string>();

            #endregion

        
        
            #region Properties

            public string Name => name;

            public string Version => version;

            public Dictionary<string, string> Dependencies => dependencies;

            #endregion

        
        
            #region Properties
        
            public ModuleInfo(string name, string version, Dictionary<string, string> dependencies)
            {
                this.name = name;
                this.version = version;
                this.dependencies = dependencies;
            }
        
            #endregion
        }

        #endregion
       
        
        
        #region Fields

        private const string PackagesNamePrefix = "com.playgendary";
        private const string PackageJsonFileName = "package.json";
        private const string SubmodulesAssetPathRegex = @"^\s*path ?= ?(.*)\s*$";
        private readonly string GitModulesFilePath = UnityPath.Combine(UnityPath.ProjectPath, ".gitmodules");
        
        private Dictionary<string, string> allModulesInUse = new Dictionary<string, string>();
        private List<ModuleInfo> pluginsInfo = new List<ModuleInfo>();
        
        #endregion

        
        
        #region Public methods

        public bool HasUnresolvedDependencies()
        {
            bool hadIssue = false;

            allModulesInUse = GetCurrentPluginsVersions();

            foreach (var plugin in pluginsInfo)
            {
                if (plugin.Dependencies == null || plugin.Dependencies.Count < 1) continue;
                
                bool hasIssue = plugin.Dependencies.Any(dependencyPlugin =>
                    HasIssuesOnPlugin(plugin.Name, 
                        dependencyPlugin.Key, 
                        dependencyPlugin.Value));

                hadIssue = hadIssue || hasIssue;
            }
            
            return hadIssue;
        }
        
        #endregion

        

        #region Private methods

        private bool HasIssuesOnPlugin(string currentPlugin, string dependentPluginName, string dependentPluginVersion)
        {
            if (allModulesInUse.TryGetValue(dependentPluginName, out var pluginInstalledVersion))
            {
                if (!IsDependencyCompleted(dependentPluginVersion, pluginInstalledVersion))
                {
                    Debug.LogError(
                        $"Hive build error: Plugin {currentPlugin} requires {dependentPluginName} version " +
                        $"{dependentPluginVersion} or higher. Your installed version is " +
                        $"{pluginInstalledVersion}. Please upgrade!");
                    return true;
                }

                return false;
            }

            Debug.LogError(
                $"Hive build error: Plugin {currentPlugin} requires {dependentPluginName} version " +
                $"{dependentPluginVersion}. Please don`t forget to install it!");
            return true;
        }
        
        
        private Dictionary<string, string> GetCurrentPluginsVersions()
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            FindAllSubmodules(result);
            FindAllPackages(result);

            return result;
        }


        private void FindAllSubmodules(Dictionary<string, string> result)
        {
            if (!File.Exists(GitModulesFilePath)) return;
            
            string gitModulesFileContent;
            using (StreamReader streamReader = File.OpenText(GitModulesFilePath))
            {
                gitModulesFileContent = streamReader.ReadToEnd();
            }
                
            MatchCollection matches = Regex.Matches(
                gitModulesFileContent,
                SubmodulesAssetPathRegex,
                RegexOptions.Multiline);
            
            foreach (Match match in matches)
            {
                if (!match.Success) continue;
                AddPluginInfoToDictionary(
                    result,
                    UnityPath.Combine(match.Groups[1].Value, PackageJsonFileName));
            }
        }
        
        
        private void FindAllPackages(Dictionary<string, string> result)
        {
            Func<PackageInfo[]> packageInfoDelegate = ReflectionHelper.CreateDelegateToMethod<Func<PackageInfo[]>>(
                typeof(PackageInfo),
                "GetAll",
                BindingFlags.NonPublic | BindingFlags.Static,
                true);
            
            PackageInfo[] packageInfos = packageInfoDelegate();
            packageInfos.ToList().ForEach(package => AddPluginInfoToDictionary(
                result,
                UnityPath.Combine(package.resolvedPath, PackageJsonFileName)));

        }
        
        
        private void AddPluginInfoToDictionary(IDictionary<string, string> dictionary, string packageJsonPath)
        {
            string trimmedPath = String.Concat(packageJsonPath.Where(c => !Char.IsWhiteSpace(c)));

            UnityPackageInfo packageInfo = UnityPackageInfo.Open(trimmedPath);

            if (packageInfo == null) return;

            string pluginName = packageInfo.name;
            string pluginVersion = packageInfo.version;
            if (packageInfo.name.StartsWith(PackagesNamePrefix))
            {
                pluginsInfo.Add(new ModuleInfo(pluginName, pluginVersion, packageInfo.dependencies));
            }
            dictionary.Add(pluginName, pluginVersion);
        }

        
        private bool IsDependencyCompleted(string dependencyMinVersion, string currentVersion)
        {
            dependencyMinVersion = ClearPreviewIfHas(dependencyMinVersion);
            currentVersion = ClearPreviewIfHas(currentVersion);
            
            var minVersion = new Version(dependencyMinVersion);
            var ourVersion = new Version(currentVersion);
            var result = minVersion.CompareTo(ourVersion);

            return result <= 0;
        }
        
        
        private string ClearPreviewIfHas(string version)
        {
            int index = version.IndexOf("-");
            if (index >= 0)
            {
                version = version.Substring(0, index);
            }

            return version;
        }

        #endregion
    }
}