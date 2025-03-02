using Modules.Hive.Pipeline;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    public class OpenBuildLocationTask : EditorPipelineTask<IBuildProjectTaskContext>
    {
        public OpenBuildLocationTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IBuildProjectTaskContext context)
        {
            // Unity bug: Clear progress bar before calling EditorUtility.DisplayDialog
            //EditorUtility.ClearProgressBar();
            pipeline.View.Hide();

            // Do nothing if pipeline is cancelled
            if (pipeline.IsCancellationRequested)
            {
                return SetStatus(PipelineTaskStatus.Canceled);
            }

            // Skip the task in batch mode
            if (Application.isBatchMode)
            {
                return SetStatus(PipelineTaskStatus.Succeeded);
            }

            // Alert if build failed
            if (pipeline.HasFailedTasks)
            {
                Debug.Log("Build failed! See the log below for more details.");
                EditorUtility.DisplayDialog(pipeline.View.Title, "Errors occurred during the build.\nSee the log below for more details.", "Close");
                return SetStatus(PipelineTaskStatus.Succeeded);
            }

            // Alert if build location doesn't exist
            string path = context.BuildProjectSummary.outputPath;
            if (string.IsNullOrWhiteSpace(path) || (!Directory.Exists(path) && !File.Exists(path)))
            {
                EditorUtility.DisplayDialog(pipeline.View.Title, "Unable to find a build location.\nMaybe a build process has been skipped.", "Close");
                return SetStatus(PipelineTaskStatus.Succeeded);
            }

            // Alert that build succeeded
            Debug.Log("Build succeeded!");
            if (!EditorUtility.DisplayDialog(pipeline.View.Title, "Build succeeded!", "Close", "Open folder"))
            {
                EditorUtility.RevealInFinder(path);
            }

            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}
