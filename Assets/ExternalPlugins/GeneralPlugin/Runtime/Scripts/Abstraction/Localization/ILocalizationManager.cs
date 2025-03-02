using System;
using System.Collections.Generic;


namespace Modules.General.Abstraction
{
    public interface ILocalizationManager
    {
        event Action<string> OnLocaleChanged;
        
        string CurrentLocale { get; }
        string DeviceLocale { get; }

        bool HasLocale(string locale);
        bool TrySetLocale(string locale, bool isForce = false);
        List<String> GetAllLocales();
        string GetLocalizedText(string key);
    }
}