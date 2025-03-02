namespace Modules.Hive
{
    public interface IAppLifecycleHandler
    {
        void OnAppLaunch();
        //void OnAppEnterForeground();
        void OnAppResume();
        void OnAppPause();
        //void OnAppEnterBackground();
        void OnAppQuit();
    }
}
