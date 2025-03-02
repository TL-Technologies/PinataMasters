using System;
using System.Linq;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleTaskExtensions
    {
        private const string TaskAtTopSemantic = "HIVE_TASKS_TOP";
        private const GradleType TaskAtTopGradleType = GradleType.Main;
        private const string TaskAtBottomSemantic = "HIVE_TASKS_BOTTOM";
        private const GradleType TaskAtBottomGradleType = GradleType.Main;


        public static void AddTask(this GradleScript script, params GradleTask[] tasks)
        {
            script.AddTask(GradleTaskPlacement.Default, tasks);
        }


        public static void AddTask(this GradleScript script, GradleTaskPlacement placement, params GradleTask[] tasks)
        {
            string semantic = GetSemantic(placement);
            GradleType gradleType = GetGradleType(placement);

            foreach (var task in tasks)
            {
                if (script.IsTaskAlreadyExists(task))
                {
                    throw new ArgumentException($"Unable to add new task with identifier '{task.Identifier}' because another one with the same identifier is already exist.");
                }

                script.AddElement(semantic, gradleType, task);
            }
        }


        private static string GetSemantic(GradleTaskPlacement placement)
        {
            string result;
            switch (placement)
            {
                case GradleTaskPlacement.Top:
                    result = TaskAtTopSemantic;
                    break;
                case GradleTaskPlacement.Bottom:
                    result = TaskAtBottomSemantic;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
            
            return result;
        }
        
        
        private static GradleType GetGradleType(GradleTaskPlacement placement)
        {
            GradleType result;
            switch (placement)
            {
                case GradleTaskPlacement.Top:
                    result = TaskAtTopGradleType;
                    break;
                case GradleTaskPlacement.Bottom:
                    result = TaskAtBottomGradleType;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
            
            return result;
        }


        private static bool IsTaskAlreadyExists(this GradleScript script, GradleTask task)
        {
            // Tasks without identifier can be added more than one time
            if (string.IsNullOrEmpty(task.Identifier))
            {
                return false;
            }

            return script
                .GetElementsEnumerable<GradleTask>(TaskAtBottomSemantic, TaskAtTopSemantic)
                .Any(p => p.Identifier == task.Identifier);
        }
    }
}
