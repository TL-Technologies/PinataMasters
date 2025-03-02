using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public static class LLSwipeHandlerManager
{
    #region Variables

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void LLSwipeHandlerUpdate();
    [DllImport("__Internal")]
    private static extern string LLSwipeHandlerPopHistory();
    #endif

    #endregion

 
    #region Public methods

    public static void Update()
    {
        #if UNITY_IOS && !UNITY_EDITOR
        LLSwipeHandlerUpdate();
        #endif
    } 


    public static string PopHistory()
    {
        string result = string.Empty;
        #if UNITY_IOS && !UNITY_EDITOR
        result = LLSwipeHandlerPopHistory();
        #endif
        return result;
    }

    #endregion
}