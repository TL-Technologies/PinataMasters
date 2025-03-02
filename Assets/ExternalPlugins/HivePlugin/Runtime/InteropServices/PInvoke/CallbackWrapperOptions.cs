using System;


namespace Modules.Hive.InteropServices.PInvoke
{
    [Flags]
    public enum CallbackWrapperOptions
    {
        None = 0,
        Transient = 1,

        Default = Transient
    }
}
