using Modules.Hive.Editor.BuildUtilities;


namespace Modules.BuildProcess
{
    public interface ITargetSettingsActualizer
    {
        bool TryActualizeSettingsAtPath(string settingsPath, IBuildPreprocessorContext context);
    }
}
