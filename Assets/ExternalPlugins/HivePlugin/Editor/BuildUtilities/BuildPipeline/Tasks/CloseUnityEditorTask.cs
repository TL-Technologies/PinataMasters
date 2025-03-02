using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class CloseUnityEditorTask : EditorPipelineTask
    {
        public CloseUnityEditorTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IEditorPipelineContext context)
        {
            pipeline.View.Description = "Exit...";

            EditorApplication.Exit(pipeline.HasFailedTasks ? 1 : 0);

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
