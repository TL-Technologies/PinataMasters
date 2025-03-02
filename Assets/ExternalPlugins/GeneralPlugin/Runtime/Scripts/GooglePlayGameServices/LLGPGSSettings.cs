using Modules.General.Abstraction.GooglePlayGameServices;
using Modules.General.HelperClasses;
using Modules.Hive;
using UnityEngine;


namespace Modules.General.GooglePlayGameServices
{
    [CreateAssetMenu(fileName = "GpgsSettings")]
    public class LLGPGSSettings : ScriptableSingleton<LLGPGSSettings>, IGpgsSettings
    {
        [SerializeField] private bool isGpgsEnabled = false;
        [SerializeField] private string appId = "";
        [Header("Optional fields")]
        [SerializeField] private string webClientId = "";
        
        
        public bool IsGpgsEnabled => isGpgsEnabled && PlatformInfo.AndroidTarget == AndroidTarget.GooglePlay;
        public string AppId => appId;
        public string WebClientId => webClientId;
    }
}
