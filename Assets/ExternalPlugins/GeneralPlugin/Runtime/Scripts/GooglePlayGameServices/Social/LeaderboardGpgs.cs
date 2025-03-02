using Modules.General.Abstraction.GooglePlayGameServices;
using System;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    public class LeaderboardGpgs : ILeaderboardGpgs
    {
        #region Fields

        public const string DefaultTitle = "Default";
        public const int DefaultMaxRange = 25;
        public const LeaderboardType DefaultLeaderboardType = LeaderboardType.CurrentPlayer;
        public const TimeScope DefaultTimeScope = TimeScope.AllTime;
        
        private ISocialGpgs socialGpgs;

        #endregion
        
        
        
        #region ILeaderboardGpgs
        
        public string id { get; set; }
        
        
        public string title { get; }
        
        
        public uint maxRange { get; }
        
        
        public TimeScope timeScope { get; set; }
        
        
        public LeaderboardType LeaderboardType { get; set; }
        
        
        public bool loading { get; private set; }
        
        
        public IScore[] scores { get; private set; }

        
        public IScore localUserScore { get; private set; }
        

        public UserScope userScope
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        
        
        public Range range
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
        

        public void SetUserFilter(string[] userIDs) => throw new NotImplementedException();
        
        
        public void LoadScores(Action<bool> callback)
        {
            if (socialGpgs != null)
            {
                loading = true;
                Action<IScore[]> callbackLoadScores = HandlerUploadedScores(callback);
                socialGpgs.LoadScores(
                    id,
                    LeaderboardType.CurrentPlayer,
                    timeScope,
                    -1,
                    callbackLoadScores);
            }
            else
            {
                CustomDebug.LogError("Error: can't load scores!");
                callback?.Invoke(false);
            }


            Action<IScore[]> HandlerUploadedScores(Action<bool> internalCallback)
            {
                Action<IScore[]> handler = loadedScores =>
                {
                    if (loadedScores != null && loadedScores.Length > 0)
                    {
                        scores = loadedScores;

                        if (LeaderboardType == LeaderboardType.CurrentPlayer)
                        {
                            loading = false;
                            internalCallback?.Invoke(true);
                        }
                        else
                        {
                            LoadLocalUserScore(internalCallback);
                        }
                    }
                    else
                    {
                        loading = false;
                        internalCallback?.Invoke(false);
                    }
                };

                return handler;
            }
        }
        
        
        public void LoadLocalUserScore(Action<bool> callback)
        {
            if (socialGpgs != null)
            {
                loading = true;
                Action<IScore[]> callbackLoadUserScore = HandlerUploadedUserScore(callback);
                socialGpgs.LoadScores(
                    id,
                    LeaderboardType.CurrentPlayer,
                    timeScope,
                    -1,
                    callbackLoadUserScore);
            }
            else
            {
                CustomDebug.LogError("Error: can't load local user scores!");
                callback?.Invoke(false);
            }
            
            
            Action<IScore[]> HandlerUploadedUserScore(Action<bool> internalCallback)
            {
                Action<IScore[]> handler = loadedScore =>
                {
                    loading = false;

                    if (scores != null && scores.Length > 0)
                    {
                        localUserScore = loadedScore[0];
                        internalCallback?.Invoke(true);
                    }
                    else
                    {
                        internalCallback?.Invoke(false);
                    }
                };

                return handler;
            }
        }

        #endregion



        #region Class lifecycle

        public LeaderboardGpgs(ISocialGpgs socialGpgsImplementor)
        {
            socialGpgs = socialGpgsImplementor;
            
            title = DefaultTitle;
            maxRange = DefaultMaxRange;
            timeScope = DefaultTimeScope;
            
            LeaderboardType = DefaultLeaderboardType;
        }
        
        
        public LeaderboardGpgs(
            LeaderboardType leaderboardType,
            TimeScope scope,
            ISocialGpgs socialGpgsImplementor) : this(socialGpgsImplementor)
        {
            timeScope = scope;
            LeaderboardType = leaderboardType;
        }

        #endregion
    }
}
