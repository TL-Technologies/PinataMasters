using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class MacOsBuildPostprocessorContext : BuildPostprocessorContext, IMacOsBuildPostprocessorContext
    {
        public MacOsBuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) :
            base(preprocessorDirectives, buildOptions, buildSettings, buildProjectSummary) { }
    }
}
