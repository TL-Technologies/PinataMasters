namespace Modules.Hive.Ioc
{
    internal enum CallSiteKind
    {
        Transient,
        Scope,
        Singleton,
        Activator,
        Constructor,
        Constant,
        Factory,
    }

    /// <summary>
    /// Summary description for ICallSite
    /// </summary>
    internal interface ICallSite
    {
        CallSiteKind Kind { get; }

        object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope);
    }
}
