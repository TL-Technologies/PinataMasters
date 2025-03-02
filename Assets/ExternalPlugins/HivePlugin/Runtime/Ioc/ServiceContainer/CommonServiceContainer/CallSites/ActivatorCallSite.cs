using System;


namespace Modules.Hive.Ioc
{
    internal class ActivatorCallSite : ICallSite
    {
        public readonly Type implementationType;

        
        public CallSiteKind Kind => CallSiteKind.Activator;
        
        
        public ActivatorCallSite(Type implementationType)
        {
            this.implementationType = implementationType;
        }

        
        public object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitActivatorCallSite(this, scope);
    }
}
