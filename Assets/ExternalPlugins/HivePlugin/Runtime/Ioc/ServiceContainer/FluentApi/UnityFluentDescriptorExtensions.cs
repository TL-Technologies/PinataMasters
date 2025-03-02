namespace Modules.Hive.Ioc
{
    // TODO: Implement UnityFluentDescriptorExtensions

    public static class UnityFluentDescriptorExtensions
    {
        public static IUnityFluentDescriptor<T> CreateUnityService<T>(this IServiceContainer container)
            where T : UnityEngine.Object
        {
            return new UnityFluentDescriptor<T>(container);
        }
    }
}
