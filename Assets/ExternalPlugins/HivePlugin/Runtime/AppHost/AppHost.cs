using System;


namespace Modules.Hive
{
    public static class AppHost
    {
        private static IAppHost instance = null;
        

        public static IAppHost Instance
        {
            get => instance;
            internal set => instance = value;
        }
        

        public static void AttachAppHost(IAppHost appHost)
        {
            if (instance == appHost)
            {
                return;
            }

            if (instance != null && appHost != null)
            {
                throw new InvalidOperationException("Another application host is already active.");
            }

            instance = appHost;
        }
        

        public static void DetachAppHost(IAppHost appHost)
        {
            if (instance != appHost)
            {
                // TODO: Get logger
                UnityEngine.Debug.LogWarning("Failed to detach application host because it is not attached.");
                return;
            }

            instance = null;
        }
        

        public static IAppHostBuilder CreateDefault()
        {
            return new CommonAppHostBuilder()
                .AddStorageService();
        }
    }
}
