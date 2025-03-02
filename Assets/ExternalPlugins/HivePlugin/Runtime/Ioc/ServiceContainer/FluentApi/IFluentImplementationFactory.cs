using System;


namespace Modules.Hive.Ioc
{
    public interface IFluentImplementationFactory<out TFluentInterface, in TServiceImplementation>
    {
        /// <summary>
        /// Sets an implementation factory of the service.
        /// </summary>
        /// <param name="implementationFactory">A delegate that represent an implementation factory.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        TFluentInterface SetImplementationFactory(Func<IServiceProvider, TServiceImplementation> implementationFactory);
    }
}
