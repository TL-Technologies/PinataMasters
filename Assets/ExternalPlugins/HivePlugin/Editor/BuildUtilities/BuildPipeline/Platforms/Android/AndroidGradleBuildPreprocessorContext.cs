using Modules.Hive.Editor.BuildUtilities.Android;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class AndroidGradleBuildPreprocessorContext : BuildProcessorContext, IGradleBuildPreprocessorContext
    {
        private string exportAndroidProjectPath;
        
        
        /// <inheritdoc/>
        public GradleProperties GradleProperties { get; }


        /// <inheritdoc/>
        public string GradleProjectPath { get; }


        protected CompositeDisposable Disposables { get; private set; }


        public AndroidGradleBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            string gradleProjectPath) : 
            base(preprocessorDirectives, buildOptions, buildSettings)
        {
            if (BuildSettings.ContainsKey(BuildSetting.ExportAndroidProjectPath))
            {
                exportAndroidProjectPath = BuildSettings[BuildSetting.ExportAndroidProjectPath];
            }
            
            GradleProjectPath = gradleProjectPath;

            // Open or create gradle properties file
            string gradlePropertiesPath = UnityPath.Combine(
                #if UNITY_2019_3_OR_NEWER
                    UnityPath.GetDirectoryName(GradleProjectPath), 
                #else
                    GradleProjectPath, 
                #endif
                GradleProperties.DefaultFileName);
            
            GradleProperties = File.Exists(gradlePropertiesPath) ?
                GradleProperties.Open(gradlePropertiesPath) :
                GradleProperties.Create(gradlePropertiesPath);

            Disposables = new CompositeDisposable
            {
                GradleProperties
            };
        }


        protected override void Disposing()
        {
            // Archive and save Android project as build artifact
            if (!string.IsNullOrEmpty(exportAndroidProjectPath))
            {
                string gradleProjectRootPath = UnityPath.Combine(
                    UnityPath.ProjectPath, 
                    UnityPath.GetDirectoryName(GradleProjectPath));
                string gradleProjectRenamedPath = UnityPath.Combine(
                    UnityPath.GetDirectoryName(gradleProjectRootPath),
                    $"{Path.GetFileNameWithoutExtension(BuildOptions.locationPathName)}AndroidProject");
                
                FileSystemUtilities.Move(gradleProjectRootPath, gradleProjectRenamedPath);
                FileSystemUtilities.CreateArchive(
                    gradleProjectRenamedPath, 
                    exportAndroidProjectPath);
                FileSystemUtilities.Move(gradleProjectRenamedPath, gradleProjectRootPath);
            }
            
            // Save gradle files and other Android project settings as build artifact
            if (IsCiBuild)
            {
                string resultArchiveFileName = $"{Path.GetFileNameWithoutExtension(BuildOptions.locationPathName)}GradleInfo";
                string gradleProjectRootPath = UnityPath.Combine(
                    UnityPath.ProjectPath, 
                    UnityPath.GetDirectoryName(GradleProjectPath));
                string tempDirectoryPath = UnityPath.Combine(
                    UnityPath.ProjectTempPath, 
                    resultArchiveFileName);
                
                if (Directory.Exists(tempDirectoryPath))
                {
                    Directory.Delete(tempDirectoryPath, true);
                }
                Directory.CreateDirectory(tempDirectoryPath);

                foreach (FileInfo fileInfo in FileSystemUtilities.EnumerateFiles(
                    gradleProjectRootPath, 
                    SearchOption.TopDirectoryOnly)
                    .Where(info => info.Name.EndsWith("properties") || info.Name.EndsWith("gradle")))
                {
                    FileSystemUtilities.Copy(
                        fileInfo.FullName, 
                        UnityPath.Combine(tempDirectoryPath, fileInfo.Name));
                }
                
                FileSystemUtilities.Copy(
                    UnityPath.Combine(gradleProjectRootPath, "launcher", "build.gradle"),
                    UnityPath.Combine(tempDirectoryPath, "launcher_build.gradle"));
                FileSystemUtilities.Copy(
                    UnityPath.Combine(gradleProjectRootPath, "unityLibrary", "build.gradle"),
                    UnityPath.Combine(tempDirectoryPath, "unityLibrary_build.gradle"));
                
                string resultArchivePath = UnityPath.Combine(
                    UnityPath.GetDirectoryName(BuildOptions.locationPathName),
                    $"{resultArchiveFileName}.zip");
                FileSystemUtilities.CreateArchive(tempDirectoryPath, resultArchivePath);
                
                Directory.Delete(tempDirectoryPath, true);
            }
            
            // Dispose all objects
            Disposables.Dispose();

            base.Disposing();
        }
    }
}
