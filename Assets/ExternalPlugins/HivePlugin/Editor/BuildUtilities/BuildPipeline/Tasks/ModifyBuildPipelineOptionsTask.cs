using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class ModifyBuildPipelineOptionsTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Modifying build pipeline context...";
            
            EditorPipelineBroadcaster.InvokeProcessors(
                typeof(IBuildPipelineOptionsModifier),
                nameof(IBuildPipelineOptionsModifier.OnModifyBuildPipelineOptions),
                context);

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
