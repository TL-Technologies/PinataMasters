using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class MacOsBuildPipelineContext : BuildPipelineContext
    {
        public MacOsBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_MACOS");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new MacOsBuildPreprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new MacOsBuildPostprocessorContext(PreprocessorDirectives, BuildOptions, BuildSettings, BuildProjectSummary);
    }
}
