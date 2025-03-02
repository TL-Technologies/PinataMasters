using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Reflection;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Xml;
using UnityEditor;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;


namespace Modules.Hive.Editor.BuildUtilities
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.Internal)]
    internal class CommonBuildProcessor : IBuildPreprocessor<IBuildPreprocessorContext>
    {
//        private const string PackagesNamePrefix = "com.playgendary";
//        private const string LinkXmlFileName = "link.xml";
        
        
        public void OnPreprocessBuild(IBuildPreprocessorContext context)
        {
            // https://docs.unity3d.com/Manual/ManagedCodeStripping.html
            // link.xml files do not work in packages, that's why we should add them
            // explicitly to project for preventing unnecessary and harmful stripping during build
            
//            XmlDocument result = new XmlDocument();
//            XmlElement rootElement = result.CreateElement("linker");
//            result.AppendChild(rootElement);
//            
//            Func<PackageInfo[]> packageInfoDelegate = ReflectionHelper.CreateDelegateToMethod<Func<PackageInfo[]>>(
//                typeof(PackageInfo),
//                "GetAll",
//                BindingFlags.NonPublic | BindingFlags.Static,
//                true);
//            PackageInfo[] packageInfos = packageInfoDelegate();
//            foreach (PackageInfo packageInfo in packageInfos)
//            {
//                rootElement.AppendChild(result.CreateComment($"Preserving for {packageInfo.name}"));
//                
//                if (packageInfo.name.StartsWith(PackagesNamePrefix))
//                {
//                    // Find all runtime asmdefs
//                    foreach (FileInfo fileInfo in FileSystemUtilities.EnumerateFiles(
//                        UnityPath.Combine(packageInfo.resolvedPath, "Runtime"),
//                        "*.asmdef",
//                        SearchOption.AllDirectories))
//                    {
//                        string asmdefContent = File.ReadAllText(fileInfo.FullName);
//                        UnityAssemblyDefinition assemblyDefinition = JsonConvert.DeserializeObject<UnityAssemblyDefinition>(asmdefContent);
//                        
//                        XmlElement assemblyElement = result.CreateElement("assembly");
//                        assemblyElement.SetAttribute("fullname", assemblyDefinition.Name);
//                        assemblyElement.SetAttribute("preserve", "all");
//                        rootElement.AppendChild(assemblyElement);
//                    }
//                }
//                
//                // File all link.xml files
//                foreach (FileInfo fileInfo in FileSystemUtilities.EnumerateFiles(
//                    packageInfo.resolvedPath,
//                    LinkXmlFileName,
//                    SearchOption.AllDirectories))
//                {
//                    XmlDocument linkDocument = new XmlDocument();
//                    linkDocument.Load(fileInfo.FullName);
//                        
//                    XmlNodeList preserveSettings = linkDocument.DocumentElement.ChildNodes;
//                    foreach (XmlNode node in preserveSettings)
//                    {
//                        rootElement.AppendChild(result.ImportNode(node, true));
//                    }
//                }
//            }
//            
//            string outputDirectoryPath = UnityPath.GetFullPathFromAssetPath(
//                UnityPath.Combine(UnityPath.ExternalPluginsSettingsAssetPath, "HivePlugin"));
//            string outputFilePath = UnityPath.Combine(outputDirectoryPath, LinkXmlFileName);
//            string outputFileAssetPath = UnityPath.GetAssetPathFromFullPath(outputFilePath);
//
//            FileSystemUtilities.Delete(outputFilePath);
//            if (!Directory.Exists(outputDirectoryPath))
//            {
//                Directory.CreateDirectory(outputDirectoryPath);
//            }
//            result.Save(File.Create(outputFilePath));
//            AssetDatabase.ImportAsset(outputFileAssetPath);
//            
//            context.ProjectSnapshot.MarkAssetToDelete(outputFileAssetPath);
        }
    }
}
