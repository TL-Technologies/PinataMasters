using UnityEngine;


namespace Modules.Hive
{
    internal class CommonAppHostBehaviour : MonoBehaviour
    {
        internal CommonAppHost appHost = null;

        
        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                appHost.ProcessAppPause();
            }
            else
            {
                appHost.ProcessAppResume();
            }
        }
        

        private void Update()
        {
            appHost.ProcessUpdate();
        }
        

        private void OnDestroy()
        {
            appHost.ProcessAppQuit();
        }
    }
}
