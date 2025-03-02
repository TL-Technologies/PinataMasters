using Modules.General.Abstraction;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;


public static class LLSystemPopupManager
{
    #region Variables

    delegate void LLSystemPopupManagerDelegate(string str);

    const string NoSystemPopupManagerText = "LLSystemPopupManager: You need to Initialize() it first!";

    private static bool isInitialized = false;
    private static Dictionary<string, Action> sendersCallback = new Dictionary<string, Action>();

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    private static extern void LLSystemPopUpWithoutButtonsShow(string title, string message);
    [DllImport ("__Internal")]
    private static extern void LLSystemPopUpWithoutButtonsHide();
    [DllImport ("__Internal")]
    private static extern void LLSystemPopUpShow(string title, string message, string button, string callbackName);
    [DllImport ("__Internal")]
    private static extern void LLSystemPopUpWithTwoButtons(string title, string message, string firstButtonText, string secondButtonText,
        string firstButtonCallback, string secondButtonCallback, int isVerticalLayout, int isSecondButtonBold);
    [DllImport ("__Internal")]
    private static extern void LLSystemPopUpRegisterCallback(LLSystemPopupManagerDelegate callback);
    #endif

    #endregion



    #region Methods

    public static void Initialize()
    {
        if (isInitialized)
        {
            return;
        }

        #if UNITY_IOS && !UNITY_EDITOR
            LLSystemPopUpRegisterCallback(PopUpManagerCallback);
        #endif

        isInitialized = true;
    }


    public static void ShowPopupWithoutButtons(string title, string message)
    {
        CheckInitialization();

        title = CheckText(title);
        message = CheckText(message);

        #if UNITY_IOS && !UNITY_EDITOR
            LLSystemPopUpWithoutButtonsShow(title, message);
        #endif

        #if UNITY_ANDROID && !UNITY_EDITOR
            LLActivity.ShowPopup(title, message, false, null, false);
        #endif
    }


    public static void HidePopupWithoutButtons()
    {
        CheckInitialization();

        #if UNITY_IOS && !UNITY_EDITOR
            LLSystemPopUpWithoutButtonsHide();
        #endif
    }


    public static void ShowPopUp(string title, string message, string buttonTitle, System.Action callback = null)
    {
        CheckInitialization();

        #if UNITY_IOS && !UNITY_EDITOR
            title = CheckText(title);
            message = CheckText(message);
            buttonTitle = CheckText(buttonTitle);
            string callbackName = CheckCallback(callback);

            LLSystemPopUpShow(title, message, buttonTitle, callbackName);
        #endif
    }


    public static void ShowPopUpWithTwoButtons(string title,
                                               string message,
                                               string firstButtonTitle,
                                               string secondButtonTitle,
                                               Action firstButtonCallback = null,
                                               Action secondButtonCallback = null,
                                               bool isVerticalLayout = true,
                                               bool isSecondButtonBold = true)
    {
        CheckInitialization();

        #if UNITY_IOS && !UNITY_EDITOR
            title = CheckText(title);
            message = CheckText(message);
            firstButtonTitle = CheckText(firstButtonTitle);
            secondButtonTitle = CheckText(secondButtonTitle);
            string firstButtonCallbackName = CheckCallback(firstButtonCallback);
            string secondButtonCallbackName = CheckCallback(secondButtonCallback);

            LLSystemPopUpWithTwoButtons(title, message, firstButtonTitle, secondButtonTitle, firstButtonCallbackName,
                secondButtonCallbackName, Convert.ToInt32(isVerticalLayout), Convert.ToInt32(isSecondButtonBold));
        #endif
    }


    private static void CheckInitialization()
    {
        if (!isInitialized)
        {
            throw new InvalidOperationException(NoSystemPopupManagerText);
        }
    }


    private static string CheckText(string value)
    {
        if (value == null)
        {
            CustomDebug.LogWarning("Provided value is null");
            return string.Empty;
        }

        return value;
    }


    private static string CheckCallback(Action callback)
    {
        string callbackName = string.Empty;

        if (callback != null)
        {
            Guid guidCallback = Guid.NewGuid();
            callbackName = guidCallback.ToString();
            sendersCallback.Add(callbackName, callback);
        }

        return callbackName;
    }

    #endregion



    #region Popup Delegate

    [AOT.MonoPInvokeCallbackAttribute(typeof(LLSystemPopupManagerDelegate))]
    private static void PopUpManagerCallback(string callbackName)
    {
        if (!string.IsNullOrEmpty(callbackName))
        {
            if (sendersCallback.ContainsKey(callbackName))
            {
                Action callback = sendersCallback[callbackName];

                callback?.Invoke();

                sendersCallback.Remove(callbackName);
            }
        }
    }

    #endregion
}
