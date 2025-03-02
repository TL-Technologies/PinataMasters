#if UNITY_IOS
using Modules.General.HelperClasses;
using UnityEngine;
using UnityEngine.iOS;


namespace Modules.General.Utilities
{
    public class SplashHelperIos : ISplashHelper
    {
        public Vector2 SplashSize { get; private set; }


        public Texture2D LoadSplash()
        {
            bool isLandscapeOrientation = Screen.width > Screen.height;
            
            string texturePath = Application.streamingAssetsPath.RemoveLastPathComponent().RemoveLastPathComponent();
            if (IsIpadWithClassicRatio())
            {
                texturePath = $"{texturePath}/LaunchScreen-iPad.png";
            }
            else
            {
                texturePath = isLandscapeOrientation ? 
                    $"{texturePath}/LaunchScreen-iPhoneLandscape.png" : 
                    $"{texturePath}/LaunchScreen-iPhonePortrait.png";
            }
            
            Texture2D result = TextureManager.LoadTexture(texturePath, false);
            
            if (result != null)
            {
                SplashSize = SplashHelperUtility.CalculateSplashSize(
                    new Vector2(result.width, result.height));
            }

            return result;
        }


        public void UnloadSplash(Texture2D texture)
        {
            TextureManager.UnloadTexture(texture);
        }
        
        
        private bool IsIpadWithClassicRatio()
        {
            float currentWidth = Screen.width; 
            float currentHeight = Screen.height; 
            bool isSquareScreen = (Mathf.Max(currentWidth, currentHeight) / Mathf.Min(currentWidth, currentHeight)) < 1.34f;

            bool isIpad = Device.generation.ToString().Contains("iPad");

            return isIpad && isSquareScreen;
        }
    }
}
#endif
