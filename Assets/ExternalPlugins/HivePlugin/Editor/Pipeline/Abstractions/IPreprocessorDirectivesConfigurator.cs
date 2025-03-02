namespace Modules.Hive.Editor.Pipeline
{
    public interface IPreprocessorDirectivesConfigurator
    {
        void OnConfigurePreprocessorDirectives(IPreprocessorDirectivesCollection directives);
    }
}
