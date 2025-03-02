using System;
using System.Collections.Generic;
using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleDependency : IGradleScriptElement
    {
        private static readonly ExtendedVersion DeprecatedGradleVersion = new ExtendedVersion(3, 0, 1);
        private List<IGradleDependencyConfiguration> dependencyConfigurations = new List<IGradleDependencyConfiguration>();

        public GradleDependencyType DependencyType { get; }
        public string GroupId { get; }
        public string ArtifactId { get; }
        public ExtendedVersion Version { get; protected set; }
        public string Package { get; }


        public GradleDependency(string reference, GradleDependencyType dependencyType = GradleDependencyType.Implementation)
        {
            DependencyType = dependencyType;

            string[] items = reference.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (items.Length > 0)
            {
                GroupId = items[0];
            }
            if (items.Length > 1)
            {
                ArtifactId = items[1];
            }
            if (items.Length > 2)
            {
                string[] elements = items[2].Split(new[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                if (elements.Length > 1)
                {
                    Package = elements[1];
                }

                Version = new ExtendedVersion(elements[0]);
            }
        }


        public bool IsValid => !string.IsNullOrEmpty(GroupId) && !string.IsNullOrEmpty(ArtifactId);


        public bool HasVersion => Version != null;


        public string Identifier => IsValid ? string.Format("{0}:{1}", GroupId, ArtifactId) : null;


        public void Validate()
        {
            if (!IsValid)
            {
                throw new FormatException("Wrong gradle dependency format: " + ToString());
            }
        }


        public override string ToString()
        {
            return ToString(null);
        }


        public bool IsSameDependency(GradleDependency dependency)
        {
            return DependencyType == dependency.DependencyType && Identifier == dependency.Identifier;
        }


        public string ToString(IReadOnlyGradleScript gradleScript)
        {
            string mainDependencyString = GetDependencyString();
            
            StringBuilder sb = new StringBuilder();
            sb.Append(GetOperator());
            if (dependencyConfigurations.Count > 0)
            {
                sb.Append($" ({mainDependencyString}) {{\n");
                foreach (IGradleDependencyConfiguration configuration in dependencyConfigurations)
                {
                    sb.Append($"{configuration}\n");
                }
                sb.Append("}");
            }
            else
            {
                sb.Append($" {mainDependencyString}");
            }
            
            return sb.ToString();
            

            string GetOperator()
            {
                switch (DependencyType)
                {
                    case GradleDependencyType.ClassPath:
                        return "classpath";
                    case GradleDependencyType.Implementation:
                        return (gradleScript != null && gradleScript.GradlePackageVersion <= DeprecatedGradleVersion) ? "compile" : "implementation";
                    default:
                        throw new NotImplementedException($"Unable to resolve gradle operation for DependencyType = {DependencyType}");
                }
            }
            
            
            string GetDependencyString()
            {
                StringBuilder builder = new StringBuilder();
                builder.Append($"'{GroupId}:{ArtifactId}");
                if (Version != null)
                {
                    builder.Append($":{Version}");
                }
                if (!string.IsNullOrEmpty(Package))
                {
                    builder.Append($"@{Package}");
                }
                builder.Append("'");
                
                return builder.ToString();
            }
        }
        
        
        internal void AddConfiguration(params IGradleDependencyConfiguration[] configurations)
        {
            if (configurations != null)
            {
                dependencyConfigurations.AddRange(configurations);
            }
        }
    }
}
