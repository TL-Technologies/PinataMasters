using System;


// Source: https://developer.apple.com/library/archive/documentation/General/Reference/InfoPlistKeyReference/Articles/CocoaKeys.html#//apple_ref/doc/uid/TP40009251-SW3
namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public struct PrivacyDataUsage
    {
        /// <summary>
        /// Specifies the reason for your app to use the device’s NFC reader.
        /// <para>Platforms: iOS 11 and later.</para>
        /// </summary>
        public static PrivacyDataUsage NfcReader => new PrivacyDataUsage("NFCReaderUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to use the media library.
        /// <para>Platforms: iOS.</para>
        /// </summary>
        public static PrivacyDataUsage AppleMusic => new PrivacyDataUsage("NSAppleMusicUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to use Bluetooth.
        /// <para>Starts with iOS 13 it changes to <see cref="BluetoothAlways"/>.</para>
        /// <para>Platforms: iOS 6.0 to iOS 13.0.</para>
        /// </summary>
        public static PrivacyDataUsage BluetoothPeripheral => new PrivacyDataUsage("NSBluetoothPeripheralUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to use Bluetooth.
        /// <para>Platforms: iOS 13.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage BluetoothAlways => new PrivacyDataUsage("NSBluetoothAlwaysUsageDescription");        

        /// <summary>
        /// Specifies the reason for your app to access the user’s calendars.
        /// <para>Platforms: iOS 6.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Calendars => new PrivacyDataUsage("NSCalendarsUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the device’s camera.
        /// <para>Platforms: iOS 7.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Camera => new PrivacyDataUsage("NSCameraUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s contacts.
        /// <para>Platforms: iOS 6.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Contacts => new PrivacyDataUsage("NSContactsUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to use Face ID.
        /// <para>Platforms: iOS 11 and later.</para>
        /// </summary>
        public static PrivacyDataUsage FaceId => new PrivacyDataUsage("NSFaceIDUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to read the user’s health data.
        /// <para>Platforms: iOS 8.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage HealthShare => new PrivacyDataUsage("NSHealthShareUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to make changes to the user’s health data.
        /// <para>Platforms: iOS 8.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage HealthUpdate => new PrivacyDataUsage("NSHealthUpdateUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s HomeKit configuration data.
        /// <para>Platforms: iOS.</para>
        /// </summary>
        public static PrivacyDataUsage HomeKit => new PrivacyDataUsage("NSHomeKitUsageDescription");

        /// <summary>
        /// Unused. Use NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription instead.
        /// <para>Platforms: iOS 6.0 and later.</para>
        /// </summary>
        [Obsolete("Unused. Use NSLocationWhenInUseUsageDescription or NSLocationAlwaysUsageDescription instead.")]
        public static PrivacyDataUsage Location => new PrivacyDataUsage("NSLocationUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s location information at all times.
        /// <para>Platforms: iOS 8.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage LocationAlways => new PrivacyDataUsage("NSLocationAlwaysUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s location information while your app is in use.
        /// <para>Platforms: iOS 8.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage LocationWhenInUse => new PrivacyDataUsage("NSLocationWhenInUseUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access any of the device’s microphones.
        /// <para>Platforms: iOS 7.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Microphone => new PrivacyDataUsage("NSMicrophoneUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the device’s accelerometer.
        /// <para>Platforms: iOS 7.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Motion => new PrivacyDataUsage("NSMotionUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to get write-only access to the user’s photo library.
        /// <para>Platforms: iOS 11 and later.</para>
        /// </summary>
        public static PrivacyDataUsage PhotoLibraryAdd => new PrivacyDataUsage("NSPhotoLibraryAddUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s photo library.
        /// <para>Platforms: iOS 6.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage PhotoLibrary => new PrivacyDataUsage("NSPhotoLibraryUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access the user’s reminders.
        /// <para>Platforms: iOS 6.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage Reminders => new PrivacyDataUsage("NSRemindersUsageDescription");

        /// <summary>
        /// Specifies the reason for your app to access app-related data for recording the user or the device.
        /// <para>Platforms: iOS 14.0 and later.</para>
        /// </summary>
        public static PrivacyDataUsage UserTracking => new PrivacyDataUsage("NSUserTrackingUsageDescription");
        
        /// <summary>
        /// Specifies the reason for your app to access the user’s TV provider account.
        /// <para>Platforms: tvOS.</para>
        /// </summary>
        public static PrivacyDataUsage VideoSubscriberAccount => new PrivacyDataUsage("NSVideoSubscriberAccountUsageDescription");


        /// <summary>
        /// The key that describes a privacy data usage.
        /// </summary>
        public string Key { get; }


        private PrivacyDataUsage(string key)
        {
            Key = key;
        }
    }
}
