namespace Modules.Hive
{
    /// <summary>
    /// Application Layer:     (Usual plugins)
    /// PreApplication Layer:  ()
    /// PostInternal Layer:    (Storage plugin, Permission plugin, ...)
    /// Internal Layer:        (Foundation, Native App Host)
    /// </summary>
    public enum AppHostLayer
    {
        /// <summary>
        /// The default layer for all common plugins.
        /// </summary>
        Application = 0,

        /// <summary>
        /// The default layer for all common plugins, which should be started before other plugins on the <see cref="Application"/> layer.
        /// </summary>
        PreApplication = 10,

        // ........
        // reserved
        // ........

        /// <summary>
        /// The layer for privileged plugins, which deeply integrated into general.
        /// </summary>
        PostInternal = 90,

        /// <summary>
        /// <para>Reserved! Don't use it directly!</para>
        /// The layer for internal services of the general. For example, AppHost. 
        /// </summary>
        Internal = 100,
        
        /// <summary>
        /// Default layer. It's similar to <see cref="Application"/>.
        /// </summary>
        Default = Application
    }
}
