using Modules.Hive.Pipeline;


namespace Modules.Hive.Editor.Pipeline
{
    public interface IEditorPipeline : IPipeline
    {
        IEditorPipelineView View { get; }
    }


    public interface IEditorPipeline<out T> : IPipeline<T>, IEditorPipeline
        where T : IEditorPipelineContext { }
}
