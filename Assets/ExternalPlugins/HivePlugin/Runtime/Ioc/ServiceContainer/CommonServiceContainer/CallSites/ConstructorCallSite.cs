using System;
using System.Reflection;


namespace Modules.Hive.Ioc
{
    internal class ConstructorCallSite : ICallSite
    {
        public readonly ConstructorInfo constructorInfo;
        public readonly Type[] serviceTypes;

        
        public CallSiteKind Kind => CallSiteKind.Constructor;
        
        
        public ConstructorCallSite(ConstructorInfo constructorInfo, Type[] serviceTypes)
        {
            this.constructorInfo = constructorInfo;
            this.serviceTypes = serviceTypes;
        }

        
        public object Accept(ICallSiteVisitor visitor, ServiceProviderEngineScope scope) => visitor.VisitConstructorCallSite(this, scope);
    }
}
