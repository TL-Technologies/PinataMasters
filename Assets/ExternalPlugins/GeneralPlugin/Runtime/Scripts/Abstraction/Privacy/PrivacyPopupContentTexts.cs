using System;


namespace Modules.General.Abstraction.Privacy
{
    public abstract class PrivacyPopupContentTexts : IPrivacyPopupContentTexts
    {
        public abstract event Action OnSystemPopupsContentTextsChanged;
        
        public abstract PopupContentTexts FirstSystemPopupContentTexts { get; }
        public abstract PopupContentTexts SecondSystemPopupContentTexts { get; }

        
        public PopupContentTexts OfType(SystemPopupType systemPopupType)
        {
            PopupContentTexts popupContentTexts = null;

            switch (systemPopupType)
            {
                case SystemPopupType.None:
                    break;
                
                case SystemPopupType.First:
                    popupContentTexts = FirstSystemPopupContentTexts;
                    break;
                
                case SystemPopupType.Second:
                    popupContentTexts = SecondSystemPopupContentTexts;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(systemPopupType), systemPopupType, null);
            }

            return popupContentTexts;
        }
    }
}

