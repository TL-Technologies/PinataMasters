using System;
using System.Text;

namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleApplyPlugin : IGradleScriptElement
    {
        public string Package { get; }
        public bool IsValid => !string.IsNullOrEmpty(Package);


        public GradleApplyPlugin(string package)
        {
            Package = package;
        }

        
        public void Validate()
        {
            if (!IsValid)
            {
                throw new FormatException("Wrong plugin format: " + ToString());
            }
        }


        public override string ToString()
        {
            return ToString(null);
        }


        public string ToString(IReadOnlyGradleScript gradleScript)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("apply plugin: '");
            sb.Append(Package);
            sb.Append("'");

            return sb.ToString();
        }
    }
}
