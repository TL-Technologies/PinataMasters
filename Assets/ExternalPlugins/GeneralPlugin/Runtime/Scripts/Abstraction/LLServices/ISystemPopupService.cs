using System;


namespace Modules.General.Abstraction
{
    public interface ISystemPopupService
    {
        void ShowPopupWithoutButtons(string title, string message);
        void HidePopupWithoutButtons();
        void ShowPopUp(string title, string message, string buttonTitle, System.Action callback = null);
        void ShowPopUpWithTwoButtons(string title, string message, string firstButtonTitle,
            string secondButtonTitle, Action firstButtonCallback = null, Action secondButtonCallback = null,
            bool isVerticalLayout = true);
    }
}
