namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPipelineOptionsModifier
    {
        void OnModifyBuildPipelineOptions(BuildPipelineContext context);
    }
}
