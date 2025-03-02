namespace Modules.Hive.Ioc
{
    public interface IFluentImplementationInstance<out TFluentInterface, in TServiceImplementation>
    {
        /// <summary>
        /// Set an implementation instance of the service.
        /// Note that a service with specified implementation instance can only be a singleton.
        /// </summary>
        /// <param name="implementationInstance">An instance of the service.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface SetImplementationInstance(TServiceImplementation implementationInstance);
    }
}
