using UnityEngine.SocialPlatforms;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public interface IAchievementGpgs : IAchievementDescription, IAchievement
    {
        bool IsIncremental { get; }
    }
}
