namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildFinalizer
    {
        void OnFinalizeBuild(BuildPipelineContext context);
    }
}
