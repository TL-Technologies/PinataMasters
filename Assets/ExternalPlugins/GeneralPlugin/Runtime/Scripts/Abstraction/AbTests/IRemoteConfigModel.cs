using Newtonsoft.Json.Linq;


namespace Modules.General.Abstraction
{
    public interface IRemoteConfigModel
    {
        string Version { get; set; }
        JObject Data { get; set; }
    }
}
