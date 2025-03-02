using Modules.General.HelperClasses;


namespace PinataMasters
{
    public static class Matches
    {
        #region Variables

        private const string MATCHES = "matches";

        #endregion



        #region Properties

        public static int Count
        {
            get
            {
                return CustomPlayerPrefs.GetInt(MATCHES, 0);
            }
            private set
            {
                CustomPlayerPrefs.SetInt(MATCHES, value);
                GameAnalytics.SetMatchesUserProperty();
            }
        }

        #endregion



        #region Public methods

        public static void Increment()
        {
            Count++;
        }

        #endregion
    }
}
