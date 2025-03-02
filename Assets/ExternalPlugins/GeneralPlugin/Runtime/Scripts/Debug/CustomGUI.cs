using Modules.General.HelperClasses;
using System;
using UnityEngine;


public class CustomGUI : SingletonMonoBehaviour<CustomGUI> 
{
    #region Fields
    
    public static event Action OnDebugGUI;

    static bool needShowDebugGui;
    static bool needShowDebugSwitch;
    static bool? isDevicePlatform;
    
    [SerializeField] bool shouldStopTouches;
    [SerializeField] int clicksCount = 5;
    [SerializeField] float secondsForClicks = 2.0f;
    [SerializeField] Vector2 clicksRectMinRelativePosition;
    [SerializeField] Vector2 clicksRectMaxRelativePosition;
    
    float[] clickTimes;
    int clickTimesIndex;

    #endregion
    
    
    
    #region Properties
    
    bool IsDevicePlatform
    {
        get
        {
            if (!isDevicePlatform.HasValue)
            {
                isDevicePlatform = Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;
            }
            
            return isDevicePlatform.Value;
        }
    }
    
    
    bool IsSimultaneousTouchesSwitchAvailable => IsDevicePlatform && Input.touchCount == 4;
    
    
    bool IsRightMouseButtonSwitchAvailable => !IsDevicePlatform && Input.GetMouseButtonDown(1);
    
    
    bool IsMultipleClicksSwitchAvailable
    {
        get
        {
            bool result = true;
            float timeSinceStartup = Time.realtimeSinceStartup;
            
            for (int i = 0; i < clickTimes.Length; i++)
            {
                if (clickTimes[i] < timeSinceStartup - secondsForClicks)
                {
                    result = false;
                    break;
                }
            }
            
            return result;
        }
    }
    
    #endregion
    


    #region Unity lifecycle

    protected override void Awake()
    {
        base.Awake();
        
        if (CustomDebug.Enable)
        {
            clickTimes = new float[clicksCount];
            ResetClicks();
        }
        else
        {
            enabled = false;
            Destroy(this);
        }
    }


    void Update()
    {
        CheckClicksCount();
        
        if (IsSimultaneousTouchesSwitchAvailable ||
            IsRightMouseButtonSwitchAvailable ||
            IsMultipleClicksSwitchAvailable)
        {
            if (!needShowDebugSwitch)
            {
                needShowDebugSwitch = true;
                needShowDebugGui = !needShowDebugGui;
                
                ResetClicks();
            }
        }
        else
        {
            if (needShowDebugSwitch)
            {
                needShowDebugSwitch = false;
            }
        }
        
        if (needShowDebugGui && shouldStopTouches)
        {
            Input.ResetInputAxes();
        }
    }
    
    
    void OnGUI()
    {
        if (needShowDebugGui)
        {
            OnDebugGUI?.Invoke();
        }
    }
    
    #endregion
    
    
    
    #region Private methods
    
    void CheckClicksCount()
    {
        Vector2? inputPosition = null;
        if (IsDevicePlatform)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    inputPosition = touch.position;
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            inputPosition = Input.mousePosition;
        }
        
        if (inputPosition.HasValue && IsClickInClicksRect(inputPosition.Value))
        {
            clickTimes[clickTimesIndex] = Time.realtimeSinceStartup;
            clickTimesIndex = (clickTimesIndex + 1) % clicksCount;
        }
    }
    
    
    void ResetClicks()
    {
        for (int i = 0; i < clickTimes.Length; i++)
        {
            clickTimes[i] = float.MinValue;
        }
    }
    
    
    bool IsClickInClicksRect(Vector2 clickPosition)
    {
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        
        return clickPosition.x >= screenWidth * clicksRectMinRelativePosition.x &&
            clickPosition.x <= screenWidth * clicksRectMaxRelativePosition.x &&
            clickPosition.y >= screenHeight * clicksRectMinRelativePosition.y &&
            clickPosition.y <= screenHeight * clicksRectMaxRelativePosition.y;
    }
    
    #endregion
}