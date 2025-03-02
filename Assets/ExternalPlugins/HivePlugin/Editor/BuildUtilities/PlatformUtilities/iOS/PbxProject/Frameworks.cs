namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public struct Framework
    {
        public static Framework Accelerate                  => new Framework("Accelerate.framework");
        public static Framework Accounts                    => new Framework("Accounts.framework");
        public static Framework AdServices                  => new Framework("AdServices.framework");
        public static Framework AdSupport                   => new Framework("AdSupport.framework");
        public static Framework AppTrackingTransparency     => new Framework("AppTrackingTransparency.framework");
        public static Framework AudioToolbox                => new Framework("AudioToolbox.framework");
        public static Framework AVFoundation                => new Framework("AVFoundation.framework");
        public static Framework AVKit                       => new Framework("AVKit.framework");
        public static Framework CoreBluetooth               => new Framework("CoreBluetooth.framework");
        public static Framework CoreData                    => new Framework("CoreData.framework");
        public static Framework CoreFoundation              => new Framework("CoreFoundation.framework");
        public static Framework CoreGraphics                => new Framework("CoreGraphics.framework");
        public static Framework CoreImage                   => new Framework("CoreImage.framework");
        public static Framework CoreLocation                => new Framework("CoreLocation.framework");
        public static Framework CoreMedia                   => new Framework("CoreMedia.framework");
        public static Framework CoreMotion                  => new Framework("CoreMotion.framework");
        public static Framework CoreTelephony               => new Framework("CoreTelephony.framework");
        public static Framework CoreVideo                   => new Framework("CoreVideo.framework");
        public static Framework CFNetwork                   => new Framework("CFNetwork.framework");
        public static Framework EventKit                    => new Framework("EventKit.framework");
        public static Framework EventKitUI                  => new Framework("EventKitUI.framework");
        public static Framework Foundation                  => new Framework("Foundation.framework");
        public static Framework GameKit                     => new Framework("GameKit.framework");
        public static Framework GLKit                       => new Framework("GLKit.framework");
        public static Framework IAd                         => new Framework("iAd.framework");
        public static Framework ImageIO                     => new Framework("ImageIO.framework");
        public static Framework JavaScriptCore              => new Framework("JavaScriptCore.framework");
        public static Framework LocalAuthentication         => new Framework("LocalAuthentication.framework");
        public static Framework MapKit                      => new Framework("MapKit.framework");
        public static Framework MediaPlayer                 => new Framework("MediaPlayer.framework");
        public static Framework MediaToolbox                => new Framework("MediaToolbox.framework");
        public static Framework MessageUI                   => new Framework("MessageUI.framework");
        public static Framework Metal                       => new Framework("Metal.framework");
        public static Framework MetalKit                    => new Framework("MetalKit.framework");
        public static Framework MobileCoreServices          => new Framework("MobileCoreServices.framework");
        public static Framework PassKit                     => new Framework("PassKit.framework");
        public static Framework QuartzCore                  => new Framework("QuartzCore.framework");
        public static Framework SafariServices              => new Framework("SafariServices.framework");
        public static Framework Security                    => new Framework("Security.framework");
        public static Framework Social                      => new Framework("Social.framework");
        public static Framework StoreKit                    => new Framework("StoreKit.framework");
        public static Framework SystemConfiguration         => new Framework("SystemConfiguration.framework");
        public static Framework Twitter                     => new Framework("Twitter.framework");
        public static Framework UIKit                       => new Framework("UIKit.framework");
        public static Framework WebKit                      => new Framework("WebKit.framework");


        /// <summary>
        /// Gets a reference to the framework
        /// </summary>
        public string Reference { get; }


        private Framework(string reference)
        {
            Reference = reference;
        }
    }
}
