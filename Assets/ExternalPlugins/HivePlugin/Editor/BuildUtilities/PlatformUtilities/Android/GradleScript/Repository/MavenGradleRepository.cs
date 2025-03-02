using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class MavenGradleRepository : GradleRepository
    {
        public MavenGradleRepository(string url) : base(GetReferenceFromUrl(url)) { }
        

        private static string GetReferenceFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("maven { url \"");
            stringBuilder.Append(url);
            stringBuilder.Append("\" }");

            return stringBuilder.ToString();
        }
    }
}
