using Modules.Hive.Editor.BuildUtilities.Android;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class AndroidBuildPipelineContext : BuildPipelineContext
    {
        public PackageSignOptions PackageSignOptions { get; set; }


        public AndroidBuildPipelineContext(BuildPlayerOptions buildOptions) : base(buildOptions)
        {
            PreprocessorDirectives.Add("HIVE_ANDROID");
        }


        protected internal override IBuildPreprocessorContext CreateBuildPreprocessorContext() =>
            new AndroidBuildPreprocessorContext(
                PreprocessorDirectives, 
                BuildOptions,
                BuildSettings,
                PackageSignOptions);


        protected internal override IBuildPostprocessorContext CreateBuildPostprocessorContext() =>
            new AndroidBuildPostprocessorContext(
                PreprocessorDirectives, 
                BuildOptions,
                BuildSettings,
                BuildProjectSummary);


        protected internal virtual IGradleBuildPreprocessorContext CreateGradleBuildPreprocessorContext(string gradleProjectPath) =>
            new AndroidGradleBuildPreprocessorContext(
                PreprocessorDirectives,
                BuildOptions,
                BuildSettings,
                gradleProjectPath);
    }
}
