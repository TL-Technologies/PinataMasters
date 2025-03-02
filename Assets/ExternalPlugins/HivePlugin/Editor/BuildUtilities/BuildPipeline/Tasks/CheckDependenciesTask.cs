using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class CheckDependenciesTask : EditorPipelineTask<BuildPipelineContext>
    {
        public CheckDependenciesTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }
        
        
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Checking dependencies in project...";
            
            PackagesDependenciesChecker checker = new PackagesDependenciesChecker(); 
            
            PipelineTaskStatus taskStatus = checker.HasUnresolvedDependencies()
                ? PipelineTaskStatus.Failed
                : PipelineTaskStatus.Succeeded;
          
            return SetStatus(taskStatus);
        }
    }
}