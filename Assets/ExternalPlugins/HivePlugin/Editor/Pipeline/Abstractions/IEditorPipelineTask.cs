using Modules.Hive.Pipeline;


namespace Modules.Hive.Editor.Pipeline
{
    public interface IEditorPipelineTask<in T> : IPipelineTask<IEditorPipeline, T>
        where T : IEditorPipelineContext { }
}
