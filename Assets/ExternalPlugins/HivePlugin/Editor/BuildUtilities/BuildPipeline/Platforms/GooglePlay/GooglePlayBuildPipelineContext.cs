using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class GooglePlayBuildPipelineContext : AndroidBuildPipelineContext
    {
        // Platform-depended options


        public GooglePlayBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_GOOGLEPLAY");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new GooglePlayBuildPreprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                PackageSignOptions);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new GooglePlayBuildPostprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                BuildProjectSummary);
    }
}
