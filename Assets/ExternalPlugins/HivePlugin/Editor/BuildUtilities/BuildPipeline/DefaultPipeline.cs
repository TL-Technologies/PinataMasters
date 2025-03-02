using Modules.Hive.Editor.Pipeline;


namespace Modules.Hive.Editor.BuildUtilities
{
    public sealed class DefaultPipeline : IBuildPipeline
    {
        public string PipelineName => "default";

        public EditorPipelineBuilder<BuildPipelineContext> Construct(
            EditorPipelineBuilder<BuildPipelineContext> builder)
        {
            return builder
                .AddTask(new SetAndroidSignatureTask())
                .AddTask(new CheckDependenciesTask())
                .AddTask(new ModifyBuildPipelineOptionsTask())
                .AddTask(new SwitchPlatformTask())
                .AddTask(new CollectPreprocessorDirectivesTask())
                .AddTask(new SetPreprocessorDirectivesTask())
                .AddTask(new CheckScriptCompileErrorsTask())
                .AddTask(new DeleteFileSystemEntryTask())
                .AddTask(new MakeProjectSnapshotTask())
                .AddTask(new InvokeBuildPreprocessorTask())
                .AddTask(new SetAndroidSignatureTask())
                .AddTask(new PluginRemoveTask())
                .AddTask(new BuildProjectTask())
                .AddTask(new InvokeBuildPostprocessorTask())
                .AddTask(new InvokeBuildFinalizerTask())
                .AddTask(new RestoreProjectSnapshotTask())
                .AddTask(new PluginRestoreTask())
                .AddTask(new LogExceptionsTask())
                .AddTask(new CheckPackagesVersionTask())
                .AddTask(new OpenBuildLocationTask());
        }
    }
}