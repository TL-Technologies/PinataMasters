using Modules.General.HelperClasses;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;


namespace Modules.General.Utilities
{
    public class SplashHelperAndroid : ISplashHelper
    {
        private const string GetMetaDataMethodName = "UtilitiesGetMetaDataInt";
        private const string SplashMetaDataKey = "unity.splash-mode";
        
        
        public Vector2 SplashSize { get; private set; }


        public Texture2D LoadSplash()
        {
            string texturePath = Application.streamingAssetsPath.RemoveLastPathComponent();
            texturePath = $"{texturePath}/res/drawable/unity_static_splash.png";
            
            Texture2D result = null;
            using (UnityWebRequest request = UnityWebRequest.Get(texturePath))
            {
                request.SendWebRequest();

                while (!request.isDone)
                {
                    Thread.Sleep(5);
                }
            
                if (!request.isNetworkError && !request.isHttpError)
                {
                    DownloadHandler handler = request.downloadHandler;
                    if (handler != null)
                    {
                        result = Texture2D.CreateExternalTexture(
                            4,
                            4,
                            TextureFormat.RGBA32,
                            false,
                            false,
                            Texture2D.blackTexture.GetNativeTexturePtr());
                        result.LoadImage(handler.data);
                    }
                }
            }
            
            if (result != null)
            {
                // Additional texture setup
                result.hideFlags = HideFlags.DontSave;
                result.wrapMode = TextureWrapMode.Clamp;
            
                DetermineSplashSize();
            }
            
            return result;
            
            
            void DetermineSplashSize()
            {
                if (result == null)
                {
                    return;
                }
                
                int splashType = LLAndroidJavaSingletone<Utilities>.CallStatic<int>(
                    GetMetaDataMethodName,
                    SplashMetaDataKey);
                
                SplashSize = SplashHelperUtility.CalculateSplashSize(
                    new Vector2(result.width, result.height),
                    splashType);
            }
        }


        public void UnloadSplash(Texture2D texture) { }
    }
}
