using System;
using System.Collections.Generic;


namespace Modules.Hive.Ioc
{
    internal static class ServiceProviderUtilities
    {
        /// <summary>
        /// Checks whether a <paramref name="serviceType"/> is a collection of services.
        /// </summary>
        /// <param name="serviceType">A service type to check.</param>
        /// <returns>true, if specified type describes a collection of services; otherwise - false.</returns>
        public static bool IsServiceCollectionType(Type serviceType)
        {
            if (!serviceType.IsConstructedGenericType)
            {
                return false;
            }

            Type genericType = serviceType.GetGenericTypeDefinition();
            if (genericType != typeof(IReadOnlyList<>) &&
                genericType != typeof(IReadOnlyCollection<>) &&
                genericType != typeof(IEnumerable<>))
            {
                return false;
            }

            return true;
        }

        
        /// <summary>
        /// Unable to resolve service for type '{0}' that required to activate another service of type '{1}'.
        /// </summary>
        internal static string CannotResolveServiceExceptionMessage(object p0, object p1)
            => string.Format("Unable to resolve service for type '{0}' that required to activate another service of type '{1}'.", p0, p1);

        
        /// <summary>
        /// A circular dependency was detected for the service of type '{0}'.
        /// </summary>
        internal static string CircularDependencyExceptionMessageFormat
            => "A circular dependency was detected for the service of type '{0}'.";

        
        /// <summary>
        /// A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.
        /// </summary>
        internal static string NoConstructorMatchExceptionMessage(object p0)
            => string.Format("A suitable constructor for type '{0}' could not be located. Ensure the type is concrete and services are registered for all parameters of a public constructor.", p0);

        
        /// <summary>
        /// Unable to activate type '{0}'. The following constructors are ambiguous:
        /// </summary>
        internal static string AmbiguousConstructorExceptionMessage(object p0)
            => string.Format("Unable to activate type '{0}'. The following constructors are ambiguous:", p0);

        
        /// <summary>
        /// No constructor for type '{0}' can be instantiated using services from the service container and default values.
        /// </summary>
        internal static string UnableToActivateTypeExceptionMessage(object p0)
            => string.Format("No constructor for type '{0}' can be instantiated using services from the service container and default values.", p0);

        
        /// <summary>
        /// Unable to add a new service of type '{0}'. The service container already contains a service with the same type.
        /// </summary>
        internal static string CannotAddServiceExceptionMessage(object p0)
            => string.Format("Unable to add a new service of type '{0}'. The service container already contains a service with the same type.", p0);

        
        /// <summary>
        /// Cannot instantiate implementation type '{0}'.
        /// </summary>
        internal static string TypeCannotBeActivatedExceptionMessage(object p0)
            => string.Format("Cannot instantiate implementation type '{0}'.", p0);

        
        /// <summary>
        /// A service of type '{0}' cannot be resolved by type '{1}'. Ensure the service of type '{0}' inherits all required abstractions.
        /// </summary>
        internal static string InvalidServiceAbstractionTypeExceptionMessage(object p0, object p1)
            => string.Format("A service of type '{0}' cannot be resolved by type '{1}'. Ensure the service of type '{0}' inherits all required abstractions.", p0, p1);

        
        /// <summary>
        /// A service type '{0}' is not assignable from service implementation type '{1}'.
        /// </summary>
        internal static string InvalidServiceImplementationTypeExceptionMessage(object p0, object p1)
            => string.Format("A service type '{0}' is not assignable from service implementation type '{1}'.", p0, p1);

        
        /// <summary>
        /// Unable to make a binding between a service of type '{0}' and a service abstraction of type '{1}' because the abstraction is already reserved by another service and marked as exclusive.
        /// </summary>
        internal static string CannotOverrideExclusiveBindingExceptionMessage(object p0, object p1)
            => string.Format("Unable to make a binding between a service of type '{0}' and a service abstraction of type '{1}' because the abstraction is already reserved by another service and marked as exclusive.", p0, p1);

        
        /// <summary>
        /// Unable to make an exclusive binding between a service of type '{0}' and a service abstraction of type '{1}' because the abstraction is already reserved by other services.
        /// </summary>
        internal static string CannotApplyExclusiveBindingExceptionMessage(object p0, object p1)
            => string.Format("Unable to make an exclusive binding between a service of type '{0}' and a service abstraction of type '{1}' because the abstraction is already reserved by other services.", p0, p1);


        /// <summary>
        /// There is no service of type '{0}'.
        /// </summary>
        internal static string MissingServiceExceptionMessage(object p0)
            => string.Format("There is no service of type '{0}'.", p0);
    }
}
