namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleRawDataExtensions
    {
        private const string GradleRawDataSemantic = "HIVE_RAW";
        private const GradleType GradleRawDataGradleType = GradleType.Main;


        public static void AddRawData(this GradleScript script, params GradleRawData[] data)
        {
            foreach (var element in data)
            {
                script.AddElement(GradleRawDataSemantic, GradleRawDataGradleType, element);
            }
        }
    }
}
