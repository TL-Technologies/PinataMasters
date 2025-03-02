using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using UnityEngine;

namespace Modules.General.InitializationQueue.Sample
{
    public interface IServiceC
    {
        string ObjectName { get; }
    }


    [InitQueueService(-1, bindTo: typeof(IServiceC))]
    public class ServiceC : MonoBehaviour, IServiceC, IInitializableService
    {
        public string ObjectName { get => name; }


        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            CustomDebug.Log("ServiceC::registered");
            onCompleteCallback?.Invoke();
        }
    }
}
