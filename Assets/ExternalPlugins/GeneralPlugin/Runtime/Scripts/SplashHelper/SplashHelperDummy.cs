using UnityEngine;


namespace Modules.General.Utilities
{
    public class SplashHelperDummy : ISplashHelper
    {
        public Vector2 SplashSize => throw new System.NotImplementedException();
        public Texture2D LoadSplash() => throw new System.NotImplementedException();
        public void UnloadSplash(Texture2D texture) => throw new System.NotImplementedException();
    }
}