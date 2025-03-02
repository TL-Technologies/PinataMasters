namespace Modules.Hive.Ioc
{
    internal class SingletonCallSite : ScopedCallSite
    {
        public override CallSiteKind Kind => CallSiteKind.Singleton;
        
        
        public SingletonCallSite(ICallSite callSite, object cacheKey) : base(callSite, cacheKey) { }


        public override object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitSingletonCallSite(this, scope);
    }
}
