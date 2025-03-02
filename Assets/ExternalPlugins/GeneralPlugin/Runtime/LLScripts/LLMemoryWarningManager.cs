using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public static class LLMemoryWarningManager
{
    #region Variables

    delegate void LLMemoryManagerDelegate();

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    static extern void LLMemoryWarningInit(LLMemoryManagerDelegate callback);
    #endif

    static System.Action OnMemoryWarning;
    static bool initialized = false;

    #endregion

 
    #region Public methods

    public static void RegisterMemoryWarning(System.Action callback)
    {
        Initialize();    
        OnMemoryWarning += callback;
    } 


    public static void UnregisterMemoryWarning(System.Action callback)
    {
        Initialize();    
        OnMemoryWarning -= callback;
    }

    #endregion


    #region Private methods

    static void Initialize()
    {
        if (!initialized)
        {
            #if UNITY_IOS && !UNITY_EDITOR
            LLMemoryWarningInit(MemoryWarningCallback);
            #endif
            initialized = true;
        }
    } 

    #endregion


    #region Popup Delegate
 
    [AOT.MonoPInvokeCallbackAttribute(typeof(LLMemoryManagerDelegate))]
    static void MemoryWarningCallback()
    {
        if (OnMemoryWarning != null)
        {
            OnMemoryWarning();
        }
    }

    #endregion
}