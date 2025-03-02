using System;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Describes a binding to a service abstraction.
    /// </summary>
    public struct ServiceBinding
    {
        /// <summary>
        /// A type of service abstraction.
        /// </summary>
        public Type @Type;

        /// <summary>
        /// A binding options.
        /// </summary>
        public ServiceBindingOptions Options;
    }
}
