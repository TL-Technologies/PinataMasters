namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Options for configuring various behaviors of the default <see cref="ServiceContainer"/> implementation.
    /// </summary>
    public class ServiceContainerOptions
    {
        /// <summary>
        /// Gets a default options.
        /// </summary>
        internal static readonly ServiceContainerOptions Default = new ServiceContainerOptions();

        
        /// <summary>
        /// Gets or sets a service container operation mode.
        /// </summary>
        public ServiceContainerMode Mode { get; set; } = ServiceContainerMode.RuntimeMultiThread;

        
        /// <summary>
        /// Gets or sets whether transient objects should be captured by <see cref="IServiceScope"/> and disposed with it.
        /// </summary>
        public bool CaptureDisposableTransientServices { get; set; } = false;

        
        /// <summary>
        /// Gets or sets whether a dependency injector will use variable default values for not registered services.
        /// </summary>
        public bool AllowToInjectDefaults { get; set; } = false;
    }
}
