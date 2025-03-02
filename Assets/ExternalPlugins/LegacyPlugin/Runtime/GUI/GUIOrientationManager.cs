using Modules.General.HelperClasses;
using System;
using UnityEngine;


public class GUIOrientationManager : SingletonMonoBehaviour<GUIOrientationManager> 
{
	#region Variables

    public static event Action<GUILayouterRotationType> OnDeviceOrientationChanged;

	static GUILayouterRotationType currentRotationType = GUILayouterRotationType.Both;
    DeviceOrientation lastDeviceOrientation;
    ScreenOrientation lastScreenOrientation;

    SizeHelperSettings cachedSizeHelperSettings;

	#endregion



	#region Properties

	public static GUILayouterRotationType CurrentRotationType
	{
		get 
        { 
            if (currentRotationType == GUILayouterRotationType.Both)
            {
                currentRotationType = (Screen.width > Screen.height) ? 
                                      (GUILayouterRotationType.Landscape) :
                                      (GUILayouterRotationType.Portrait);
            }

            return currentRotationType; 
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



	#region Unity lifecycles

	protected override void Awake()
	{
        base.Awake();

        CheckOrientation();
	}


	void Update()
	{
        CheckOrientation();
	}

	#endregion



    #region Public methods

    public void RotateToLandscape()
    {
        int height = CachedSizeHelperSettings.baseHeight;
        int width = CachedSizeHelperSettings.baseWidth;
        CachedSizeHelperSettings.baseHeight = (height > width) ? (width) : (height);
        CachedSizeHelperSettings.baseWidth = (height > width) ? (height) : (width);

        currentRotationType = GUILayouterRotationType.Landscape;

        if (OnDeviceOrientationChanged != null)
        {
            OnDeviceOrientationChanged(currentRotationType);
        }
    }


    public void RotateToPortrait()
    {
        int height = CachedSizeHelperSettings.baseHeight;
        int width = CachedSizeHelperSettings.baseWidth;
        CachedSizeHelperSettings.baseWidth = (height > width) ? (width) : (height);
        CachedSizeHelperSettings.baseHeight = (height > width) ? (height): (width);

        currentRotationType = GUILayouterRotationType.Portrait;

        if (OnDeviceOrientationChanged != null)
        {
            OnDeviceOrientationChanged(currentRotationType);
        }
    }

    #endregion



	#region Private methods

    void CheckOrientation()
    {
        #if UNITY_EDITOR
            if (Screen.height > Screen.width)
            {
                if ((lastScreenOrientation != ScreenOrientation.Portrait) &&
                    (lastScreenOrientation != ScreenOrientation.PortraitUpsideDown))
                {
                    lastScreenOrientation = ScreenOrientation.Portrait;
                    RotateToPortrait();
                }
            }
            else
            {
                if ((lastScreenOrientation != ScreenOrientation.LandscapeLeft) &&
                    (lastScreenOrientation != ScreenOrientation.LandscapeRight))
                {
                    lastScreenOrientation = ScreenOrientation.LandscapeRight;
                    RotateToLandscape();
                }
            }
        #else

            #if UNITY_IOS
                if ((Screen.autorotateToLandscapeLeft) ||
                    (Screen.autorotateToLandscapeRight) ||
                    (Screen.autorotateToPortrait) ||
                    (Screen.autorotateToPortraitUpsideDown))
                {
                    DeviceOrientation currentDeviceOrientation = Input.deviceOrientation;

                    if (currentDeviceOrientation != lastDeviceOrientation)
                    {
                        lastDeviceOrientation = currentDeviceOrientation;       

                        if (((lastDeviceOrientation == DeviceOrientation.LandscapeLeft) && (Screen.autorotateToLandscapeLeft)) ||
                            ((lastDeviceOrientation == DeviceOrientation.LandscapeRight) && (Screen.autorotateToLandscapeRight)))
                        {
                            RotateToLandscape();
                        }
                        else if (((lastDeviceOrientation == DeviceOrientation.Portrait) && (Screen.autorotateToPortrait)) ||
                            ((lastDeviceOrientation == DeviceOrientation.PortraitUpsideDown) && (Screen.autorotateToPortraitUpsideDown)))
                        {
                            RotateToPortrait();
                        }
                    }
                }
                else
            #endif

            {
                ScreenOrientation currentScreenOrientation = Screen.orientation;

                if (lastScreenOrientation != currentScreenOrientation)
                {
                    lastScreenOrientation = currentScreenOrientation;

                    if ((lastScreenOrientation == ScreenOrientation.LandscapeLeft) ||
                        (lastScreenOrientation == ScreenOrientation.LandscapeRight) || 
                        (lastScreenOrientation == ScreenOrientation.Landscape))
                    {
                        RotateToLandscape();
                    }
                    else if ((lastScreenOrientation == ScreenOrientation.Portrait) ||
                        (lastScreenOrientation == ScreenOrientation.PortraitUpsideDown))
                    {
                        RotateToPortrait();
                    }
                }
            }
        #endif

    }

	#endregion
}