namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public struct CompatibleScreen
    {
        public string size;
        public string density;
    }


    public static class CompatibleScreenSize
    {
        public const string Small = "small";
        public const string Normal = "normal";
        public const string Large = "large";
        public const string Xlarge = "xlarge";

    }


    public static class CompatibleScreenDensity
    {
        public const string LDpi = "ldpi";   // (approximately 120 dpi)
        public const string MDpi = "mdpi";   // (approximately 160 dpi)
        public const string HDpi = "hdpi";   // (approximately 240 dpi)
        public const string XhDpi = "xhdpi"; // (approximately 320 dpi)
        public const string XxhDpi = "xxhdpi"; // (approximately 480 dpi)
        public const string XxxhDpi = "xxxhdpi"; // (approximately 640 dpi)

        public const string Dpi280 = "280";
        public const string Dpi360 = "360";
        public const string Dpi420 = "420";
        public const string Dpi480 = "480";
        public const string Dpi560 = "560";
    }
}
