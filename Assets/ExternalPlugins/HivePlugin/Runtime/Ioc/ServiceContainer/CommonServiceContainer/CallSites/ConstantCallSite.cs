namespace Modules.Hive.Ioc
{
    internal class ConstantCallSite : ICallSite
    {
        public readonly object implementationInstance;

        
        public CallSiteKind Kind => CallSiteKind.Constant;

        
        public ConstantCallSite(object implementationInstance)
        {
            this.implementationInstance = implementationInstance;
        }


        public object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitConstantCallSite(this, scope);
    }
}
