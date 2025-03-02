namespace Modules.Hive.Editor.Pipeline
{
    public interface IPreprocessorDirectivesTaskContext : IEditorPipelineContext
    {
        /// <summary>
        /// Gets a set of preprocessor directives that will be applied to the project.
        /// </summary>
        PreprocessorDirectivesCollection PreprocessorDirectives { get; }
    }
}
