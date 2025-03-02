using System;
using UnityEngine;


namespace Modules.General.Utilities
{
    public static class SplashHelperUtility
    {
        public static Vector2 CalculateSplashSize(Vector2 textureSize, int splashType = 0)
        {
            float textureWidth = textureSize.x;
            float textureHeight = textureSize.y;
            float widthRatio = textureWidth / Screen.width;
            float heightRatio = textureHeight / Screen.height;
            
            float ratio =
                #if UNITY_ANDROID
                    CalculateAndroidSplashRatio();
                #elif UNITY_IOS
                    CalculateIosSplashRatio();
                #else
                    CalculateIosSplashRatio();
                #endif
            
            return new Vector2(textureWidth / ratio, textureHeight / ratio);
            
            
            float CalculateAndroidSplashRatio()
            {
                float result;
                // SplashType int values correspond to UnityEditor.AndroidSplashScreenScale enum values
                if (splashType == 0) // AndroidSplashScreenScale.Center
                {
                    result = Mathf.Max(widthRatio, heightRatio, 1.0f);
                }
                else if (splashType == 1) // AndroidSplashScreenScale.ScaleToFit
                {
                    result = Mathf.Max(widthRatio, heightRatio);
                }
                else if (splashType == 2) // AndroidSplashScreenScale.ScaleToFill
                {
                    result = Mathf.Min(widthRatio, heightRatio);
                }
                else
                {
                    throw new NotImplementedException($"Splash type with value {splashType} not implemented!");
                }
                
                return result;
            }
            
            
            float CalculateIosSplashRatio()
            {
                return Mathf.Min(widthRatio, heightRatio);
            }
        }
    }
}