namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleAaptOptionsExtensions
    {
        private const string GradleAaptOptionsSemantic = "HIVE_AAPT_OPTIONS";
        private const GradleType GradleConfigurationGradleType = GradleType.Main;

        public static void AddAaptOption(this GradleScript script, params GradleAaptOption[] data)
        {
            foreach (GradleAaptOption element in data)
            {
                script.AddElement(GradleAaptOptionsSemantic, GradleConfigurationGradleType, element);
            }
        }
    }
}