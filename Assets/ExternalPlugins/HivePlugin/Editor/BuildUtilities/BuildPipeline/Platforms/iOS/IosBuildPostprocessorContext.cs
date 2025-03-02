using Modules.Hive.Editor.BuildUtilities.Ios;
using Modules.Hive.Reflection;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class IosBuildPostprocessorContext : BuildPostprocessorContext, IIosBuildPostprocessorContext
    {
        /// <inheritdoc/>
        public PbxProject PbxProject { get; }

        /// <inheritdoc/>
        public InfoPlist InfoPlist { get; }

        /// <inheritdoc/>
        public EntitlementsPlist Entitlements { get; }

        protected CompositeDisposable Disposables { get; private set; }


        #region Instancing

        public IosBuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) : 
            base(preprocessorDirectives, buildOptions, buildSettings, buildProjectSummary)
        {
            PbxProject = PbxProject.OpenInFolder(buildProjectSummary.outputPath);
            InfoPlist = InfoPlist.OpenInFolder(buildProjectSummary.outputPath);
            Entitlements = CreateEntitlements(PbxProject, buildProjectSummary.outputPath);

            Disposables = new CompositeDisposable
            {
                PbxProject,
                InfoPlist,
                Entitlements
            };
        }


        protected override void Disposing()
        {
            // Apply base-class settings first
            base.Disposing();
            
            if (!IsCiBuild || !BuildSettings.ContainsKey(BuildSetting.SymbolsFileNecessity))
            {
                // Fix for symbols upload crash after building Xcode project using Unity account without crash reports permissions
                string processSymbolsPath = Path.Combine(BuildPath, "process_symbols.sh");
                if (File.Exists(processSymbolsPath))
                {
                    File.WriteAllText(processSymbolsPath, "#!/bin/sh\n");
                }
            }
            
            // Dispose all objects
            Disposables.Dispose();
        }


        private EntitlementsPlist CreateEntitlements(PbxProject pbxProject, string buildPath)
        {
            EntitlementsPlist entitlements = null;
            string entitlementsRelativePath = null;

            string[] files = Directory.GetFiles(buildPath, "*.entitlements", SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
            {
                // Create new entitlements
                entitlements = EntitlementsPlist.CreateInFolder(buildPath);
                entitlementsRelativePath = Path.GetFileName(entitlements.OutputPath);
            }
            else
            {
                // Open current entitlements
                entitlements = EntitlementsPlist.Open(files[0]);
                entitlementsRelativePath = Path.GetFileName(files[0]);

                if (files.Length > 1)
                {
                    Debug.LogWarning($"There are many .entitlements files in the project. '{entitlementsRelativePath}' will be used.");
                }
            }

            // Override reference to entitlements in the project
            SetEntitlementsFile(entitlementsRelativePath);
            pbxProject.AddFile(entitlementsRelativePath, entitlementsRelativePath, PBXSourceTree.Source);
            pbxProject.SetBuildProperty(
                "CODE_SIGN_ENTITLEMENTS", 
                UnityPath.FixPathSeparator(entitlementsRelativePath), 
                PbxProjectTargetType.Main);
            return entitlements;
            
            
            void SetEntitlementsFile(string entitlementsPath)
            {
                object pbxProjectSection = ReflectionHelper.GetPropertyValue<object>(
                    pbxProject.GetType(),
                    pbxProject,
                    "project");
                object projectObjectData = ReflectionHelper.GetPropertyValue<object>(
                    pbxProjectSection.GetType(),
                    pbxProjectSection,
                    "project");
                ReflectionHelper.SetFieldValue(
                    projectObjectData.GetType(),
                    projectObjectData,
                    "entitlementsFile",
                    entitlementsPath);
            }
        }

        #endregion


        /// <inheritdoc/>
        public string GetDestinationPath(string moduleName)
        {
            if (string.IsNullOrWhiteSpace(moduleName))
            {
                throw new ArgumentException("module name should not be empty", nameof(moduleName));
            }
            if (string.IsNullOrWhiteSpace(BuildPath))
            {
                throw new InvalidOperationException("Xcode project doesn't exist");
            }

            return $"{BuildPath}/Hive/{moduleName}";
        }
    }
}
