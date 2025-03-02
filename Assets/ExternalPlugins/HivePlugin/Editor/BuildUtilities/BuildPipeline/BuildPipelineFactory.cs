using Modules.Hive.Editor.BuildUtilities.Android;
using Modules.Hive.Editor.Pipeline;
using System;
using System.Linq;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public static class BuildPipelineFactory
    {
        public static BuildPlayerOptions GetBuildPlayerOptionsFromEditor()
        {
            return GetBuildPlayerOptionsFromEditor(EditorUserBuildSettings.activeBuildTarget);
        }


        public static BuildPlayerOptions GetBuildPlayerOptionsFromEditor(BuildTarget buildTarget)
        {
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                target = buildTarget,
                targetGroup = BuildPipeline.GetBuildTargetGroup(buildTarget),
                scenes = EditorBuildSettings.scenes
                    .Where(p => p.enabled)
                    .Select(p => p.path)
                    .ToArray(),
                options = BuildOptions.None
            };

            if (EditorUserBuildSettings.development)
            {
                buildOptions.options |= BuildOptions.Development;
            }
            if (EditorUserBuildSettings.allowDebugging)
            {
                buildOptions.options |= BuildOptions.AllowDebugging;
            }
            if (EditorUserBuildSettings.connectProfiler)
            {
                buildOptions.options |= BuildOptions.ConnectWithProfiler;
            }
            
            return buildOptions;
        }


        public static BuildPipelineContext CreateDefaultPipelineContext(
            BuildPlayerOptions options,
            AndroidTarget androidTarget = AndroidTarget.None)
        {
            switch (options.target)
            {
                case BuildTarget.Android:
                    if (androidTarget == AndroidTarget.Huawei)
                    {
                        return new HuaweiBuildPipelineContext(options);
                    }
                    if (androidTarget == AndroidTarget.Amazon)
                    {
                        return new AmazonBuildPipelineContext(options);
                    }
                    if (androidTarget == AndroidTarget.GooglePlay)
                    {
                        return new GooglePlayBuildPipelineContext(options);
                    }
                    return new GooglePlayBuildPipelineContext(options);

                case BuildTarget.iOS:
                    return new IosBuildPipelineContext(options);

                case BuildTarget.StandaloneOSX:
                    return new MacOsBuildPipelineContext(options);
                
                case BuildTarget.StandaloneWindows64:
                    return new WindowsBuildPipelineContext(options);
                
                case BuildTarget.WebGL:
                    return new WebGlBuildPipelineContext(options);
                
                default:
                    throw new NotImplementedException($"Unable to determine a pipeline context for {nameof(BuildTarget)}.{options.target}");
            }
        }


        public static EditorPipelineBuilder<BuildPipelineContext> CreateDefaultPipeline(BuildPipelineContext pipelineContext)
        {
            var builder = new EditorPipelineBuilder<BuildPipelineContext>(pipelineContext);
            return new DefaultPipeline().Construct(builder);
        }
        
        
        public static EditorPipelineBuilder<BuildPipelineContext> CreatePipeline(string pipelineName, BuildPipelineContext pipelineContext)
        {
            var pipelines = EditorPipelineBroadcaster.GetPipelineInstances();
            foreach (var pipeline in pipelines)
            {
                if (pipeline.PipelineName == pipelineName)
                {
                    var builder = new EditorPipelineBuilder<BuildPipelineContext>(pipelineContext);
                    return pipeline.Construct(builder);
                }
            }

            throw new InvalidOperationException($"Pipeline with name {pipelineName} not found.");
        }


        public static EditorPipelineBuilder<BuildPipelineContext> CreatePipeline(IBuildPipeline pipeline, AndroidTarget androidTarget = AndroidTarget.None)
        {
            BuildPlayerOptions buildOptions = GetBuildPlayerOptionsFromEditor();
            
            BuildPipelineContext context = CreateDefaultPipelineContext(buildOptions, androidTarget);
            
            return pipeline.Construct(new EditorPipelineBuilder<BuildPipelineContext>(context));
        }
    }
}
