using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


namespace Modules.Advertising
{
    [CreateAssetMenu(fileName = "AdvertisingEditorSettings")]
    public class AdvertisingEditorSettings : ScriptableSingleton<AdvertisingEditorSettings>
    {
        #region Fields

        [EnumFlags] [SerializeField]
        private AdModule supportedModules = AdModule.Banner | AdModule.Interstitial | AdModule.RewardedVideo;

        [Module(AdModule.Banner)] [SerializeField]
        private CustomBannerSettings customBannerSettings = new CustomBannerSettings();

        [Module(AdModule.Interstitial | AdModule.RewardedVideo)] [SerializeField]
        private VideoSettings videoSettings = new VideoSettings();

        [Module(AdModule.Interstitial)] [SerializeField]
        private InterstitialSettings interstitialModuleSettings = new InterstitialSettings();

        [Module(AdModule.RewardedVideo)] [SerializeField]
        private RewardedVideoSettings rewardedVideoModuleSettings = new RewardedVideoSettings();

        private Dictionary<string, AdModuleSettings> modules = null;

        #endregion



        #region Properties

        public CustomBannerSettings EditorBannerSettings => customBannerSettings;


        public InterstitialSettings EditorInterstitialModuleSettings => interstitialModuleSettings;


        public RewardedVideoSettings EditorRewardedVideoModuleSettings => rewardedVideoModuleSettings;


        public VideoSettings EditorVideoSettings => videoSettings;


        public List<AdModule> SupportedModules
        {
            get
            {
                Type moduleType = typeof(AdModuleSettings);
                Type settingsType = typeof(AdvertisingEditorSettings);

                if (modules == null)
                {
                    modules = settingsType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Where(fieldInfo => moduleType.IsAssignableFrom(fieldInfo.FieldType))
                        .ToDictionary(
                            fieldInfo => fieldInfo.Name,
                            fieldInfo => fieldInfo.GetValue(Instance) as AdModuleSettings);
                }

                return modules.Where(pair => (Instance.supportedModules & pair.Value.ModuleType) != 0)
                    .ToDictionary(pair => pair.Key, pair => pair.Value).Select(x => x.Value.ModuleType).ToList();
            }
        }

        #endregion
    }
}