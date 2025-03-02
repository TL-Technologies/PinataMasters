using UnityEngine;


public static class ScreenDimensions
{
    #region Variables

    public const int IPHONE_6_HEIGHT        = 1334; // --> 640x1136
    public const int IPHONE_6_PLUS_HEIGHT   = 1920; // --> 1242x2208
    public const int IPAD_PRO_HEIGHT        = 2732; // --> 1536x2048
    public const int IPAD_PRO_HEIGHT_ZOOM   = 2730; // --> 1536x2048
    public const int IPHONE_X_HEIGHT        = 2436; // --> 1242x2688
    public const int IPHONE_XR_HEIGHT       = 1792; // --> 750x1624
    public const int IPHONE_XS_MAX_HEIGHT   = 2688; // --> 1242x2688 (don't need change)

    #if UNITY_ANDROID
    public static readonly Vector2 ANDROID_REFERENCE_SCREEN = new Vector2(1136.0f * 2.0f, 640.0f * 2.0f);
    public const float ANDROID_DOWNSCALE_RATIO              = 1.9f;
    public const float ANDROID_DOWNSCALE_MULTIPLIER         = 1.1f;
    #endif

    #endregion



    #region Properties

    public static int Width
    {
        get
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            int result = screenWidth;

            #if UNITY_EDITOR
            float width;
            float height;
            float aspect;
            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            result = (int)width;
            #endif

            #if UNITY_IOS
            if (screenHeight == IPHONE_6_PLUS_HEIGHT)
            {
                result = 1242;
            }
            else if (screenWidth == IPHONE_6_PLUS_HEIGHT)
            {
                result = 2208;
            }
            else if (screenHeight == IPAD_PRO_HEIGHT || screenHeight == IPAD_PRO_HEIGHT_ZOOM)
            {
                result = 1536;
            }
            else if (screenWidth == IPAD_PRO_HEIGHT || screenWidth == IPAD_PRO_HEIGHT_ZOOM)
            {
                result = 2048;
            }
            else if (screenHeight == IPHONE_6_HEIGHT)
            {
                result = 640;
            }
            else if (screenWidth == IPHONE_6_HEIGHT)
            {
                result = 1136;
            }
            else if (screenHeight == IPHONE_X_HEIGHT)
            {
                result = 1242;
            }
            else if (screenWidth == IPHONE_X_HEIGHT)
            {
                result = 2688;
            }      
            else if (screenHeight == IPHONE_XR_HEIGHT)
            {
                result = 750;
            }      
            else if (screenWidth == IPHONE_XR_HEIGHT)            
            {                
                result = 1624;            
            }
            #elif UNITY_ANDROID
            float retinaMultiplier = (tk2dSystem.IsRetina) ? (1f) : (0.5f);
            result = (int)(result * AndroidScreenMultiplier * retinaMultiplier);
            #endif
            
            if ((result & 1) != 0)
            {
                result += 1;
            }

            return result;
        }
    }


    public static int Height
    {
        get
        {
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            int result = screenHeight;           

            #if UNITY_EDITOR
            float width;
            float height;
            float aspect;
            tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);

            result = (int)height;
            #endif

            #if UNITY_IOS
            if (screenHeight == IPHONE_6_PLUS_HEIGHT)
            {
                result = 2208;
            }
            else if (screenWidth == IPHONE_6_PLUS_HEIGHT)
            {
                result = 1242;
            }
            else if (screenHeight == IPAD_PRO_HEIGHT || screenHeight == IPAD_PRO_HEIGHT_ZOOM)
            {
                result = 2048;
            }
            else if (screenWidth == IPAD_PRO_HEIGHT || screenWidth == IPAD_PRO_HEIGHT_ZOOM)
            {
                result = 1536;
            }
            else if (screenHeight == IPHONE_6_HEIGHT)
            {
                result = 1136;
            }
            else if (screenWidth == IPHONE_6_HEIGHT)
            {
                result = 640;
            }
            else if (screenHeight == IPHONE_X_HEIGHT)
            {
                result = 2688;
            }
            else if (screenWidth == IPHONE_X_HEIGHT)
            {
                result = 1242;
            }            
            else if (screenHeight == IPHONE_XR_HEIGHT)            
            {                
                result = 1624;            
            }
            else if (screenWidth == IPHONE_XR_HEIGHT)
            {
                result = 750;
            }
            #elif UNITY_ANDROID
            float retinaMultiplier = (tk2dSystem.IsRetina) ? (1f) : (0.5f);
            result = (int)(result * AndroidScreenMultiplier * retinaMultiplier);
            #endif

            if ((result & 1) != 0)
            {
                result += 1;
            }
            
            return result;
        }
    }


    #if UNITY_ANDROID
    public static float AndroidScreenMultiplier
    {
        get
        {
            float maxScreenSize = Mathf.Max(Screen.width, Screen.height);
            float maxReferenceSize = Mathf.Max(ANDROID_REFERENCE_SCREEN.x, ANDROID_REFERENCE_SCREEN.y);    
            float result = maxReferenceSize / maxScreenSize;

            if (IsAndroidDeviceTall)
            {
                result *= ANDROID_DOWNSCALE_MULTIPLIER;
            }

            return result;
        }
    }


    public static bool IsAndroidDeviceTall
    {
        get
        {
            bool result = false;
            int screenWidth = Screen.width;
            int screenHeight = Screen.height;
            float minScreenSize = Mathf.Min(screenWidth, screenHeight);
            float maxScreenSize = Mathf.Max(screenWidth, screenHeight);

            if (maxScreenSize >= ANDROID_DOWNSCALE_RATIO * minScreenSize)
            {
                result = true;
            }

            return result;
        }
    }
    #endif

    #endregion
}
