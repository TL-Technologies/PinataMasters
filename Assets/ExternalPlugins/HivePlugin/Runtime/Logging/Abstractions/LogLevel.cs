using System;


namespace Modules.Hive.Logging
{
    public enum LogLevel
    {
        None    = 0,
        Error   = 1,
        Warning = 2,
        Info    = 3,
        Debug   = 4,
    }


    [Flags]
    public enum LogLevelMask
    {
        None     = 0,
        Errors   = 1 << LogLevel.Error,
        Warnings = 1 << LogLevel.Warning,
        Info     = 1 << LogLevel.Info,
        Debug    = 1 << LogLevel.Debug,
    }
}
