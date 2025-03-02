using Newtonsoft.Json.Linq;


namespace Modules.General.Abstraction
{
    /// <summary>
    /// The interface allows to access to persistent data of A/B test service
    /// </summary>
    public interface IAbTestServiceData
    {
        /// <summary>
        /// Gets or sets a format version of the manifest
        /// </summary>
        string ManifestVersion { get; set; }

        /// <summary>
        /// Gets or sets raw json data of the A/B test manifest.
        /// Do not forget to call <see cref="SetModified"/> after changing it.
        /// </summary>
        JObject ManifestData { get; set; }

        /// <summary>
        /// Gets whether the data is modified and should be saved later.
        /// </summary>
        
        bool IsModified { get; }
        
        IAbTestManifestInfo GetManifestInfo();
        
        /// <summary>
        /// Sets that the data is changed and should be saved later.
        /// </summary>
        void SetModified();
    }
}
