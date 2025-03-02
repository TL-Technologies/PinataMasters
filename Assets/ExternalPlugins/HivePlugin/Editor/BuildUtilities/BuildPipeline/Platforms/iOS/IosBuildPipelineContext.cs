using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class IosBuildPipelineContext : BuildPipelineContext
    {
        // Platform-depended options


        public IosBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_IOS");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() => 
            new IosBuildPreprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() => 
            new IosBuildPostprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings, BuildProjectSummary);
    }
}
