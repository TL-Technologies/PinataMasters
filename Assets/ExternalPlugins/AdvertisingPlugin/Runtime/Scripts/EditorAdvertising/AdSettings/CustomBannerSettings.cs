using Modules.General.Abstraction;
using System;
using UnityEngine;


namespace Modules.Advertising
{
    [Serializable]
    public class CustomBannerSettings : AdModuleSettings
    {
        #region Nested types

        public enum BannerSizeType
        {
            BannerType_320x50 = 0,
            BannerType_300x250 = 1,
            BannerType_728x90 = 2,
            BannerType_160x600 = 3,
            BannerType_Custom = 4
        }


        public enum BannerPosition
        {
            Top = 0,
            Bottom = 1,
            TopLeft = 2,
            TopRight = 3,
            BottomLeft = 4,
            BottomRight = 5
        }


        public enum DeviceDPI
        {
            None = 0,
            Iphone5 = 326,
            Iphone7Plus = 401,
            IphoneX = 463,
            IphoneXr = 324,
            IphoneXsMax = 456,
            SamsungS8 = 568,
            XiaomiRN5 = 403,
            Custom = 465
        }

        #endregion



        #region Fields

        [Header("Banner Settings")]
        [SerializeField] private BannerSizeType sizeType = BannerSizeType.BannerType_320x50;
        [SerializeField] private BannerPosition position = BannerPosition.Bottom;
        [SerializeField] private Vector2 customBannerSize = Vector2.zero;
        [SerializeField] private bool useNotchArea = true;
        [SerializeField] private float notchAreaSize = 63.0f;
        [SerializeField] private bool useHomeButtonArea = true;
        [SerializeField] private float homeButtonAreaSize = 44.0f;

        [Header("Device Settings")] [SerializeField]
        private DeviceDPI currentDeviceDPI = DeviceDPI.IphoneX;

        [SerializeField] [DeviceDpi(DeviceDPI.Custom)]
        private int customDeviceDPI = 0;

        #endregion



        #region Class lifecycle

        public CustomBannerSettings(BannerSizeType sizeType = BannerSizeType.BannerType_320x50,
            BannerPosition position = BannerPosition.Bottom,
            Vector2 customBannerSize = default, bool useNotchArea = true,
            float notchAreaSize = 63.0f, bool useHomeButtonArea = true,
            float homeButtonAreaSize = 44.0f, DeviceDPI currentDeviceDPI = DeviceDPI.IphoneX, int customDeviceDPI = 0)
        {
            this.sizeType = sizeType;
            this.position = position;
            this.customBannerSize = customBannerSize;
            this.useNotchArea = useNotchArea;
            this.notchAreaSize = notchAreaSize;
            this.useHomeButtonArea = useHomeButtonArea;
            this.homeButtonAreaSize = homeButtonAreaSize;
            this.currentDeviceDPI = currentDeviceDPI;
            this.customDeviceDPI = customDeviceDPI;
        }

        #endregion



        #region Properties

        public BannerSizeType SizeType => sizeType;


        public BannerPosition Position => position;


        public Vector2 CustomBannerSize => customBannerSize;


        public float NotchAreaSize => useNotchArea ? notchAreaSize : 0.0f;


        public float HomeButtonAreaSize => useHomeButtonArea ? homeButtonAreaSize : 0.0f;


        public override AdModule ModuleType => AdModule.Banner;


        public int CurrentDeviceDPI
        {
            get
            {
                int result;

                if (currentDeviceDPI == DeviceDPI.None || currentDeviceDPI == DeviceDPI.Custom)
                {
                    result = (customDeviceDPI == 0) ? ((int) DeviceDPI.IphoneX) : (customDeviceDPI);
                }
                else
                {
                    result = (int) currentDeviceDPI;
                }

                return result;
            }
        }

        #endregion
    }
}