using System;


namespace Modules.General.Abstraction.Privacy
{
    public interface IPrivacyPopupContentTexts
    {
        event Action OnSystemPopupsContentTextsChanged;
        
        PopupContentTexts FirstSystemPopupContentTexts { get; }
        PopupContentTexts SecondSystemPopupContentTexts { get; }
        PopupContentTexts OfType(SystemPopupType systemPopupType);
    }
}

