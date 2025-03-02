using System;
using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleNamedTask : GradleTask
    {
        public string Name { get; }
        public string Code { get; }
        public string Type { get; }


        public GradleNamedTask(string name, string code, string type = null) : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Code = code;
            Type = type;
        }


        public override void Validate() { }


        public override string ToString() => ToString(null);


        public override string ToString(IReadOnlyGradleScript gradleScript)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("task ");
            sb.Append(Name);

            if (!string.IsNullOrEmpty(Type))
            {
                sb.Append("(type: ");
                sb.Append(Type);
                sb.Append(")");
            }

            sb.Append("{\n");
            sb.Append(Code);
            sb.Append("\n}");

            return sb.ToString();
        }
    }
}
