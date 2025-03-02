using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using UnityEditor;
using UnityEngine;


namespace Modules.Max.Editor
{
    public class MaxAdaptersProcessor : IBuildPreprocessor<IIosBuildPreprocessorContext>,
        IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        #region Fields

        public const string XmlOutputPath = "MaxDependencies/Editor";
        public const string XmlFileName = "Dependencies.xml";

        public const string RootElementName = "dependencies";

        public const string AndroidRootElementName = "androidPackages";
        public const string AndroidElementName = "androidPackage";
        public const string RepositoriesRootElementName = "repositories";
        public const string RepositoryElementName = "repository";
        public const string IosRootElementName = "iosPods";
        public const string IosElementName = "iosPod";

        public const string SpecAttribute = "spec";
        public const string NameAttribute = "name";
        public const string VersionAttribute = "version";
        public const string SourcesAttribute = "sources";
        public const string SourceAttribute = "source";

        public const string AndroidPackagesProperty = "AndroidPackages";
        public const string IosPodsProperty = "IosPods";

        #endregion



        #region methods

        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            var result = CreateDependenciesXml(BuildTarget.Android);
            SaveDependenciesXml(result, context);
        }


        public void OnPreprocessBuild(IIosBuildPreprocessorContext context)
        {
            var result = CreateDependenciesXml(BuildTarget.iOS);
            SaveDependenciesXml(result, context);
        }


        public static string SaveDependenciesXml(XmlDocument document, IBuildPreprocessorContext context)
        {
            string outputDirectoryPath = UnityPath.GetFullPathFromAssetPath(
                UnityPath.Combine(UnityPath.ExternalPluginsSettingsAssetPath, XmlOutputPath));

            if (!Directory.Exists(outputDirectoryPath))
            {
                Directory.CreateDirectory(outputDirectoryPath);
            }

            string outputFilePath = UnityPath.Combine(outputDirectoryPath, XmlFileName);
            string outputFileAssetPath = UnityPath.GetAssetPathFromFullPath(outputFilePath);

            document.Save(outputFilePath);
            AssetDatabase.ImportAsset(outputFileAssetPath);
            context?.ProjectSnapshot.MarkAssetToDelete(outputFileAssetPath);

            return outputFileAssetPath;
        }


        private static XmlDocument CreateDependenciesXml(BuildTarget target)
        {
            if (!LLMaxSettings.DoesInstanceExist)
            {
                return null;
            }

            List<Type> adapters = CollectAdapters();

            XmlDocument document = new XmlDocument();
            XmlElement rootElement = document.CreateElement(RootElementName);
            XmlElement platformRoot =
                document.CreateElement(target == BuildTarget.Android ? AndroidRootElementName : IosRootElementName);

            document.AppendChild(rootElement);
            rootElement.AppendChild(platformRoot);
            
            List<string> enabledAdapters = new List<string>(LLMaxSettings.Instance.EnabledAdapters);
            enabledAdapters.Add(nameof(AppLovinAdapter));

            foreach (var adapterName in enabledAdapters)
            {
                var adapterType = adapters.First(a => a.Name == adapterName);
                if (adapterType == null)
                {
                    Debug.LogWarning($"[ApplovinMax] Unknown adapter {adapterName}");
                    continue;
                }

                var adapter = Activator.CreateInstance(adapterType);

                if (target == BuildTarget.Android)
                {
                    var field = adapterType.GetProperty(AndroidPackagesProperty);
                    var dependencies = (List<MaxAdapter.AndroidPackage>) field.GetValue(adapter);
                    foreach (var dependency in dependencies)
                    {
                        XmlElement packageElement = CreateAndroidDependency(dependency, document);
                        platformRoot.AppendChild(packageElement);
                    }
                }
                else
                {
                    var field = adapterType.GetProperty(IosPodsProperty);
                    var dependencies = (List<MaxAdapter.IosPod>) field.GetValue(adapter);
                    foreach (var dependency in dependencies)
                    {
                        XmlElement podElement = CreateIosDependency(dependency, document);
                        platformRoot.AppendChild(podElement);
                    }
                }
            }

            return document;
        }


        private static XmlElement CreateAndroidDependency(MaxAdapter.AndroidPackage dependency, XmlDocument document)
        {
            XmlElement packageElement = document.CreateElement(AndroidElementName);
            packageElement.SetAttribute(SpecAttribute, dependency.Spec);

            if (!string.IsNullOrEmpty(dependency.Repository))
            {
                XmlElement repositoriesRoot = document.CreateElement(RepositoriesRootElementName);
                XmlElement repository = document.CreateElement(RepositoryElementName);
                repository.InnerText = dependency.Repository;
                repositoriesRoot.AppendChild(repository);
                packageElement.AppendChild(repositoriesRoot);
            }

            return packageElement;
        }


        private static XmlElement CreateIosDependency(MaxAdapter.IosPod dependency, XmlDocument document)
        {
            XmlElement podElement = document.CreateElement(IosElementName);
            podElement.SetAttribute(NameAttribute, dependency.Name);
            podElement.SetAttribute(VersionAttribute, dependency.Version);

            if (!string.IsNullOrEmpty(dependency.Source))
            {
                XmlElement sourcesRoot = document.CreateElement(SourcesAttribute);
                XmlElement source = document.CreateElement(SourceAttribute);
                source.InnerText = dependency.Source;
                sourcesRoot.AppendChild(source);
                podElement.AppendChild(sourcesRoot);
            }
            
            return podElement;
        }


        private static List<Type> CollectAdapters()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(wh => wh.IsSubclassOf(typeof(MaxAdapter)))
                .ToList();
        }
        
        #endregion
    }
}
