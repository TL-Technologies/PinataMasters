using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using AOT;

public class LLApplicationWebViewController : MonoBehaviour 
{
    #region Variables

    #if UNITY_IOS && !UNITY_EDITOR 

    [DllImport ("__Internal")]
    static extern void LLApplicationWebViewControllerShow(string url, string title);

    #endif

    #endregion


    #region Public methods

    public static void ShowUrlInApplication(string url, string title)
    {
        #if UNITY_IOS && !UNITY_EDITOR 
        LLApplicationWebViewControllerShow(url, title);
        #elif UNITY_ANDROID && !UNITY_EDITOR
        Application.OpenURL(url);
        #endif
    }

    #endregion
}
