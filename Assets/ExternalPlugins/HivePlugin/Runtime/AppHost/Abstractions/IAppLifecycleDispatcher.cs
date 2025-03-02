namespace Modules.Hive
{
    public interface IAppLifecycleDispatcher
    {
        int Count { get; }
        void Add(IAppLifecycleHandlerDescriptor descriptor);
        void Remove(IAppLifecycleHandlerDescriptor descriptor);
        void Remove(IAppLifecycleHandler handler);
        bool Contains(IAppLifecycleHandlerDescriptor descriptor);
        bool Contains(IAppLifecycleHandler handler);
        bool TryGetDescriptor(IAppLifecycleHandler handler, out IAppLifecycleHandlerDescriptor descriptor);
    }


    public static class AppLifecycleDispatcherExtensions
    {
        public static IAppLifecycleHandlerDescriptor Add(this IAppLifecycleDispatcher dispatcher, IAppLifecycleHandler handler)
        {
            var descriptor = new AppLifecycleHandlerDescriptor(handler);
            dispatcher.Add(descriptor);
            return descriptor;
        }


        public static IAppLifecycleHandlerDescriptor Add(this IAppLifecycleDispatcher dispatcher, IAppLifecycleHandler handler, int order)
        {
            var descriptor = new AppLifecycleHandlerDescriptor(handler, order: order);
            dispatcher.Add(descriptor);
            return descriptor;
        }


        public static IAppLifecycleHandlerDescriptor Add(this IAppLifecycleDispatcher dispatcher, IAppLifecycleHandler handler, AppHostLayer layer)
        {
            var descriptor = new AppLifecycleHandlerDescriptor(handler, layer);
            dispatcher.Add(descriptor);
            return descriptor;
        }


        public static IAppLifecycleHandlerDescriptor Add(this IAppLifecycleDispatcher dispatcher, IAppLifecycleHandler handler, AppHostLayer layer, int order)
        {
            var descriptor = new AppLifecycleHandlerDescriptor(handler, layer, order);
            dispatcher.Add(descriptor);
            return descriptor;
        }
    }
}
