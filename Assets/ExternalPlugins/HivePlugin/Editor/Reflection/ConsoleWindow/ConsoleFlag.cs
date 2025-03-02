using System;


// From https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/ConsoleWindow.cs
namespace Modules.Hive.Editor.Reflection
{
    [Flags]
    public enum ConsoleFlag
    {
        Collapse = 1 << 0,
        ClearOnPlay = 1 << 1,
        ErrorPause = 1 << 2,
        Verbose = 1 << 3,
        StopForAssert = 1 << 4,
        StopForError = 1 << 5,
        Autoscroll = 1 << 6,
        LogLevelLog = 1 << 7,
        LogLevelWarning = 1 << 8,
        LogLevelError = 1 << 9,
        ShowTimestamp = 1 << 10,
        ClearOnBuild = 1 << 11,
    }
}