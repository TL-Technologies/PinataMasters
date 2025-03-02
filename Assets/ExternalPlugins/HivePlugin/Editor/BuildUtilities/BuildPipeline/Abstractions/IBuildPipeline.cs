using Modules.Hive.Editor.Pipeline;


namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPipeline
    {
        string PipelineName { get; }
        
        EditorPipelineBuilder<BuildPipelineContext>  Construct(EditorPipelineBuilder<BuildPipelineContext> builder);
    }
}