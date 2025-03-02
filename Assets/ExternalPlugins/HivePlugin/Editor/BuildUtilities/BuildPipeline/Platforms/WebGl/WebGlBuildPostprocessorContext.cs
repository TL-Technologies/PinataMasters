using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class WebGlBuildPostprocessorContext : BuildPostprocessorContext, IWebGlBuildPostprocessorContext
    {
        public WebGlBuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) : 
            base(preprocessorDirectives, buildOptions, buildSettings, buildProjectSummary) { }
    }
}
