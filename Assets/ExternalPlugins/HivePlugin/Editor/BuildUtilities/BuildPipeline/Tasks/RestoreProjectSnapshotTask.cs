using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class RestoreProjectSnapshotTask : EditorPipelineTask<BuildPipelineContext>
    {
        public RestoreProjectSnapshotTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Restoring project snapshot...";

            ProjectSnapshot.RestoreSnapshot();

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
