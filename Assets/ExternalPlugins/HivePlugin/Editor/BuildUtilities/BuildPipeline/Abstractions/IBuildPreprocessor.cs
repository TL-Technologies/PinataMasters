namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildPreprocessor<in T> where T : IBuildPreprocessorContext
    {
        void OnPreprocessBuild(T context);
    }
}
