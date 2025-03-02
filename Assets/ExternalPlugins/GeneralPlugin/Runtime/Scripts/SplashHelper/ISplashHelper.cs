using UnityEngine;


namespace Modules.General.Utilities
{
    public interface ISplashHelper
    {
        Vector2 SplashSize { get; }
        Texture2D LoadSplash();
        void UnloadSplash(Texture2D texture);
    }
}