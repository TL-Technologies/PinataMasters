namespace Modules.Hive
{
    internal class AppLifecycleHandlerDescriptor : IAppLifecycleHandlerDescriptor
    {
        public IAppLifecycleHandler Handler { get; }
        public AppHostLayer Layer { get; }
        public int Order { get; }

        public AppLifecycleHandlerDescriptor(
            IAppLifecycleHandler handler,
            AppHostLayer layer = AppHostLayer.Default,
            int order = 0)
        {
            Handler = handler;
            Layer = layer;
            Order = order;
        }
    }
}
