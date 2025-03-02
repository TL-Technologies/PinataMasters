using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class InvokeBuildFinalizerTask : EditorPipelineTask<BuildPipelineContext>
    {
        public InvokeBuildFinalizerTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }
        
        
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Build finalization...";
            
            EditorPipelineBroadcaster.InvokeProcessors(
                typeof(IBuildFinalizer),
                nameof(IBuildFinalizer.OnFinalizeBuild),
                context);

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}