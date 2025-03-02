using Modules.General;
using UnityEngine;


public sealed class LLAndroidJavaSingletone<T>
{
    private static AndroidJavaClass instance;
    private static string className;
    
    
    private static AndroidJavaClass Instance
    {
        get
        {
            if (instance == null)
            {
                ThreadDispatcher.CheckThreadAttach(() =>
                {
                    if (string.IsNullOrEmpty(className))
                    {
                        className = typeof(T).Name;
                    }
                
                    string classPath = string.Format("com.lllibset.{0}.{1}", className, className);
                    instance = new AndroidJavaClass(classPath);
                });
            }
            return instance;
        }
    }


    public static void RegisterClassName(string name)
    {
        className = name;
    }


    public static void CallStatic(string methodName, params object[] args)
    {
        ThreadDispatcher.CheckThreadAttach(() =>
        {
            Instance.CallStatic(methodName, args);
        });
    }


    public static TReturnType CallStatic<TReturnType>(string methodName, params object[] args)
    {
        TReturnType result = default;
        
        ThreadDispatcher.CheckThreadAttach(() =>
        {
            result = Instance.CallStatic<TReturnType>(methodName, args);
        });
        
        return result;
    }
}


public static class LLAndroidJavaCallback 
{
    // TODO: There is a problem with functioning in a non-main thread. Details link: https://clck.ru/NJ9eP
    
    public static AndroidJavaProxy ProxyCallback(System.Action callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string,bool> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string,int> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string,string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string,string,bool,bool> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<bool> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<int> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string, string, string, int> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string, string, int> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string, string, int, int> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<int, bool> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<string[]> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<int, string, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<int, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public static AndroidJavaProxy ProxyCallback(System.Action<int, int, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }

    
    public static AndroidJavaProxy ProxyCallback(System.Action<int, int, string, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }
    

    public static AndroidJavaProxy ProxyCallback(System.Action<int, int, int, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }
    
    
    public static AndroidJavaProxy ProxyCallback(System.Action<int, int, int, string, string> callback, float time = -1)
    {
        AndroidJavaProxy result = null;
        ThreadDispatcher.CheckThreadAttach(() => result = new AndroidJavaProxyCallback(callback, time));
        return result;
    }


    public sealed class AndroidJavaProxyCallback : AndroidJavaProxy
    {
        #region Variables

        static string LLLIBSET_INTERFACE_CALLBACK = "com.lllibset.LLActivity.ILLLibSetCallback";
        public static GameObject dummyObject;

        LLAndroidCallback proxyCallback;

        #endregion


        #region Unity lifecycle

        public AndroidJavaProxyCallback(System.Delegate callback, float time)
            : base(LLLIBSET_INTERFACE_CALLBACK)
        {
            if(dummyObject == null)
            {
                dummyObject = new GameObject("AndroidCallbacksDummy");
            }
            proxyCallback = new LLAndroidCallback();
            proxyCallback.Setup(callback, time);
        }

        #endregion


        #region Events

        public void OnCallback()
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke();
            }
        }
        
        
        public void OnCallback(string message)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(message);
            }
        }
        
        
        public void OnCallback(string messageString, bool messageBool)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString, messageBool);
            }
        }
        
        
        public void OnCallback(string messageString, int messageInt)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString, messageInt);
            }
        }
        
        
        public void OnCallback(string messageString1, string messageString2)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString1, messageString2);
            }
        }
        
        
        public void OnCallback(string messageString1, string messageString2, bool messageBool1, bool messageBool2)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString1, messageString2, messageBool1, messageBool2);
            }
        }
        
        
        public void OnCallback(bool messageBool)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageBool);
            }
        }
        
        
        public void OnCallback(int messageInt)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageInt);
            }
        }
        
        
        public void OnCallback(string messageString1, string messageString2, string messageString3, int messageInt)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString1, messageString2, messageString3, messageInt);
            }
        }
        
        
        public void OnCallback(string messageString1, string messageString2, int messageInt)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString1, messageString2, messageInt);
            }
        }
        
        
        public void OnCallback(string messageString1, string messageString2, int messageInt1, int messageInt2)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageString1, messageString2, messageInt1, messageInt2);
            }
        }
        
        
        public void OnCallback(int messageInt, bool messageBool)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(messageInt, messageBool);
            }
        }
        
        
        public void OnCallback(string[] messageStringArray)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke((object)messageStringArray);
            }
        }


        public void OnCallback(int v1, string v2, string v3)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2, v3);
            }
        }


        public void OnCallback(int v1, string v2)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2);
            }
        }


        public void OnCallback(int v1, int v2, string v3)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2, v3);
            }
        }
        
        
        public void OnCallback(int v1, int v2, string v3, string v4)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2, v3, v4);
            }
        }


        public void OnCallback(int v1, int v2, int v3, string v4)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2, v3, v4);
            }
        }
        
        
        public void OnCallback(int v1, int v2, int v3, string v4, string v5)
        {
            if (proxyCallback != null)
            {
                proxyCallback.DynamicInvoke(v1, v2, v3, v4, v5);
            }
        }

        #endregion
    }
}