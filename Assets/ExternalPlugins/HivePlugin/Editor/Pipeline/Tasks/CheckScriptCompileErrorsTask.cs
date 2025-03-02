using Modules.Hive.Editor.Reflection;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.Pipeline
{
    public class CheckScriptCompileErrorsTask : EditorPipelineTask
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IEditorPipelineContext context)
        {
            pipeline.View.Description = "Checking scripts compilation...";
            int count = ConsoleWindowHelper.GetScriptCompileErrorsCount();

            return SetStatus(count == 0 ? PipelineTaskStatus.Succeeded : PipelineTaskStatus.Failed);
        }
    }
}
