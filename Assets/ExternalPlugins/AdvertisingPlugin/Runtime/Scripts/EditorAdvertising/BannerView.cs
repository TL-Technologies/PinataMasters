using System;
using UnityEngine;
using UnityEngine.UI;
using BannerPosition = Modules.Advertising.CustomBannerSettings.BannerPosition;


namespace Modules.Advertising
{
    [RequireComponent(typeof(RectTransform))]
    public class BannerView : MonoBehaviour
    {
        #region Nested Types

        private struct BannerViewSettings
        {
            public Vector2 size;
            public Vector2 anchor;
            public Vector2 positionShift;
        }

        #endregion



        #region Fields

        private const string bannerDescription = "BANNER: {0}";

        [SerializeField] private RectTransform viewRect = default;
        [SerializeField] private Text viewText = default;

        #endregion



        #region Methods

        public void Show(CustomBannerSettings bannerSettings, string placementName)
        {
            BannerViewSettings bannerViewSettings = GetBannerSettings(bannerSettings);

            viewRect.anchorMin = bannerViewSettings.anchor;
            viewRect.anchorMax = bannerViewSettings.anchor;
            viewRect.offsetMin = bannerViewSettings.positionShift;
            viewRect.offsetMax = bannerViewSettings.positionShift;
            viewRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bannerViewSettings.size.x);
            viewRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, bannerViewSettings.size.y);
            
            viewText.text = String.Format(bannerDescription, placementName);
            
            gameObject.SetActive(true);
        }


        public void Hide()
        {
            gameObject.SetActive(false);
        }


        private BannerViewSettings GetBannerSettings(CustomBannerSettings bannerSettings)
        {
            BannerViewSettings result = new BannerViewSettings();

            switch (bannerSettings.SizeType)
            {
                case CustomBannerSettings.BannerSizeType.BannerType_Custom:
                    result.size = bannerSettings.CustomBannerSize;
                    break;
                case CustomBannerSettings.BannerSizeType.BannerType_160x600:
                    result.size = new Vector2(160.0f, 600.0f);
                    break;
                case CustomBannerSettings.BannerSizeType.BannerType_300x250:
                    result.size = new Vector2(300.0f, 250.0f);
                    break;
                case CustomBannerSettings.BannerSizeType.BannerType_320x50:
                    result.size = new Vector2(320.0f, 50.0f);
                    break;
                case CustomBannerSettings.BannerSizeType.BannerType_728x90:
                    result.size = new Vector2(728.0f, 90.0f);
                    break;
            }

            result.size *= GetBannerSizeMultiplier(bannerSettings.CurrentDeviceDPI);

            switch (bannerSettings.Position)
            {
                case CustomBannerSettings.BannerPosition.Bottom:
                    result.anchor = new Vector2(0.5f, 0.0f);
                    result.positionShift = new Vector2(0.0f, 
                        bannerSettings.HomeButtonAreaSize + result.size.y / 2.0f);
                    break;
                case CustomBannerSettings.BannerPosition.BottomLeft:
                    result.anchor = new Vector2(0.0f, 0.0f);
                    result.positionShift = new Vector2(result.size.x / 2.0f, 
                        bannerSettings.HomeButtonAreaSize + result.size.y / 2.0f);
                    break;
                case CustomBannerSettings.BannerPosition.BottomRight:
                    result.anchor = new Vector2(1.0f, 0.0f);
                    result.positionShift = new Vector2(-result.size.x / 2.0f, 
                        bannerSettings.HomeButtonAreaSize + result.size.y / 2.0f);
                    break;
                case CustomBannerSettings.BannerPosition.Top:
                    result.anchor = new Vector2(0.5f, 1.0f);
                    result.positionShift = new Vector2(0.0f, -result.size.y / 2.0f);
                    break;
                case CustomBannerSettings.BannerPosition.TopLeft:
                    result.anchor = new Vector2(0.0f, 1.0f);
                    result.positionShift = new Vector2(result.size.x / 2.0f, -result.size.y / 2.0f);
                    break;
                case CustomBannerSettings.BannerPosition.TopRight:
                    result.anchor = new Vector2(1.0f, 1.0f);
                    result.positionShift = new Vector2(-result.size.x / 2.0f, -result.size.y / 2.0f);
                    break;
            }

            result.positionShift += GetNotchAreaSizeShift(bannerSettings);

            return result;
        }


        private int GetBannerSizeMultiplier(int currentDeviceDPI)
        {
           return Mathf.RoundToInt(currentDeviceDPI / 160.0f);
        }


        private Vector2 GetNotchAreaSizeShift(CustomBannerSettings bannerSettings)
        {
            Vector2 result = Vector2.zero;
            float notchAreaSize = bannerSettings.NotchAreaSize;
            BannerPosition bannerPosition = bannerSettings.Position;

            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.LandscapeLeft:
                    if (bannerPosition == CustomBannerSettings.BannerPosition.BottomRight || bannerPosition == CustomBannerSettings.BannerPosition.TopRight)
                    {
                        result = new Vector2(-notchAreaSize, 0.0f);
                    }
                    
                    break;
                
                case DeviceOrientation.LandscapeRight:
                    if (bannerPosition == CustomBannerSettings.BannerPosition.BottomLeft || bannerPosition == CustomBannerSettings.BannerPosition.TopLeft)
                    {
                        result = new Vector2(notchAreaSize, 0.0f);
                    }
                    
                    break;
                
                case DeviceOrientation.Portrait:
                    if (bannerPosition == CustomBannerSettings.BannerPosition.Top || 
                        bannerPosition == CustomBannerSettings.BannerPosition.TopLeft || 
                        bannerPosition == CustomBannerSettings.BannerPosition.TopRight)
                    {
                        result = new Vector2(0.0f, -notchAreaSize);
                    }
                    
                    break;
                
                case DeviceOrientation.PortraitUpsideDown:
                    if (bannerPosition == CustomBannerSettings.BannerPosition.Bottom || 
                        bannerPosition == CustomBannerSettings.BannerPosition.BottomLeft || 
                        bannerPosition == CustomBannerSettings.BannerPosition.BottomRight)
                    {
                        result = new Vector2(0.0f, notchAreaSize);
                    }
                    
                    break;
                
                case DeviceOrientation.Unknown:
                    
                    break;
            }

            return result;
        }

        #endregion
    }
}
