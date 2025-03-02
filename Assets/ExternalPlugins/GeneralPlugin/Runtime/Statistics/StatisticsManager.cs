using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using System;


namespace Modules.General
{
    [InitQueueService(-10, bindTo: typeof(IStatisticsManager))]
    public class StatisticsManager : IStatisticsManager
    {
        #region Fields

        public event Action<int> OnCurrentLevelChanged;

        public const bool IsVibrationEnabledByDefault = true;
        public const bool IsMusicEnabledByDefault = true;
        public const bool IsSoundEnabledByDefault = true;
        public const int CurrentLevelNumberByDefault = 1;
        public const int CurrentKeysCountByDefault = 0;
        public const int CurrentStarsCountByDefault = 0;
        public const int SoftCurrencyDefault = 0;

        private const string SettingsVibrationState = "settings_vibration_state";
        private const string SettingsMusicState = "settings_music_state";
        private const string SettingsSoundState = "settings_sound_state";
        private const string CurrentLevelNumberKey = "current_level_number";
        private const string CurrentKeysCountKey = "current_keys_count";
        private const string CurrentStarsCountKey = "current_stars_count";
        private const string SoftCurrencyKey = "soft_currency";
        private const string CountryCodeKey = "country_code";
        private const string ChinaCountryCode = "ch";

        #endregion



        #region Properties

        public bool IsVibrationEnabled
        {
            get => CustomPlayerPrefs.GetBool(SettingsVibrationState, IsVibrationEnabledByDefault);
            set => CustomPlayerPrefs.SetBool(SettingsVibrationState, value);
        }


        public bool IsMusicEnabled
        {
            get => CustomPlayerPrefs.GetBool(SettingsMusicState, IsMusicEnabledByDefault);
            set => CustomPlayerPrefs.SetBool(SettingsMusicState, value);
        }


        public bool IsSoundEnabled
        {
            get => CustomPlayerPrefs.GetBool(SettingsSoundState, IsSoundEnabledByDefault);
            set => CustomPlayerPrefs.SetBool(SettingsSoundState, value);
        }


        public int CurrentLevelNumber
        {
            get { return CustomPlayerPrefs.GetInt(CurrentLevelNumberKey, CurrentLevelNumberByDefault); }
            set
            {
                if (CurrentLevelNumber != value)
                {
                    CustomPlayerPrefs.SetInt(CurrentLevelNumberKey, value);
                    OnCurrentLevelChanged?.Invoke(value);
                }
            }
        }


        public int CurrentKeysCount
        {
            get { return CustomPlayerPrefs.GetInt(CurrentKeysCountKey, CurrentKeysCountByDefault); }
            set
            {
                if (CurrentKeysCount != value)
                {
                    CustomPlayerPrefs.SetInt(CurrentKeysCountKey, value);
                }
            }
        }


        public int CurrentStarsCount
        {
            get { return CustomPlayerPrefs.GetInt(CurrentStarsCountKey, CurrentStarsCountByDefault); }
            set
            {
                if (CurrentStarsCount != value)
                {
                    CustomPlayerPrefs.SetInt(CurrentStarsCountKey, value);
                }
            }
        }


        public int SoftCurrency
        {
            get { return CustomPlayerPrefs.GetInt(SoftCurrencyKey, SoftCurrencyDefault); }
            set
            {
                if (SoftCurrency != value)
                {
                    CustomPlayerPrefs.SetInt(SoftCurrencyKey, value);
                }
            }
        }


        public string CountryCode
        {
            get => CustomPlayerPrefs.GetString(CountryCodeKey);
            set => CustomPlayerPrefs.SetString(CountryCodeKey, value);
        }


        public bool IsLocatedInChina => CountryCode == ChinaCountryCode;

        #endregion
    }
}
