using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class PluginRemoveTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Removing plugins from project...";

            PluginDisabler.RemovePlugins();
            AssetDatabase.Refresh();

            string path = Path.GetDirectoryName(context.BuildOptions.locationPathName);
            ProjectPluginsUtilities.CurrentUtilities.WritePluginsToFile(path);
            
            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
