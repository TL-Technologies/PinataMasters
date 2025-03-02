using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Ios.Stub
{
    public class PBXProject
    {
        public static string GetPBXProjectPath(string buildPath) { return null; }

        public void ReadFromFile(string path) { }

        public void WriteToFile(string path) { }

        public void AddFrameworkToProject(string targetGuid, string framework, bool weak) { }

        public string AddFile(string path, string projectPath, PBXSourceTree sourceTree = PBXSourceTree.Source) { return null; }

        public void AddFileToBuild(string targetGuid, string fileGuid) { }

        public void AddBuildProperty(string targetGuid, string name, string value) { }

        public void SetBuildProperty(string targetGuid, string name, string value) { }

        public string AddShellScriptBuildPhase(string targetGuid, string name, string shellPath, string shellScript) { return null; }

        public string GetUnityFrameworkTargetGuid() { return null; }

        public string GetUnityMainTargetGuid() { return null; }

        public string TargetGuidByName(string name) { return null; }

        public static string GetUnityTestTargetName() { return null; }

        public void AddBuildPropertyForConfig(string configGuid, string name, string value) { }

        public string AddTarget(string name, string ext, string type) { return null; }

        public IEnumerable<string> BuildConfigNames() { return null; }

        public string BuildConfigByName(string targetGuid, string name) { return null; }

        public void SetBuildPropertyForConfig(string configGuid, string name, string value) { }

        public string AddResourcesBuildPhase(string targetGuid) { return null; }

        public string AddCopyFilesBuildPhase(string targetGuid, string name, string dstPath, string subfolderSpec) { return null; }

        public void AddFileToBuildSection(string targetGuid, string sectionGuid, string fileGuid) { }

        public string GetTargetProductFileRef(string targetGuid) { return null; }

        public void AddTargetDependency(string targetGuid, string targetDependencyGuid) { }

        public void AddFileToEmbedFrameworks(string targetGuid, string fileGuid) { }
    }
}