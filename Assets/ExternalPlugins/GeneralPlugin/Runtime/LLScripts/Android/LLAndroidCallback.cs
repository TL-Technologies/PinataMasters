using Modules.General;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LLAndroidCallback
{
    #region Variables 

    Delegate proxyCallback;
    float time;

    #endregion



    #region Properties

    public Delegate ProxyCallback
    {
        get
        {
            return proxyCallback;
        }
    }

    #endregion


    #region Public methods

    public void Setup(Delegate callback, float t)
    {
        proxyCallback = callback;
        time = t;
    }


    public void DynamicInvoke(params object[] args)
    {
        if (ProxyCallback != null)
        {
            if (time >= 0f)
            {                
                Scheduler.Instance.CallMethodWithDelay(LLAndroidJavaCallback.AndroidJavaProxyCallback.dummyObject, delegate
                {
                    ProxyCallback.DynamicInvoke(args);
                }, time, true);
            }
            else
            {
                ProxyCallback.DynamicInvoke(args);
            }
        }
    }

    #endregion
}