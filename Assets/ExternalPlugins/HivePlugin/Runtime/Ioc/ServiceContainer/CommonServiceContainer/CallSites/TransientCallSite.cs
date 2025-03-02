namespace Modules.Hive.Ioc
{
    internal class TransientCallSite : ICallSite
    {
        internal ICallSite CallSite { get; }
        
        public CallSiteKind Kind => CallSiteKind.Transient;
        

        public TransientCallSite(ICallSite callSite)
        {
            CallSite = callSite;
        }


        public object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitTransientCallSite(this, scope);
    }
}
