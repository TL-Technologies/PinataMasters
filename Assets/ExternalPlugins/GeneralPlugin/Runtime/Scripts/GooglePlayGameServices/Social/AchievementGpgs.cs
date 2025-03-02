using Modules.General.Abstraction.GooglePlayGameServices;
using System;
using UnityEngine;


namespace Modules.General.GooglePlayGameServices
{
    public class AchievementGpgs : IAchievementGpgs
    {
        #region Fields

        private AchievementState state;
        private AchievementType type;
        private int currentSteps;
        private int totalSteps;
        
        private ISocialGpgs socialGpgs;

        #endregion
        


        #region IAchievementGpgs

        public bool IsIncremental => type == AchievementType.Incremental;
        
        
        public string id { get; set; }
        
        
        public bool hidden => state == AchievementState.Hidden;
        
        
        public string title { get; }
        
        
        public double percentCompleted { get; set; }
        
        
        public DateTime lastReportedDate { get; }
        
        
        public string achievedDescription { get; }
        
        
        public string unachievedDescription { get; }
        
        
        public int points { get; }
        
        
        public Texture2D image { get; }
        
        
        public bool completed => state == AchievementState.Unlocked;
        
        
        public void ReportProgress(Action<bool> callback)
        {
            if (type == AchievementType.Standart)
            {
                if (socialGpgs != null)
                {
                    socialGpgs.ReportProgress(id, percentCompleted, callback);
                }
                else
                {
                    CustomDebug.LogError("Can't report progress for achievement!");
                    callback?.Invoke(false);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #endregion
        
        
        
        #region Class lifecycle
        
        public AchievementGpgs(ISocialGpgs socialGpgsImplementor)
        {
            socialGpgs = socialGpgsImplementor;
        }
        

        public AchievementGpgs(NativeAchievement achievement, ISocialGpgs socialGpgsImplementor)
        {
            socialGpgs = socialGpgsImplementor;
            
            state = achievement.state;
            type = achievement.type;
            currentSteps = achievement.currentSteps;
            totalSteps = achievement.totalSteps;
            
            id = achievement.id;
            
            percentCompleted = CalculatePercentCompleted();
            lastReportedDate = achievement.LastReportedDate;
            
            title = achievement.name;
            achievedDescription = achievement.description;
            unachievedDescription = achievedDescription;
            points = (int)achievement.xpValue;
            image = null;
            
            
            double CalculatePercentCompleted()
            {
                double percent;

                if (IsIncremental)
                {
                    if (totalSteps > 0)
                    {
                        percent = currentSteps / (double)totalSteps * 100.0f;
                    }
                    else
                    {
                        percent = 0.0;
                    }
                }
                else
                {
                    percent = completed ? 100.0f : 0.0f;
                }

                return percent;
            }
        }

        #endregion
    }
}
