using System;


namespace Modules.Hive.Editor
{
    [Flags]
    public enum FileSystemOperationOptions
    {
        None = 0,
        Override = 1,
        IgnoreMetaFiles = 2,

        Default = Override | IgnoreMetaFiles
    }
}
