using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Pipeline;
using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    public interface IBuildProjectTaskContext : IEditorPipelineContext
    {
        /// <summary>
        /// Gets a summary of build project process.
        /// </summary>
        HiveBuildSummary BuildProjectSummary { get; set; }
    }


    public class BuildProjectTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            pipeline.View.Description = "Build project...";

            // Initialize insertion point
            BuildProjectInsertionPoint.CurrentPipeline = pipeline;

            // Build project
            BuildReport report = BuildPipeline.BuildPlayer(context.BuildOptions);
            BuildResult buildResult = report.summary.result;
            context.BuildProjectSummary = (HiveBuildSummary)report.summary;

            // Finalize insertion point
            BuildProjectInsertionPoint.CurrentPipeline = null;

            // Checks build process result
            if (buildResult == BuildResult.Cancelled)
            {
                pipeline.Cancel();
            }
            
            // It's possible in Unity to have BuildResult.Succeeded with not zero totalErrors count.
            PipelineTaskStatus status = buildResult == BuildResult.Succeeded && report.summary.totalErrors == 0 ? 
                PipelineTaskStatus.Succeeded : 
                PipelineTaskStatus.Failed;
            if (status == PipelineTaskStatus.Failed)
            {
                Debug.Log($"Hive build: Unity build task has {report.summary.totalErrors} problems!");
            }
            
            return SetStatus(status);
        }
    }


    internal static class BuildProjectInsertionPoint
    {
        public static IEditorPipeline CurrentPipeline { get; internal set; }

        public static T GetCurrentPipelineContext<T>() where T : class, IEditorPipelineContext
        {
            if (CurrentPipeline == null)
            {
                return null;
            }

            // It's important to cast CurrentPipeline to base interface.
            // Might be a case when the CurrentPipeline does not directly inherit an interface IEditorPipeline<T>
            if (!(CurrentPipeline is IEditorPipeline<IEditorPipelineContext> pipeline))
            {
                throw new InvalidCastException(string.Format("Cannot cast from source type '{0}' to destination type '{1}'",
                    TypeNameHelper.GetTypeDisplayName(CurrentPipeline),
                    TypeNameHelper.GetTypeDisplayName<IEditorPipeline<IEditorPipelineContext>>()));
            }

            // Cast Context to required type if it possible
            if (!(pipeline.Context is T context))
            {
                throw new InvalidCastException(string.Format("Cannot cast from source type '{0}' to destination type '{1}'",
                    TypeNameHelper.GetTypeDisplayName(pipeline.Context),
                    TypeNameHelper.GetTypeDisplayName<T>()));
            }

            return context;
        }
    }
}
