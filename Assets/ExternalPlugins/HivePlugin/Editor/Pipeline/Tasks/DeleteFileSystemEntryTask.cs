using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Pipeline;
using System.IO;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.Pipeline
{
    public class DeleteFileSystemEntryTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            string path = context.BuildOptions.locationPathName;
            // WARNING!!!
            // Restriction removed by request. But it's not safe!!!
            // Anyone can execute this task with any path outside of the project and delete files and directories there.
            // if (!UnityPath.IsPathLocatedInsideProject(path))
            //    throw new ArgumentException("Unable to delete file system entry outside of the project.", nameof(path));
            
            pipeline.View.Description = $"Deleting: {path}";

            if (File.Exists(path))
            {
                File.Delete(path);
            }

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
