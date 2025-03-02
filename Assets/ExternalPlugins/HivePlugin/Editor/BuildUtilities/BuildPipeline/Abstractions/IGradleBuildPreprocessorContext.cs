using Modules.Hive.Editor.BuildUtilities.Android;


namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IGradleBuildPreprocessorContext : IBuildProcessorContext
    {
        /// <summary>
        /// Gets a gradle properties of the project.
        /// </summary>
        GradleProperties GradleProperties { get; }
    

        /// <summary>
        /// Gets the path to the root of the Gradle project.
        /// </summary>
        string GradleProjectPath { get; }
    }
}
