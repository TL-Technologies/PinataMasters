#if UNITY_ANDROID
using Modules.General;
using System;
using UnityEngine;


public struct LLAndroidButtonOnPopup
{
    public enum ButtonIndex
    {
        Positive = 0,
        Negative = 1,
        Neutral = 2
    }

    public string label;
    public Action clickCallback;
    public ButtonIndex index;
}


public sealed class LLActivity
{

    #region Variables

    #if !UNITY_EDITOR
    const string METHOD_DEBUG_ENABLE = "LLActivitySetDebugEnable";
    const string METHOD_CALLBACK_CHANGE_VISIBLE_STATE = "LLActivitySetCallbackChangeVisibleState";
    const string METHOD_SHOW_TOAST = "LLActivityShowToast";
    const string METHOD_SHOW_POPUP = "LLActivityShowPopup";
    const string METHOD_IS_NEED_TO_UPDATE = "LLActivityIsNeedToUpdate";

    const string METHOD_PROCESS_REFERRER = "LLActivityProcessReferrer";
    const string METHOD_IS_REFERRER_INFO_VALID = "LLActivityGetIsReferrerInfoValid";
    const string METHOD_GET_REFERRER_SCHEME = "LLActivityGetReferrerScheme";
    const string METHOD_GET_REFERRER_INFO = "LLActivityGetReferrerInfo";
    const string METHOD_GET_REFERRER_STRING = "LLActivityGetReferrerString";
    #endif

    static Action<bool> callbackChangeVisibleState;

    #endregion


    #region Properties

    public static bool IsNeedUpdate
    {
        get
        {
            bool result = false;

            #if !UNITY_EDITOR
            result = LLAndroidJavaSingletone<LLActivity>.CallStatic<bool>(METHOD_IS_NEED_TO_UPDATE);
            #endif

            return result;
        }
    }
    

    public static event Action<bool> OnChangeVisibleState
    {
        add
        {
            if (callbackChangeVisibleState == null)
            {
                #if !UNITY_EDITOR
                LLAndroidJavaSingletone<LLActivity>.CallStatic(
                    METHOD_CALLBACK_CHANGE_VISIBLE_STATE, 
                    LLAndroidJavaCallback.ProxyCallback(new Action<bool>(InvokeOnChangeVisibleState)));
                #endif
            }
            callbackChangeVisibleState += value;
        }
        remove
        {
            callbackChangeVisibleState -= value;
        }
    }

    #endregion


    #region Public methods

    public static void SetDebugEnable(bool value)
    {
        #if !UNITY_EDITOR
        LLAndroidJavaSingletone<LLActivity>.CallStatic(METHOD_DEBUG_ENABLE, value);
        #endif
    }


    public static void ShowToast(string text)
    {
        #if !UNITY_EDITOR
        LLAndroidJavaSingletone<LLActivity>.CallStatic(METHOD_SHOW_TOAST, text);
        #endif
    }


    public static void ShowPopup(String title, String message, bool isCancelable, LLAndroidButtonOnPopup[] buttons, bool isBoldAcceptButton = true)
    {
        CustomDebug.Log("ShowPopup " + title + ": " + message);
        int maxButtonsCount = Enum.GetNames(typeof(LLAndroidButtonOnPopup.ButtonIndex)).Length;

        int indexPositive = (int) LLAndroidButtonOnPopup.ButtonIndex.Positive;
        int indexNegative = (int) LLAndroidButtonOnPopup.ButtonIndex.Negative;
        int indexNeutral = (int) LLAndroidButtonOnPopup.ButtonIndex.Neutral;

        AndroidJavaProxy[] clickCallbacks = new AndroidJavaProxy[maxButtonsCount];
        string[] buttonNames = new string[maxButtonsCount];

        if (buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                LLAndroidButtonOnPopup button = buttons[i];

                switch (button.index)
                {
                    case LLAndroidButtonOnPopup.ButtonIndex.Positive:
                        clickCallbacks[indexPositive] = LLAndroidJavaCallback.ProxyCallback(button.clickCallback);
                        buttonNames[indexPositive] = button.label;
                        break;
                    case LLAndroidButtonOnPopup.ButtonIndex.Negative:
                        clickCallbacks[indexNegative] = LLAndroidJavaCallback.ProxyCallback(button.clickCallback);
                        buttonNames[indexNegative] = button.label;
                        break;
                    case LLAndroidButtonOnPopup.ButtonIndex.Neutral:
                        clickCallbacks[indexNeutral] = LLAndroidJavaCallback.ProxyCallback(button.clickCallback);
                        buttonNames[indexNeutral] = button.label;
                        break;
                }
            }
        }


        #if !UNITY_EDITOR 
        LLAndroidJavaSingletone<LLActivity>.CallStatic(METHOD_SHOW_POPUP, title, message, isCancelable, buttonNames,
            clickCallbacks[indexPositive], clickCallbacks[indexNegative], clickCallbacks[indexNeutral], isBoldAcceptButton);
        #endif
    }

    #endregion


    #region Delegate

    static void InvokeOnChangeVisibleState(bool isVisible)
    {
        if (callbackChangeVisibleState != null)
        {
            if (isVisible)
            {
                Scheduler.Instance.CallMethodWithDelay(Scheduler.Instance,
                    () => callbackChangeVisibleState.Invoke(isVisible), 0.0f);
            }
            else
            {
                callbackChangeVisibleState.Invoke(isVisible);
            }
        }
    }

    #endregion
    
    
    
    #region Referrer logic
    
    public static bool IsReferrerInfoValid
    {
        get
        {
            #if UNITY_EDITOR
                return false;
            #else
                return LLAndroidJavaSingletone<LLActivity>.CallStatic<bool>(METHOD_IS_REFERRER_INFO_VALID);
            #endif
        }
    }
    
    
    public static string ReferrerScheme
    {
        get
        {
            #if UNITY_EDITOR
                return string.Empty;
            #else
                return LLAndroidJavaSingletone<LLActivity>.CallStatic<string>(METHOD_GET_REFERRER_SCHEME);
            #endif
        }
    }
    
    
    public static string ReferrerInfo
    {
        get
        {
            #if UNITY_EDITOR
                return string.Empty;
            #else
                return LLAndroidJavaSingletone<LLActivity>.CallStatic<string>(METHOD_GET_REFERRER_INFO);
            #endif
        }
    }
    
    
    public static string ReferrerString
    {
        get
        {
            #if UNITY_EDITOR
                return string.Empty;
            #else
                return LLAndroidJavaSingletone<LLActivity>.CallStatic<string>(METHOD_GET_REFERRER_STRING);
            #endif
        }
    }
    
    
    public static void ProcessReferrer()
    {
        #if !UNITY_EDITOR 
            LLAndroidJavaSingletone<LLActivity>.CallStatic(METHOD_PROCESS_REFERRER);
        #endif
    }
    
    #endregion
}

#endif