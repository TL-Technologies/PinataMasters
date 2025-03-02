using Modules.Hive.Editor.Pipeline;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public abstract class BuildPipelineContext : IBuildProjectTaskContext, IPreprocessorDirectivesTaskContext
    {
        /// <summary>
        /// Gets a build options of Unity Build pipeline
        /// </summary>
        public BuildPlayerOptions BuildOptions { get; set; }
        
        
        /// <summary>
        /// Gets whether build was started manually or by CI
        /// </summary>
        public bool IsCiBuild => BuildSettings.ContainsKey(BuildSetting.CiBuildFlag);
        
        
        /// <summary>
        /// Gets custom build settings.
        /// </summary>
        public Dictionary<BuildSetting, string> BuildSettings { get; set; }

       
        /// <inheritdoc/>
        public HiveBuildSummary BuildProjectSummary { get; set; }


        /// <inheritdoc/>
        public PreprocessorDirectivesCollection PreprocessorDirectives { get; }
        

        public EnvironmentParameters Env { get; }

        [JsonConstructor]
        public BuildPipelineContext(BuildPlayerOptions buildOptions)
        {
            BuildOptions = buildOptions;
            PreprocessorDirectives = new PreprocessorDirectivesCollection { "HIVE" };
            BuildSettings = new Dictionary<BuildSetting, string>(new BuildSettingComparer());
        }


        /// <summary>
        /// Creates a platform-depended context for <see cref="IBuildPreprocessor{T}"/>.
        /// </summary>
        /// <returns>An instance of context.</returns>
        protected internal abstract IBuildPreprocessorContext CreateBuildPreprocessorContext();


        /// <summary>
        /// Creates a platform-depended context for <see cref="IBuildPostprocessor{T}"/>.
        /// </summary>
        /// <returns>An instance of context.</returns>
        protected internal abstract IBuildPostprocessorContext CreateBuildPostprocessorContext();
    }
}
