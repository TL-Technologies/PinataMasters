using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenSizeHelper
{
    static int ScreenHeight = -1;
    static int ScreenWidth = -1;


    public static int Height
    {
        get
        {
            if (ScreenHeight < 0)
            {
                ScreenHeight = Screen.height;

                #if UNITY_EDITOR
                    SetScreenWidthAndHeightFromEditorGameViewViaReflection();
                #endif
            }
            
            return ScreenHeight;
        }
    }


    public static int Width
    {
        get
        {
            if (ScreenWidth < 0)
            {
                ScreenWidth = Screen.width;

                #if UNITY_EDITOR
                    SetScreenWidthAndHeightFromEditorGameViewViaReflection();
                #endif
            }

            return ScreenWidth;
        }
    }


    #if UNITY_EDITOR
    static void SetScreenWidthAndHeightFromEditorGameViewViaReflection()
    {
        System.Type T = System.Type.GetType("UnityEditor.GameView,UnityEditor");
        System.Reflection.MethodInfo GetMainGameView = T.GetMethod("GetSizeOfMainGameView", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
        Vector2 screenSize = (Vector2)GetMainGameView.Invoke(null, null);
        ScreenHeight = (int)screenSize.y;
        ScreenWidth = (int)screenSize.x;
    }

    #endif
}