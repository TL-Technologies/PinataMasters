using Newtonsoft.Json.Linq;


namespace Modules.General.Abstraction
{
    public interface IAbTestDataModel
    {
        int Percent { get; set; }
        JObject Data { get; set; }
        (string groupName, JObject data) GetRandomTestData();
    }
}
