using System;
using System.Text;

namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleTaskDependsOn : IGradleScriptElement
    {
        public string TaskName { get; }


        public string[] Dependencies { get; }


        public GradleTaskDependsOn(string taskName, params string[] dependencies)
        {
            TaskName = taskName;
            Dependencies = dependencies;
        }


        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(TaskName) || Dependencies == null || Dependencies.Length == 0)
            {
                throw new InvalidOperationException($"Invalid format of element: {ToString()}");
            }
        }


        public override string ToString() => ToString(null);


        public string ToString(IReadOnlyGradleScript gradleScript)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(TaskName);
            sb.Append(".dependsOn ");

            sb.Append(Dependencies[0]);
            for (int i = 1; i < Dependencies.Length; i++)
            {
                sb.Append(", ");
                sb.Append(Dependencies[i]);
            }

            return sb.ToString();
        }
    }
}
