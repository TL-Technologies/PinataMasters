namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPostprocessor<in T> where T : IBuildPostprocessorContext
    {
        void OnPostprocessBuild(T context);
    }
}
