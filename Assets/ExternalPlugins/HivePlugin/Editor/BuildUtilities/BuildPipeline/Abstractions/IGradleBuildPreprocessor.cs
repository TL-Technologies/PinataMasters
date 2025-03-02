namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IGradleBuildPreprocessor<in T> where T : IGradleBuildPreprocessorContext
    {
        void OnPreprocessGradleBuild(T context);
    }
}
