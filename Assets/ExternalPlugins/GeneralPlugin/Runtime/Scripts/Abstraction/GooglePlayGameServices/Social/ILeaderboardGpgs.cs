using System;
using UnityEngine.SocialPlatforms;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public interface ILeaderboardGpgs : ILeaderboard
    {
        LeaderboardType LeaderboardType { get; set; }
        void LoadLocalUserScore(Action<bool> callback);
    }
}
