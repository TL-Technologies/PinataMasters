using Modules.General.HelperClasses;


namespace PinataMasters
{
    public static class Sessions
    {
        #region Variables

        private const string SESSIONS_COUNT = "sessions_count";

        #endregion



        #region Properties

        public static int Count
        {
            get
            {
                return CustomPlayerPrefs.GetInt(SESSIONS_COUNT, 0);
            }
            private set
            {
                CustomPlayerPrefs.SetInt(SESSIONS_COUNT, value);
                GameAnalytics.SetSessionsUserProperty();
            }
        }

        #endregion


        #region Public methods

        public static void IncrementSession()
        {
            Count++;
        }

        #endregion
    }
}
