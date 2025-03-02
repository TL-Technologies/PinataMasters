using System;


namespace Modules.Hive.Ioc
{
    internal class FactoryCallSite : ICallSite
    {
        public readonly Func<IServiceProvider, object> implementationFactory;

        
        public CallSiteKind Kind => CallSiteKind.Factory;
        
        
        public FactoryCallSite(Func<IServiceProvider, object> implementationFactory)
        {
            this.implementationFactory = implementationFactory;
        }
        
        
        public object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitFactoryCallSite(this, scope);
    }
}
