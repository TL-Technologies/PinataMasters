using System;
using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleCopyTask : GradleTask
    {
        public string From { get; }
        public string Include { get; }
        public string Into { get; }


        public GradleCopyTask(string from, string include, string into) : base(null)
        {
            if (string.IsNullOrWhiteSpace(from))
            {
                throw new ArgumentNullException(nameof(@from));
            }
            if (string.IsNullOrWhiteSpace(include))
            {
                throw new ArgumentNullException(nameof(include));
            }
            if (string.IsNullOrWhiteSpace(into))
            {
                throw new ArgumentNullException(nameof(@into));
            }

            From = from;
            Include = include;
            Into = into;
        }


        public override void Validate() { }


        public override string ToString(IReadOnlyGradleScript gradleScript)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("copy {");
            sb.AppendLine($"    from '{From}'");
            sb.AppendLine($"    include '{Include}'");
            sb.AppendLine($"    into '{Into}'");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
