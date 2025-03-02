namespace Modules.General.Abstraction
{
    public interface IAbTestManifestModel
    {
        IAbTestDataModel AbTest { get; set; }
        IRemoteConfigModel RemoteConfig { get; set; }
    }
}
