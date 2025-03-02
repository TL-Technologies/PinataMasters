using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal abstract class BuildPostprocessorContext : BuildProcessorContext, IBuildPostprocessorContext
    {
        /// <inheritdoc/>
        public HiveBuildSummary BuildProjectSummary { get; set; }


        /// <inheritdoc/>
        public string BuildPath => BuildProjectSummary.outputPath;


        public BuildPostprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            HiveBuildSummary buildProjectSummary) : 
            base(preprocessorDirectives, buildOptions, buildSettings)
        {
            BuildProjectSummary = buildProjectSummary;
        }
    }
}
