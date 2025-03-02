using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public class AsyncOperationBehaviour : MonoBehaviour
{
}


public static class AsyncOperationExtension 
{
    #region Variables
    
    static AsyncOperationBehaviour asyncOperationBehaviour = null;
    static List<Coroutine> allCoroutines = new List<Coroutine>();
        
    #endregion


    #region Public methods

    public static Coroutine StartCoroutine(this IEnumerator iterator, Action finishCallback = null)
    {
        Initialize();

        Coroutine asyncCoroutine = asyncOperationBehaviour.StartCoroutine(RunTaskInner(iterator, finishCallback));
        if (asyncCoroutine != null)
        {
            allCoroutines.Add(asyncCoroutine);
        }

        return asyncCoroutine;
    }


    public static Coroutine StartCoroutine(this AsyncOperation task, Action finishCallback = null)
    {
        Initialize();

        Coroutine asyncCoroutine = asyncOperationBehaviour.StartCoroutine(RunTaskAsyncInner(task, finishCallback));
        if (asyncCoroutine != null)
        {
            allCoroutines.Add(asyncCoroutine);
        }

        return asyncCoroutine;
    }


    public static Coroutine StartCoroutine(this WWW task, Action finishCallback = null)
    {
        Initialize();

        Coroutine asyncCoroutine = asyncOperationBehaviour.StartCoroutine(RunTaskWwwInner(task, finishCallback));
        if (asyncCoroutine != null)
        {
            allCoroutines.Add(asyncCoroutine);
        }

        return asyncCoroutine;
    }


    public static void StopCoroutine(this Coroutine coroutine)
    {
        if ((coroutine != null) && (asyncOperationBehaviour))
        {
            if (allCoroutines.Contains(coroutine))
            {
                allCoroutines.Remove(coroutine);
                asyncOperationBehaviour.StopCoroutine(coroutine);
            }
        }
    }        

    #endregion
    


    #region Private methods

    static void Initialize()
    {
        if (asyncOperationBehaviour == null)
        {
            GameObject g = new GameObject();
            Object.DontDestroyOnLoad(g);
            g.name = "AsyncOperationExtensionCoroutine";
            g.hideFlags = HideFlags.HideAndDontSave;

            asyncOperationBehaviour = g.AddComponent<AsyncOperationBehaviour>();
        }
    }


    static IEnumerator RunTaskInner(IEnumerator task, Action finishCallback = null)
    {     
        while (task.MoveNext())
        {
            yield return null;
        }

        if (finishCallback != null)
        {
            finishCallback();
        }
    }


    static IEnumerator RunTaskAsyncInner(AsyncOperation task, Action finishCallback = null)
    {
        while (!task.isDone)
        {
            yield return null;
        }

        if (finishCallback != null)
        {
            finishCallback();
        }
    }


    static IEnumerator RunTaskWwwInner(WWW task, Action finishCallback = null) 
    {
        while (!task.isDone)
        {
            yield return null;
        }

        if (finishCallback != null)
        {
            finishCallback();
        }
    }

    #endregion
}