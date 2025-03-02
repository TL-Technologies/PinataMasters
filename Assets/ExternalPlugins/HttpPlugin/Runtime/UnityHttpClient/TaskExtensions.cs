using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


namespace Http
{
    public static class TaskExtensions
    {
        public static async Task DelayWithoutException(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }
    
            try
            {
                await Task.Delay(millisecondsTimeout, cancellationToken);
            }
            catch
            {
                // exception suppressed
            }
        }
    
    
        public static async Task WaitForInternetReachableAsync(CancellationToken cancellationToken)
        {
            while (Application.internetReachability == NetworkReachability.NotReachable)
            {
                await Task.Delay(1000, cancellationToken);
            }
        }
    
    
        private static SynchronizationContext GetSynchronizationContext()
        {
            if (SynchronizationContext.Current == null)
            {
                Debug.LogError("Current SynchronizationContext is null.");
                throw new NullReferenceException("Current SynchronizationContext is null.");
            }
    
            return SynchronizationContext.Current;
        }
    
    
        public static void PostToOnUnityThread<T>(Action<T> action, T arg1)
        {
            if (action == null)
            {
                return;
            }
    
            //Progress<AdType> progress = new Progress<AdType>(CrossPromoService.Instance.RaiseAdAvailableEvent);
            //IProgress<AdType> p = progress;
            //p.Report(AdType);
    
            GetSynchronizationContext().Post(state => action(arg1), null);
        }
    
    
        public static void PostToOnUnityThread<T1, T2>(Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action == null)
            {
                return;
            }
    
            GetSynchronizationContext().Post(state => action(arg1, arg2), null);
        }
    }
}
