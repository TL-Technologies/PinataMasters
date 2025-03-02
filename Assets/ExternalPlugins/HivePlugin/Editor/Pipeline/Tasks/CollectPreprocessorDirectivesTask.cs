using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.Pipeline
{
    public class CollectPreprocessorDirectivesTask : EditorPipelineTask<IPreprocessorDirectivesTaskContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IPreprocessorDirectivesTaskContext context)
        {
            pipeline.View.Description = "Collecting preprocessor directives...";

            EditorPipelineBroadcaster.InvokeProcessors(
                typeof(IPreprocessorDirectivesConfigurator),
                nameof(IPreprocessorDirectivesConfigurator.OnConfigurePreprocessorDirectives),
                context.PreprocessorDirectives);

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
