using System;


namespace Modules.General
{
    public interface IStatisticsManager
    {
        event Action<int> OnCurrentLevelChanged;
        bool IsVibrationEnabled { get; set; }
        bool IsMusicEnabled { get; set; }
        bool IsSoundEnabled { get; set; }
        int CurrentLevelNumber { get; set; }
        int CurrentKeysCount { get; set; }
        int CurrentStarsCount { get; set; }
        int SoftCurrency { get; set; }
        string CountryCode { get; set; }
        bool IsLocatedInChina { get; }
    }
}
