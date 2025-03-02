namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleConfigurationExtensions
    {
        private const string GradleConfigurationSemantic = "HIVE_CONFIGURATIONS";
        private const GradleType GradleConfigurationGradleType = GradleType.Main;
        
        
        public static void AddConfiguration(this GradleScript script, params GradleConfiguration[] data)
        {
            foreach (GradleConfiguration element in data)
            {
                script.AddElement(GradleConfigurationSemantic, GradleConfigurationGradleType, element);
            }
        }
    }
}