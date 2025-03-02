using Modules.General.Abstraction.GooglePlayGameServices;
using System;


namespace Modules.General.GooglePlayGameServices
{
    public class ScoreGpgs : IScoreGpgs
    {
        #region Fields

        private ISocialGpgs socialGpgs;

        #endregion



        #region IScoreGpgs

        public string leaderboardID { get; set; }


        public long value { get; set; }


        public DateTime date { get; }


        public string formattedValue { get; }


        public string userID { get; }


        public int rank { get; }
        
        
        public void ReportScore(Action<bool> callback)
        {
            if (socialGpgs != null)
            {
                socialGpgs.ReportScore(value, leaderboardID, callback);
            }
            else
            {
                CustomDebug.LogError("Can't report score!!!");
                callback?.Invoke(false);
            }
        }


        #endregion



        #region Class lifecycle

        public ScoreGpgs(string leaderboardId, NativeScore score, ISocialGpgs socialGpgsImplementor)
        {
            socialGpgs = socialGpgsImplementor;
            
            leaderboardID = leaderboardId;
            value = score.score;
            date = score.Date;
            formattedValue = score.displayScore;
            userID = score.playerId;
            rank = (int)score.rank;
        }

        #endregion
    }
}
