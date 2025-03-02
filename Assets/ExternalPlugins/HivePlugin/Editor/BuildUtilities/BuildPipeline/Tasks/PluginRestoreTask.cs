using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class PluginRestoreTask : EditorPipelineTask<BuildPipelineContext>
    {
        public PluginRestoreTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }
        
        
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Restoring plugins to project...";

            PluginDisabler.RestorePlugins();

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
