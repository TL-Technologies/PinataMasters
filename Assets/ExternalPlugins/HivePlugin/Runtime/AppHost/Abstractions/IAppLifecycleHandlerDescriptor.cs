namespace Modules.Hive
{
    public interface IAppLifecycleHandlerDescriptor
    {
        IAppLifecycleHandler Handler { get; }
        AppHostLayer Layer { get; }
        int Order { get; }
    }
}
