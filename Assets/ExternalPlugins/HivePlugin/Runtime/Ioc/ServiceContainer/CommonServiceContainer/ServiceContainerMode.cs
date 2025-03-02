namespace Modules.Hive.Ioc
{
    public enum ServiceContainerMode
    {
        RuntimeMultiThread = 0,

        //TODO: Implement Runtime without locks (Engine, CallSiteResolver)
        //RuntimeSingleThread = 1
    }
}
