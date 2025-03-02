using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;


public class LLCountryCodeRegister : MonoBehaviour 
{
    #if UNITY_IOS && !UNITY_EDITOR

    [DllImport ("__Internal")]
    static extern System.IntPtr LLCountryCodeRegisterLocalPlayerCountryCode();

    #endif
    

    #region Properties

    public static string LocalPlayerCountryCode
    {
        get
        {
            string result = "BY";

            #if UNITY_IOS && !UNITY_EDITOR
            result = Marshal.PtrToStringAnsi(LLCountryCodeRegisterLocalPlayerCountryCode());
            #endif

            return result;
        }
    }

    #endregion
}
