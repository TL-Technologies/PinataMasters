using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class InvokeBuildPreprocessorTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Build preprocessing...";

            using (IBuildPreprocessorContext preprocessorContext = context.CreateBuildPreprocessorContext())
            {
                EditorPipelineBroadcaster.InvokeGenericProcessors(
                    typeof(IBuildPreprocessor<>),
                    nameof(IBuildPreprocessor<IBuildPreprocessorContext>.OnPreprocessBuild),
                    preprocessorContext);
            }

            // Refresh project files and reimport any changes
            AssetDatabase.Refresh();

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
