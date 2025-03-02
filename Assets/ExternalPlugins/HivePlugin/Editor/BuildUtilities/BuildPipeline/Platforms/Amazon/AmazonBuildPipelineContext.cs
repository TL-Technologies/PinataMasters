using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class AmazonBuildPipelineContext : AndroidBuildPipelineContext
    {
        public AmazonBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_AMAZON");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new AmazonBuildPreprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                PackageSignOptions);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new AmazonBuildPostprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                BuildProjectSummary);
    }
}
