using AOT;
using System;
using System.Runtime.InteropServices;


namespace Modules.General
{
    public class LLApplicationStateRegister 
    {
        #region Fields
    
        private static event Action<bool> onApplicationEnteredBackground;
        public static event Action<bool> OnApplicationEnteredBackground
        {
            add
            {
                lock (eventSubscriberLock)
                {
                    if (!isInitialized)
                    {
                        #if UNITY_IOS && !UNITY_EDITOR 
                            LLApplicationStateRegisterInit(ApplicationChangedState);
                        #elif UNITY_ANDROID && !UNITY_EDITOR
                            LLActivity.OnChangeVisibleState += ApplicationChangedState;
                        #endif
    
                        isInitialized = true;
                    }
                
                    onApplicationEnteredBackground += value;
                }
    
            }
            remove
            {
                lock (eventSubscriberLock)
                {
                    onApplicationEnteredBackground -= value;
                }
            }
        }
    
        #if UNITY_IOS && !UNITY_EDITOR 
        [DllImport ("__Internal")]
        static extern void LLApplicationStateRegisterInit(Action<bool> callback);
        #endif
    
        private static bool isInitialized;
        private static object eventSubscriberLock = new object();
    
        #endregion
    
    
    
        #region Events hanlders
    
        [MonoPInvokeCallback(typeof(Action<bool>))]
        private static void ApplicationChangedState(bool result)
        {
            #if UNITY_ANDROID
                result = !result;
            #endif
            
            onApplicationEnteredBackground?.Invoke(result);
        }
    
        #endregion
    }
}

