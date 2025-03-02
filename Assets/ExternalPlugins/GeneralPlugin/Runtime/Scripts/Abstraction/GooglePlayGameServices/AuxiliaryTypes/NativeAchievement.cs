using System;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public class NativeAchievement
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        
        public string id;
        public string name;
        public string description;
        public long xpValue;
        public AchievementState state;
        public AchievementType type;
        public long lastUpdatedTimestamp; 
        public int currentSteps;
        public int totalSteps;
        public string revealedImageUri;
        public string unlockedImageUri;
        
        
        public DateTime LastReportedDate => UnixEpoch.AddMilliseconds(lastUpdatedTimestamp);
        
        
        public override string ToString() =>
            $"LNativeAchievement] {nameof(id)}={id}, {nameof(name)}={name}, {nameof(description)}={description}, " +
            $"{nameof(xpValue)}={xpValue}, {nameof(state)}={state}, {nameof(type)}={type}, " +
            $"{nameof(lastUpdatedTimestamp)}={lastUpdatedTimestamp}, steps={currentSteps}/{totalSteps}, " +
            $"{nameof(revealedImageUri)}={revealedImageUri}, {nameof(unlockedImageUri)}={unlockedImageUri}.";
    }
}
