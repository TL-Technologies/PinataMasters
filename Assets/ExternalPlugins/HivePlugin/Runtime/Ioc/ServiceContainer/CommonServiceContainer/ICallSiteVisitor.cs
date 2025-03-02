namespace Modules.Hive.Ioc
{
    internal interface ICallSiteVisitor
    {
        object VisitTransientCallSite(TransientCallSite callSite, ServiceProviderEngineScope scope);
        object VisitScopedCallSite(ScopedCallSite callSite, ServiceProviderEngineScope scope);
        object VisitSingletonCallSite(SingletonCallSite callSite, ServiceProviderEngineScope scope);
        object VisitActivatorCallSite(ActivatorCallSite callSite, ServiceProviderEngineScope scope);
        object VisitConstructorCallSite(ConstructorCallSite callSite, ServiceProviderEngineScope scope);
        object VisitConstantCallSite(ConstantCallSite callSite, ServiceProviderEngineScope scope);
        object VisitFactoryCallSite(FactoryCallSite callSite, ServiceProviderEngineScope scope);        
    }
}
