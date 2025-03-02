namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPreprocessorContext : IBuildProcessorContext
    {
        /// <summary>
        /// Gets an object that allows to keep structure and contents of the project.
        /// <para>Keep in mind! It will be null if the snapshot is not created in advance.</para>
        /// </summary>
        ProjectSnapshot ProjectSnapshot { get; }
        PluginDisabler PluginDisabler { get; }
    }
}
