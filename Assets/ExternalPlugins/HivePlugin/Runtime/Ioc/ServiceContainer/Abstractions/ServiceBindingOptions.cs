using System;


namespace Modules.Hive.Ioc
{
    [Flags]
    public enum ServiceBindingOptions
    {
        /// <summary>
        /// Specifies that a service binding has no any options.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that a service binding cannot be modified or overrided by another service.
        /// </summary>
        Exclusive = 1,

        /// <summary>
        /// Specifies an options by default.
        /// </summary>
        Default = None
    }
}
