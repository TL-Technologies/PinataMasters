using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(GUILayouter))]
public class GUILayoutUpdater : MonoBehaviour 
{
    #if UNITY_EDITOR

    #region Variables

    [ButtonAttribute][SerializeField] string reset = "Reset";

    GUILayouter targetLayouter;
    SizeHelperSettings cachedSizeHelperSettings;

    int frameCount;
    int fps = 30;

    #endregion



    #region Properties

    public GUILayouter TargetLayouter
    {
        get
        {
            if (targetLayouter == null)
            {
                targetLayouter = GetComponent<GUILayouter>();
            }

            return targetLayouter;
        }
    }   


    SizeHelperSettings CachedSizeHelperSettings
    {
        get
        {
            if (cachedSizeHelperSettings == null)
            {
                cachedSizeHelperSettings = SizeHelperSettings.Instance;
            }

            return cachedSizeHelperSettings;
        }
    }

    #endregion



    #region Unity lifecycle

    void Update()
    {
        if (TargetLayouter != null && 
            TargetLayouter.IsRootLayouter)
        {
            bool isDirty = true;

            if (EditorApplication.isPlaying)
            {
                frameCount++;
                if (frameCount > fps)
                {
                    frameCount = 0;
                }
                else
                {
                    isDirty = false;
                }
            }

            if (isDirty)
            {
                ResetLayouters();

                Vector2 debugScreenSize = Vector2.zero;
                if (CachedSizeHelperSettings.isLayoutUpdaterUseScreenDimensions)
                {
                    debugScreenSize.Set(ScreenDimensions.Width, ScreenDimensions.Height);
                }
                else
                {
                    float width;
                    float height;
                    float aspect;
                    tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);
                    debugScreenSize.Set(width, height);
                }

                TargetLayouter.UpdateLayoutDebug(debugScreenSize);
            }
        }
    }

    #endregion



    #region Private methods

    void ResetLayouters()
    {
        if (TargetLayouter != null && 
            TargetLayouter.IsRootLayouter)
        {
            GUILayouter[] childLayouters = GetComponentsInChildren<GUILayouter>();

            foreach (var l in childLayouters)
            {
                l.ResetLayouter();
            }

            TargetLayouter.ResetLayouter();
        }
    }


    void Reset()
    {
        if (!string.IsNullOrEmpty(reset))
        {
            if (TargetLayouter != null && 
                TargetLayouter.IsRootLayouter)
            {
                GUILayouter[] childLayouters = GetComponentsInChildren<GUILayouter>();
                foreach (var l in childLayouters)
                {
                    l.ResetLayouter();
                }
                TargetLayouter.ResetLayouter();

                GUILayoutCell[] cells = GetComponentsInChildren<GUILayoutCell>();
                foreach (var c in cells)
                {
                    c.ResetLayoutHandlers();
                }
            }
        }
    }

    #endregion

    #endif
}