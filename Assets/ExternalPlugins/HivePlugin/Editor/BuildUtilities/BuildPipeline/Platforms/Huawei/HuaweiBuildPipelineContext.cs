using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class HuaweiBuildPipelineContext : AndroidBuildPipelineContext
    {
        public HuaweiBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_HUAWEI");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new HuaweiBuildPreprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                PackageSignOptions);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new HuaweiBuildPostprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                BuildProjectSummary);
    }
}
