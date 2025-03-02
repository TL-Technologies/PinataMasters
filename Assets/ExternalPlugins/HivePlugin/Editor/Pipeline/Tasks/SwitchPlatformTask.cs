using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    public class SwitchPlatformTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            BuildTarget buildTarget = context.BuildOptions.target;
            BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget);
            
            pipeline.View.Description = "Switching target platform...";
            bool result = EditorUserBuildSettings.SwitchActiveBuildTarget(buildTargetGroup, buildTarget);
            if (!result)
            {
                Debug.LogError($"Build to {buildTarget} is not supported. Appropriate Unity module is not installed.");
            }

            return SetStatus(result ? PipelineTaskStatus.Succeeded : PipelineTaskStatus.Failed);
        }
    }
}
