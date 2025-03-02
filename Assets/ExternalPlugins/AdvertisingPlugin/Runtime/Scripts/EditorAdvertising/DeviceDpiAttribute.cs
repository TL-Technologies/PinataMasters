using UnityEngine;


namespace Modules.Advertising
{
    public class DeviceDpiAttribute : PropertyAttribute
    {
        public CustomBannerSettings.DeviceDPI DpiType { get; }

        public DeviceDpiAttribute(CustomBannerSettings.DeviceDPI dpiType)
        {
            DpiType = dpiType;
        }
    }
}