using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class WindowsBuildPipelineContext : BuildPipelineContext
    {
        public WindowsBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_WINDOWS");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new WindowsBuildPreprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new WindowsBuildPostprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings, BuildProjectSummary);
    }
}
