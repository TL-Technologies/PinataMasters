namespace Modules.General.Abstraction
{
    public interface IAbTestManifestInfo
    {
        string AbTestGroupName { get; set; }
        string ETag { get; set; }
    }
}
