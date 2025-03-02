using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class InvokeBuildPostprocessorTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Build postprocessing...";

            using (IBuildPostprocessorContext postprocessorContext = context.CreateBuildPostprocessorContext())
            {
                EditorPipelineBroadcaster.InvokeGenericProcessors(
                    typeof(IBuildPostprocessor<>),
                    nameof(IBuildPostprocessor<IBuildPostprocessorContext>.OnPostprocessBuild),
                    postprocessorContext);
            }

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
