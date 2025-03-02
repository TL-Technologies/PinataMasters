namespace Modules.Hive.Ioc
{
    internal class ScopedCallSite : ICallSite
    {
        internal ICallSite CallSite { get; }
        public object CacheKey { get; }
        
        public virtual CallSiteKind Kind => CallSiteKind.Scope;

        
        public ScopedCallSite(ICallSite callSite, object cacheKey)
        {
            CallSite = callSite;
            CacheKey = cacheKey;
        }

        public virtual object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitScopedCallSite(this, scope);
    }
}
