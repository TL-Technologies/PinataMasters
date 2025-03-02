using System;
using System.Collections.Generic;
using System.Globalization;


namespace Modules.Analytics
{
    public static partial class CommonEvents
    {
        #region Nested types

        public enum LevelResult
        {
            Complete    = 0,
            Fail        = 1,
            Exit        = 2,
            Reload      = 3,
            Skip        = 4
        }


        public enum UnlockType
        {
            Free        = 0,
            InApp       = 1,
            Ads         = 2,
            Progress    = 3,
            Roulette    = 4,
            DailyGift   = 5,
            Other       = 6,
        }

        #endregion



        #region Fields

        private static string sessionId = "";

        #endregion



        #region Common events

        public static void SendSessionStart()
        {
            // sessionId = (DateTime.Now - DateTime.MinValue).TotalMilliseconds.ToString("F17",
            //     CultureInfo.InvariantCulture);
            //
            // AnalyticsManager.Instance.SendEvent("session_start", new Dictionary<string, string>
            // {
            //     { "session_id", sessionId },
            // });
        }


        public static void SendSessionFinish(int sessionLengthInSeconds)
        {
            // AnalyticsManager.Instance.SendEvent("session_finish", new Dictionary<string, string>
            // {
            //     { "time", sessionLengthInSeconds.ToString() },
            //     { "session_id", sessionId },
            // });
        }


        public static void SendLevelStart(
            int levelNumber, 
            string levelId = null,
            IReadOnlyDictionary<string, string> optionalParameters = null)
        {
            // var parameters = new Dictionary<string, string>
            // {
            //     { "level_number", levelNumber.ToString() },
            //     { "session_id", sessionId }
            // };
            //
            // if (!string.IsNullOrEmpty(levelId))
            // {
            //     parameters.Add("level_id", levelId);
            // }
            //
            // if (optionalParameters != null)
            // {
            //     foreach (var optionalParameterKvp in optionalParameters)
            //     {
            //         parameters.Add(optionalParameterKvp.Key, optionalParameterKvp.Value);
            //     }
            // }
            //
            // AnalyticsManager.Instance.SendEvent("level_start", parameters);
        }


        public static void SendLevelFinish(
            int levelNumber, 
            int levelTimeInSeconds, 
            LevelResult levelResult, 
            string levelId = null, 
            IReadOnlyDictionary<string, string> optionalParameters = null)
        {
            // var parameters = new Dictionary<string, string>
            // {
            //     { "level_number", levelNumber.ToString() },
            //     { "time", levelTimeInSeconds.ToString() },
            //     { "status", levelResult.ToString() },
            //     { "session_id", sessionId }
            // };
            //
            // if (!string.IsNullOrEmpty(levelId))
            // {
            //     parameters.Add("level_id", levelId);
            // }
            //
            // if (optionalParameters != null)
            // {
            //     foreach (var optionalParameterKvp in optionalParameters)
            //     {
            //         parameters.Add(optionalParameterKvp.Key, optionalParameterKvp.Value);
            //     }
            // }
            //
            // AnalyticsManager.Instance.SendEvent("level_finish", parameters);
        }


        public static void SendSubLevelStart(int levelNumber, int subLevelNumber, string levelId = null)
        {
            // var parameters = new Dictionary<string, string>
            // {
            //     { "sublevel_number", subLevelNumber.ToString() },
            //     { "level_number", levelNumber.ToString() },
            //     { "session_id", sessionId },
            // };
            //
            // if (!string.IsNullOrEmpty(levelId))
            // {
            //     parameters.Add("level_id", levelId);
            // }
            //
            // AnalyticsManager.Instance.SendEvent("sublevel_start", parameters);
        }


        public static void SendSubLevelFinish(
            int levelNumber, 
            int subLevelNumber, 
            int matchTimeInSeconds, 
            LevelResult matchResult, 
            string levelId = null)
        {
            // var parameters = new Dictionary<string, string>
            // {
            //     { "sublevel_number", subLevelNumber.ToString() },
            //     { "time", matchTimeInSeconds.ToString() },
            //     { "status", matchResult.ToString() },
            //     { "level_number", levelNumber.ToString() },
            //     { "session_id", sessionId },
            // };
            //
            // if (!string.IsNullOrEmpty(levelId))
            // {
            //     parameters.Add("level_id", levelId);
            // }
            //
            // AnalyticsManager.Instance.SendEvent("sublevel_finish", parameters);
        }


        [Obsolete("Deprecated")]
        public static void SendUnlockContent(int levelNumber, UnlockType unlockType, string contentCategory,
            string contentName)
        {
            // AnalyticsManager.Instance.SendEvent("unlock_content", new Dictionary<string, string>
            // {
            //     { "level_number", levelNumber.ToString() },
            //     { "unlock_type", unlockType.ToString() },
            //     { "category", contentCategory },
            //     { "name", contentName },
            //     { "session_id", sessionId },
            // });
        }

        #endregion
    }
}