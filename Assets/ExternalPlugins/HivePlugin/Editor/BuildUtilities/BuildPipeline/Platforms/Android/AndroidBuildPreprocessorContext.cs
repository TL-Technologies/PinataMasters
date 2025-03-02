using Modules.Hive.Editor.BuildUtilities.Android;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class AndroidBuildPreprocessorContext : BuildPreprocessorContext, IAndroidBuildPreprocessorContext
    {
        private AndroidLibrary sharedLibrary;


        /// <inheritdoc/>
        public GradleScript GradleScript { get; }


        /// <inheritdoc/>
        public AndroidManifest AndroidManifest { get; }


        /// <inheritdoc/>
        public AndroidManifest LauncherManifest { get; }


        /// <inheritdoc/>
        public PackageSignOptions PackageSignOptions { get; set; }


        /// <inheritdoc/>
        public AndroidLibrary SharedLibrary => sharedLibrary ?? (sharedLibrary = CreateTemporaryAndroidLibrary("shared"));


        protected CompositeDisposable Disposables { get; private set; }


        #region Instancing

        public AndroidBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives, 
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            PackageSignOptions signOptions) : 
            base(preprocessorDirectives, buildOptions, buildSettings)
        {
            GradleScript = CreateGradleScript();
            AndroidManifest = CreateAndroidManifest(ManifestType.Main);
            LauncherManifest = 
            #if UNITY_2019_3_OR_NEWER
                CreateAndroidManifest(ManifestType.Launcher);
            #else
                AndroidManifest;
            #endif
            
            PackageSignOptions = signOptions;

            // Keep disposable objects
            Disposables = new CompositeDisposable
            {
                GradleScript,
                AndroidManifest,
                LauncherManifest
            };
        }


        protected override void Disposing()
        {
            // Apply base-class settings first
            base.Disposing(); 

            // Create output directories if required
            CreateDirectoryForFile(AndroidManifest.OutputPath);
            CreateDirectoryForFile(LauncherManifest.OutputPath);
            CreateDirectory(GradleScript.OutputDirectoryPath);

            // Apply package sign options
            PackageSignOptions?.Apply();

            // Dispose all objects
            Disposables.Dispose();

            #if UNITY_2019_3_OR_NEWER
                ManuallyApplyTemplates();
            #endif
        }

        #endregion



        #region Android backup

        // Documentation: https://developer.android.com/guide/topics/data/autobackup?utm_campaign=autobackup-729&utm_medium=blog&utm_source=dac#configuring
        private bool? isAllowBackup = null;
        private BackupRulesResource backupRules = null;


        /// <inheritdoc/>
        public bool IsAllowBackup
        {
            get => isAllowBackup ?? true;
            set
            {
                if (isAllowBackup != null && isAllowBackup == value)
                {
                    return;
                }

                isAllowBackup = value;

                // Configure main manifest
                var element = AndroidManifest.GetApplicationElement();
                element.SetAttribute("allowBackup", AndroidManifest.AndroidNamespace, isAllowBackup.Value ? "true" : "false");

                // Update the manifest merge rules
                AndroidManifest.AddAttributesToToolsReplace(element, "android:allowBackup");
            }
        }


        /// <inheritdoc/>
        public BackupRulesResource BackupRulesIfExists => backupRules;


        /// <inheritdoc/>
        public BackupRulesResource BackupRules
        {
            get
            {
                if (backupRules == null)
                {
                    // Create xml resource (it will be disposed along with SharedLibrary)
                    backupRules = SharedLibrary.GetBackupRulesResource("hive_backup_rules.xml");

                    // Configure main manifest
                    var element = AndroidManifest.GetApplicationElement();
                    element.SetAttribute("fullBackupContent", AndroidManifest.AndroidNamespace, "@xml/hive_backup_rules");

                    // Update the manifest merge rules
                    AndroidManifest.AddAttributesToToolsReplace(element, "android:fullBackupContent");
                }

                return backupRules;
            }
        }

        #endregion



        #region Network security config

        private NetworkSecurityConfigResource networkSecurityConfig = null;

        /// <inheritdoc/>
        public NetworkSecurityConfigResource NetworkSecurityConfigIfExist => networkSecurityConfig;


        /// <inheritdoc/>
        public NetworkSecurityConfigResource NetworkSecurityConfig
        {
            get
            {
                if (networkSecurityConfig == null)
                {
                    // Create xml resource (it will be disposed along with SharedLibrary)
                    networkSecurityConfig = SharedLibrary.GetNetworkSecurityConfigResource("hive_network_security_config.xml");

                    // Configure main manifest
                    var element = AndroidManifest.GetApplicationElement();
                    element.SetAttribute("networkSecurityConfig", AndroidManifest.AndroidNamespace, "@xml/hive_network_security_config");
                    
                    // Update the manifest merge rules
                    AndroidManifest.AddAttributesToToolsReplace(element, "android:networkSecurityConfig");
                }

                return networkSecurityConfig;
            }
        }

        #endregion



        #region Other public tools

        /// <inheritdoc/>
        public AndroidLibrary CreateTemporaryAndroidLibrary(string name, string package = null)
        {
            if (string.IsNullOrEmpty(package))
            {
                package = $"{AndroidManifest.PackageRootName}.generated.{name}";
            }

            AndroidLibrary lib;
            string libraryPath = UnityPath.Combine("Assets/Plugins/Android", "hive-generated-" + name);
            #if UNITY_2020_3_OR_NEWER
                libraryPath += ".androidlib";
                lib = AndroidLibrary.Create(libraryPath, package);
                UnityPluginUtilities.SetPluginAssetEnabled(libraryPath, true, BuildTarget);
            #else
                lib = AndroidLibrary.Create(libraryPath, package);
            #endif
            Disposables.Add(lib);
            return lib;
        }

        #endregion



        #region Methods

        private void CreateDirectoryForFile(string path)
        {
            path = Path.GetDirectoryName(path);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        
        
        private void CreateDirectory(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }


        private GradleScript CreateGradleScript()
        {
            RestoreTemplatesFromBackup(GradleScriptUtilities.GradleTemplatesUnityDirectory);

            string projectDirectory = GradleScriptUtilities.ProjectTemplatesDirectoryPath;
            foreach (GradleType gradleType in GradleScriptUtilities.EnumerateGradleTemplateTypes())
            {
                string destinationPath = GradleScriptUtilities.GetProjectTemplatePath(gradleType);
                if (File.Exists(destinationPath))
                {
                    ProjectSnapshot.CurrentSnapshot?.CopyAssetToStash(UnityPath.GetAssetPathFromFullPath(destinationPath));
                }
                else
                {
                    string sourcePath = GradleScriptUtilities.GetHiveTemplatePath(gradleType);
                    if (File.Exists(sourcePath))
                    {
                        CreateDirectoryForFile(destinationPath);
                        File.Copy(sourcePath, destinationPath, true);
                    }
                }
            }
            
            return new GradleScript(projectDirectory, projectDirectory);
        }


        private AndroidManifest CreateAndroidManifest(ManifestType manifestType)
        {
            RestoreTemplatesFromBackup(AndroidManifestUtilities.AndroidManifestTemplatesUnityDirectory);
            
            string destination = AndroidManifestUtilities.GetProjectTemplatePath(manifestType);
            // Try to use manifest from project
            if (File.Exists(destination))
            {
                ProjectSnapshot.CurrentSnapshot?.CopyAssetToStash(UnityPath.GetAssetPathFromFullPath(destination));

                return AndroidManifest.Open(destination);
            }

            // Try to use manifest from Hive
            string source = AndroidManifestUtilities.GetHiveTemplatePath(manifestType);
            if (File.Exists(source))
            {
                CreateDirectoryForFile(destination);
                File.Copy(source, destination);

                return AndroidManifest.Open(destination);
            }

            // Try to use manifest from Unity
            source = AndroidManifestUtilities.GetUnityTemplatePath(manifestType);
            if (File.Exists(source))
            {
                CreateDirectoryForFile(destination);
                File.Copy(source, destination);

                return AndroidManifest.Open(destination);
            }

            // Create new manifest
            throw new Exception("Failed to create Android manifest due to missing template file.");
        }
        
        
        private static void RestoreTemplatesFromBackup(string templatesDirectoryPath)
        {
            #if UNITY_2019_3_OR_NEWER
                string backupDirectoryPath = UnityPath.Combine(templatesDirectoryPath, "Backup");
                if (Directory.Exists(backupDirectoryPath))
                {
                    DirectoryInfo directoryInfo = new DirectoryInfo(backupDirectoryPath);
                    foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles())
                    {
                        string filePath = UnityPath.Combine(templatesDirectoryPath, fileInfo.Name);
                        if (File.Exists(filePath))
                        {
                            continue;
                        }

                        File.Copy(fileInfo.FullName, filePath);
                    }
                }
                else
                {
                    Directory.CreateDirectory(backupDirectoryPath);
                    DirectoryInfo directoryInfo = new DirectoryInfo(templatesDirectoryPath);
                    foreach (FileInfo fileInfo in directoryInfo.EnumerateFiles())
                    {
                        File.Copy(fileInfo.FullName, UnityPath.Combine(backupDirectoryPath, fileInfo.Name));
                    }
                }
            #endif
        }
        
        
        private static void ManuallyApplyTemplates()
        {
            foreach (GradleType gradleType in GradleScriptUtilities.EnumerateGradleTemplateTypes())
            {
                if (gradleType == GradleType.Settings || gradleType == GradleType.Main)
                {
                    // Applied automatically by Unity
                    continue;
                }

                string sourcePath = GradleScriptUtilities.GetProjectTemplatePath(gradleType);
                if (File.Exists(sourcePath))
                {
                    string destinationPath = GradleScriptUtilities.GetUnityTemplatePath(gradleType);
                    File.Copy(sourcePath, destinationPath, true);
                }
            }

            foreach (ManifestType manifestType in AndroidManifestUtilities.EnumerateManifestTemplateTypes())
            {
                if (manifestType == ManifestType.Main)
                {
                    // Applied automatically by Unity
                    continue;
                }
                
                string sourcePath = AndroidManifestUtilities.GetProjectTemplatePath(manifestType);
                if (File.Exists(sourcePath))
                {
                    string destinationPath = AndroidManifestUtilities.GetUnityTemplatePath(manifestType);
                    File.Copy(sourcePath, destinationPath, true);
                }
            }
        }
        
        #endregion
    }
}
