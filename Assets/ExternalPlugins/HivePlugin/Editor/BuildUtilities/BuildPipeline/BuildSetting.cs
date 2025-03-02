using Newtonsoft.Json;
using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities
{
    public readonly struct BuildSetting
    {
        public static BuildSetting AppBundleNecessity => new BuildSetting("appBundle");
        public static BuildSetting BuildNumber => new BuildSetting("buildNumber");
        public static BuildSetting CiBuildFlag => new BuildSetting("ciBuild");
        public static BuildSetting ExportAndroidProjectPath => new BuildSetting("exportAndroidProjectPath");
        public static BuildSetting SymbolsFileNecessity => new BuildSetting("createSymbolsFile");
        public static BuildSetting EnableCrashReportAPI => new BuildSetting("enableCrashReportAPI");

        public static BuildSetting PipelineName => new BuildSetting("pipelineName");

        /// <summary>
        /// Gets build setting name.
        /// </summary>
        [JsonProperty] public string Name { get; }
        
        
        [JsonConstructor]
        private BuildSetting(string name)
        {
            Name = name;
        }
    }
    
    
    public class BuildSettingComparer : IEqualityComparer<BuildSetting>
    {
        public bool Equals(BuildSetting x, BuildSetting y)
        {
            return x.Name.Equals(y.Name, StringComparison.InvariantCulture);
        }


        public int GetHashCode(BuildSetting obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}
