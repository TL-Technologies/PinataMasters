namespace Modules.BuildProcess
{
    public interface IConcreteTargetSettingsActualizer : ITargetSettingsActualizer
    {
        string FolderName { get; }
    }
}
