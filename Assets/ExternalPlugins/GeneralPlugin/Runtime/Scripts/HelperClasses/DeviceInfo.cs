using Modules.General;
using Modules.General.Utilities;
using Modules.General.Utilities.StorageUtility;
using System;
using System.Threading.Tasks;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using System.Text.RegularExpressions;
#endif
using HashUtilities = Modules.General.Utilities.HashUtilities;


namespace Modules.General.HelperClasses
{
    public static class DeviceInfo
    {
        #region Fields
    
        private const string DefaultAdvertisingId = "00000000-0000-0000-0000-000000000000";
        private const string DeviceIdKey = "deviceIdKey";
        private const string DeviceIdHashSalt = "z4dwrr2qbtqxl07tm9gzbeq4q3qwp9ce";
    
        private static string deviceId = string.Empty;
    
        #endregion
    
    
    
        #region Properties
    
        public static string UUID => SystemInfo.deviceUniqueIdentifier;
    
    
        public static string OperationSystemVersion
        {
            get
            {
                string result = string.Empty;
    
                #if UNITY_EDITOR
                    result = string.Empty;
                #elif UNITY_IOS
                    result = UnityEngine.iOS.Device.systemVersion;
                    result = Regex.Replace(result, "[A-Za-z ]", "");
                #elif UNITY_ANDROID
                    AndroidJavaClass versionInfo = new AndroidJavaClass("android.os.Build$VERSION");
                    result = versionInfo.GetStatic<string>("RELEASE");
                #else
                    throw new NotImplementedException("This property is not implemented for current platform");
                #endif
    
                return result;
            }
        }
    
        #endregion
    
    
    
        #region Methods
    
        public static void RequestDeviceId(Action<string> callback)
        {
            if (string.IsNullOrEmpty(deviceId))
            {
                deviceId = StorageUtility.Instance.Load(DeviceIdKey);
    
                if (string.IsNullOrEmpty(deviceId))
                {
                    Action<string> requestIdAction = currentAdvertisingId =>
                    {
                        deviceId = HashUtilities.Sha1Hash(currentAdvertisingId, DeviceIdHashSalt);
                        StorageUtility.Instance.Save(DeviceIdKey, deviceId);
    
                        callback?.Invoke(deviceId);
                    };
                    
                    #if UNITY_EDITOR
                        requestIdAction(DefaultAdvertisingId);
                    #elif UNITY_IOS
                        requestIdAction(SystemInfo.deviceUniqueIdentifier.ToLower());
                    #elif UNITY_ANDROID
                        Services.GetService<IAdvertisingIdentifier>().GetAdvertisingIdentifier(advertisingId => 
                        {
                            requestIdAction(advertisingId);
                        });
                    #else
                        requestIdAction(DefaultAdvertisingId);
                    #endif
                }
                else
                {
                    callback?.Invoke(deviceId);
                }
            }
            else
            {
                callback?.Invoke(deviceId);
            }
        }
    
    
        public static async Task<string> RequestDeviceIdAsync(int delay = 250)
        {
            if (!string.IsNullOrEmpty(deviceId))
            {
                return deviceId;
            }
    
            bool received = false;
            RequestDeviceId(id =>
            {
                if (!string.IsNullOrEmpty(id))
                {
                    deviceId = id;
                }
    
                received = true;
            });
    
            while (!received)
            {
                await Task.Delay(delay);
            }
    
            return deviceId;
        }
    
        #endregion
    }
}
