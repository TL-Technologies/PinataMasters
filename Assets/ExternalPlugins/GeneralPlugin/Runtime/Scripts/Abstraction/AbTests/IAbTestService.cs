using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface IAbTestService
    {
        bool IsReady { get; }
        
        IDictionary<Type, IAbTestData> TestsData { get; set; }
        bool ShouldUseTestUrl { get; set; }
        IAbTestManifestModel RemoteManifestModel { get; }

        string GetDefaultGroupName();
        string CurrentGroupName { get; }
        
        T GetTestData<T>() where T : IAbTestData;
        IAbTestServiceData LoadServiceData();
        void SetManifest(IAbTestManifestModel model, string groupName, JObject groupData, string eTag);
        void ResetAbTestData();
    }
}
