using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public static class LLRatePopupManager
{
    #region Variables

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport ("__Internal")]
    static extern bool LLRatePopIsAvalaible();
    [DllImport ("__Internal")]
    static extern void LLRatePopUpShow();    
    #endif

    #endregion


    #region Properties

    public static bool IsAvalaiblePopUp
    {
        get 
        {
            bool result = false;
            #if UNITY_IOS && !UNITY_EDITOR
            result = LLRatePopIsAvalaible();
            #endif
            return result;
        }
    }

    #endregion

 
    #region Public methods

    public static void ShowPopUp()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        LLRatePopUpShow();
        #endif
    } 

    #endregion
}