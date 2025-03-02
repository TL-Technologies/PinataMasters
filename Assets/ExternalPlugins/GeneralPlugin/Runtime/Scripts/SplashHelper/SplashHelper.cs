using UnityEngine;


namespace Modules.General.Utilities
{
    public class SplashHelper : ISplashHelper
    {
        private ISplashHelper splashHelperImplementor;
        
        
        private ISplashHelper SplashHelperImplementor
        {
            get
            {
                if (splashHelperImplementor == null)
                {
                    splashHelperImplementor = 
                    #if UNITY_EDITOR
                        new SplashHelperEditor();
                    #elif UNITY_IOS
                        new SplashHelperIos();
                    #elif UNITY_ANDROID
                        new SplashHelperAndroid();
                    #else
                        new SplashHelperDummy();
                    #endif
                }
                
                return splashHelperImplementor;
            }
        }


        public Vector2 SplashSize => SplashHelperImplementor.SplashSize;


        public Texture2D LoadSplash() => SplashHelperImplementor.LoadSplash();


        public void UnloadSplash(Texture2D texture) => SplashHelperImplementor.UnloadSplash(texture);
    }
}