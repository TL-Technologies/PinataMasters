using Modules.Hive.Editor.Pipeline;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class UpdateDirectivesBuildPipeline : IBuildPipeline
    {
        public string PipelineName => "UpdateDirectives";

        public EditorPipelineBuilder<BuildPipelineContext> Construct(
            EditorPipelineBuilder<BuildPipelineContext> builder)
        {
            return builder
                .AddTask(new CollectPreprocessorDirectivesTask())
                .AddTask(new SetPreprocessorDirectivesTask());
        }
    }
}