namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleTaskDependsOnExtensions
    {
        private const string GradleTaskDependsOnSemantic = "HIVE_TASKS_DEPENDS_ON";
        private const GradleType GradleTaskDependsOnGradleType = GradleType.Main;


        public static void AddTaskDependsOn(this GradleScript script, params GradleTaskDependsOn[] depends)
        {
            foreach (var element in depends)
            {
                script.AddElement(GradleTaskDependsOnSemantic, GradleTaskDependsOnGradleType, element);
            }
        }
    }
}
