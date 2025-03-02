using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class WebGlBuildPipelineContext : BuildPipelineContext
    {
        public WebGlBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_WEBGL");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new WebGlBuildPreprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new WebGlBuildPostprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings, BuildProjectSummary);
    }
}
