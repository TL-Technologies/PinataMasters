using Modules.General.Abstraction.GooglePlayGameServices;
using System;
using UnityEngine.SocialPlatforms;


namespace Modules.General.GooglePlayGameServices
{
    internal class SocialGpgsDummy : ISocialGpgs
    {
        public ILocalUser localUser => null;


        public bool IsRequestedAuthCode => false;
        
        
        public LeaderboardConfiguration LeaderboardConfiguration { get; set; }
        
        
        public void LoadUsers(string[] userIDs, Action<IUserProfile[]> callback) => callback?.Invoke(null);


        public void ReportProgress(string achievementID, double progress, Action<bool> callback) => callback?.Invoke(false);


        public void LoadAchievementDescriptions(Action<IAchievementDescription[]> callback) => callback?.Invoke(null);


        public void LoadAchievements(Action<IAchievement[]> callback) => callback?.Invoke(null);


        public IAchievement CreateAchievement() => null;


        public void ReportScore(long score, string board, Action<bool> callback) => callback?.Invoke(false);


        public void LoadScores(string leaderboardID, Action<IScore[]> callback) => callback?.Invoke(null);


        public ILeaderboard CreateLeaderboard() => null;


        public void ShowAchievementsUI() { }


        public void ShowLeaderboardUI() { }


        public void Authenticate(ILocalUser user, Action<bool> callback) => callback?.Invoke(false);


        public void Authenticate(ILocalUser user, Action<bool, string> callback) => callback?.Invoke(false, string.Empty);


        public void LoadFriends(ILocalUser user, Action<bool> callback) => callback?.Invoke(false);


        public void LoadScores(ILeaderboard board, Action<bool> callback) => callback?.Invoke(false);


        public bool GetLoading(ILeaderboard board) => false;


        public void Activate() { }


        public void SignOut() { }


        public string GetServerAuthCode() => string.Empty;


        public bool HasPermission(GooglePermission permission) => false;


        public void RequestPermission(GooglePermission permission, Action<bool> callback) => callback?.Invoke(false);


        public void LoadScores(
            string leaderboardID,
            LeaderboardType leaderboardType,
            TimeScope timeScope,
            int rowCount,
            Action<IScore[]> callback) => callback?.Invoke(null);
    }
}
