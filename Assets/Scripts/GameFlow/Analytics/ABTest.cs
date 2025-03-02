using Modules.General;
using Modules.General.HelperClasses;
using System.Collections.Generic;


namespace PinataMasters
{
    public static class ABTest
    {
        #region Fields

        const string NameAutotap = "autotap";
        const string NameAutotapTrue = "true";
        const string NameAutotapFalse = "false";

        const string NameDifficulty = "difficulty";
        const string NameDifficultyNormal = "normal";
        const string NameDifficultyEasy = "easy";

        const string EventTestRemote = "ab_test_remote";
        const string PrefsSendTestRemote = "ABTestSendTestRemote";

        private static InGameAbTestData inGameAbTestData = null;

        #endregion



        #region Properties

        public static bool AutotapTrue
        {
            get
            {
                return InGameAbTestData.autotap == NameAutotapTrue;
            }
        }


        public static bool DifficultyEasy
        {
            get
            {
                return InGameAbTestData.difficulty == NameDifficultyEasy;
            }
        }

        
        public static InGameAbTestData InGameAbTestData
        {
            get
            {
                if (!IsAbTestsInitialized)
                {
                    CustomDebug.LogError("Do not use AbTest before initilalized");

                    return new InGameAbTestData();
                }

                return inGameAbTestData;
            }
            set
            {
                inGameAbTestData = value;
            }
        }


        public static bool IsAbTestsInitialized { get; set; }
   
        #endregion



        #region Public Methods

        public static void SendTestRemote()
        {
//            if (CustomPlayerPrefs.GetBool(PrefsSendTestRemote)) return;
//
//            CustomPlayerPrefs.SetBool(PrefsSendTestRemote, true);
//            Dictionary<string, string> parameters = new Dictionary<string, string>
//            {
//                [NameAutotap] = AutotapTrue ? NameAutotapTrue : NameAutotapFalse,
//                [NameDifficulty] = DifficultyEasy ? NameDifficultyEasy : NameDifficultyNormal
//            };
//
//            Services.AnalyticsManager.SendEvent(typeof(FirebaseAnalyticsServiceImplementor), EventTestRemote, parameters);
        }

        #endregion
    }
}
