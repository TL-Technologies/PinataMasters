namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPostprocessorContext : IBuildProcessorContext
    {
        /// <summary>
        /// Gets a summary of build project process.
        /// </summary>
        HiveBuildSummary BuildProjectSummary { get; set; }

        /// <summary>
        /// Gets an output path for the build.
        /// </summary>
        string BuildPath { get; }
    }
}
