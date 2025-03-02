using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    // Source https://developer.android.com/reference/android/R.attr.html#protectionLevel
    [Flags]
    public enum PermissionProtectionLevel
    {
        Normal = 0x0,
        Dangerous = 0x1,
        Signature = 0x2,
        Privileged = 0x10,
        Development = 0x20,

        AppOp = 0x40,
        Pre23 = 0x80,
        Installer = 0x100,
        Preinstalled = 0x400,
        Instant = 0x1000,
    }
}
