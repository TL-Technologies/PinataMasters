using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public static class BuildButtonHandler
    {
        public static void RequestBuild(BuildPlayerOptions buildPlayerOptions)
        {
            OnBuildRequested(buildPlayerOptions);
        }
        
        
        [InitializeOnLoadMethod]
        private static void BindBuildInterceptor()
        {
            BuildPlayerWindow.RegisterBuildPlayerHandler(OnBuildRequested);
        }


        private static void OnBuildRequested(BuildPlayerOptions options)
        {
            options.options &= ~BuildOptions.ShowBuiltPlayer;
            
            BuildPipelineContext context = BuildPipelineFactory.CreateDefaultPipelineContext(options, PlatformInfo.AndroidTarget);
            
            BuildPipelineFactory
                .CreateDefaultPipeline(context)
                .BuildAndSubmit();
        }
    }
}
