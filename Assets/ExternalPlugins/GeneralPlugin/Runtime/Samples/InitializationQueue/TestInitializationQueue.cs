using Modules.General.ServicesInitialization;
using UnityEngine;


namespace Modules.General.InitializationQueue.Sample
{
    public class TestInitializationQueue : MonoBehaviour
    {
        [SerializeField] private GameObject serviceCPrefab = null;


        private void Awake()
        {
            InitializationQueue.Instance.AddService<ServiceA>()
                                .AddService<ServiceB>()
                                .AddService<ServiceC>(serviceCPrefab)
                                .SetOnComplete(OnInitComplete)
                                .SetOnError(OnInitError)
                                .Apply();
        }


        private void OnInitError(object service, InitializationStatus errorCode)
        {
            CustomDebug.LogError("Initialization queue failed!");
        }


        private void OnInitComplete()
        {
            var a = Services.GetService<IServiceA>();
            a?.Test();

            var b = Services.GetService<IServiceB>();
            b?.Test();

            var c = Services.GetService<IServiceC>();
            CustomDebug.Log($"Object name from GameObject based service = {c?.ObjectName}");
        }
    }
}
