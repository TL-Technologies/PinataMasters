namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public interface IGpgsSettings
    {
        bool IsGpgsEnabled { get; }
        string AppId { get; }
        string WebClientId { get; }
    }
}
