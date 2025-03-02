using Modules.Hive.Editor.Pipeline;
using UnityEditor.Android;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class PostGenerateGradleAndroidProjectHandler : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;
        

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            var pipelineContext = BuildProjectInsertionPoint.GetCurrentPipelineContext<AndroidBuildPipelineContext>();
            if (pipelineContext == null)
            {
                return;
            }

            using (IGradleBuildPreprocessorContext preprocessorContext = pipelineContext.CreateGradleBuildPreprocessorContext(path))
            {
                EditorPipelineBroadcaster.InvokeGenericProcessors(
                    typeof(IGradleBuildPreprocessor<>),
                    nameof(IGradleBuildPreprocessor<IGradleBuildPreprocessorContext>.OnPreprocessGradleBuild),
                    preprocessorContext);
            }
        }
    }
}
