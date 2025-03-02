namespace Modules.Hive.Storage
{
    public enum StorageScope
    {
        Undefined = 0,
        Roaming, // Native default
        Local, // Native isolated
        Cache,
    }
}
