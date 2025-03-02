using System;
using System.Threading;
using UnityEngine;


namespace Modules.General
{
    public class ThreadDispatcher : MonoBehaviour
    {
        private static ThreadDispatcher instance = null;
        private Thread mainThread = null;


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            if (instance == null) 
            {
                GameObject dispatcherGameObject = new GameObject("ThreadDispatcher");
                instance = dispatcherGameObject.AddComponent<ThreadDispatcher>();
                dispatcherGameObject.hideFlags = HideFlags.HideAndDontSave;
                DontDestroyOnLoad(dispatcherGameObject);
            }
        }
        
        
        private void Awake()
        {
            mainThread = Thread.CurrentThread;
        }
        
        
        public static bool IsMainThread(Thread currentThread)
        {
            if (instance == null || instance.mainThread == null)
            {
                // The only way to get here is call method with RuntimeInitializeOnLoadMethod attribute, 
                // that means your call is already in main Unity thread.
                return true;
            }
            
            return currentThread == instance.mainThread;
        }
        
        
        public static void CheckThreadAttach(Action callback)
        {
            bool isMainThread = IsMainThread(Thread.CurrentThread);
            if (!isMainThread)
            {
                AndroidJNI.AttachCurrentThread();
            }
            
            callback();
            
            if (!isMainThread)
            {
                AndroidJNI.DetachCurrentThread();
            }
        }
    }
}
