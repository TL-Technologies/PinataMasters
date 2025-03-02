using System;
using UnityEngine.SocialPlatforms;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public interface ISocialGpgs : ISocialPlatform
    {
        bool IsRequestedAuthCode { get; }
        LeaderboardConfiguration LeaderboardConfiguration { get; set; }
        
        void Activate();
        void SignOut();
        string GetServerAuthCode();
        bool HasPermission(GooglePermission permission);
        void RequestPermission(GooglePermission permission, Action<bool> callback);
        void LoadScores(
            string leaderboardID,
            LeaderboardType leaderboardType,
            TimeScope timeScope,
            int rowCount,
            Action<IScore[]> callback);
    }
}
