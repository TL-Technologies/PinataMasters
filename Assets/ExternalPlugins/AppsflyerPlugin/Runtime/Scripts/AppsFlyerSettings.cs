namespace Modules.AppsFlyer
{
    public class AppsFlyerSettings
    {
        #region Properties

        public string ApplicationIdentifier { get; set; } = string.Empty;
        public string DeveloperKey { get; set; } = string.Empty;
        public bool IsDebugEnabled { get; set; } = false;

        #endregion



        #region Class lifecycle

        public AppsFlyerSettings(string appId, string devKey, bool? isDebug = null)
        {
            ApplicationIdentifier = appId;
            DeveloperKey = devKey;

            if (!isDebug.HasValue)
            {
                IsDebugEnabled = CustomDebug.Enable;
            }
            else
            {
                IsDebugEnabled = isDebug.Value;
            }
        }


        // It is necessary during inheritance to create an instance that will be populated by fields.
        // Example: Ab testing.
        protected AppsFlyerSettings() { }

        #endregion
    }
}