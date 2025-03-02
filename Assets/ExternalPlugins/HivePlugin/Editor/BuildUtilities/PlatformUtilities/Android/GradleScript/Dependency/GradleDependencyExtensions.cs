using System;
using System.Linq;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleDependencyExtensions
    {
        private const string BuildScriptDependenciesSemantic = "BUILD_SCRIPT_DEPS";
        private const GradleType BuildScriptDependenciesGradleType = GradleType.BaseProject;
        private const string DependenciesSemantic = "DEPS";
        private const GradleType DependenciesGradleType = GradleType.Main;


        public static void AddDependency(this GradleScript script, params GradleDependency[] dependencies)
        {
            script.AddDependency(GradleDependencyPlacement.Default, dependencies);
        }


        public static void AddDependency(
            this GradleScript script, 
            GradleDependencyPlacement placement, 
            params GradleDependency[] dependencies)
        {
            string semantic = GetSemantic(placement);
            GradleType gradleType = GetGradleType(placement);

            foreach (var dependency in dependencies)
            {
                if (!script.IsDependencyAlreadyExists(semantic, dependency))
                {
                    script.AddElement(semantic, gradleType, dependency);
                }
            }
        }


        private static string GetSemantic(GradleDependencyPlacement placement)
        {
            string result;
            switch (placement)
            {
                case GradleDependencyPlacement.Project:
                    result = DependenciesSemantic;
                    break;
                case GradleDependencyPlacement.BuildScript:
                    result = BuildScriptDependenciesSemantic;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
            
            return result;
        }
        
        
        private static GradleType GetGradleType(GradleDependencyPlacement placement)
        {
            GradleType result;
            switch (placement)
            {
                case GradleDependencyPlacement.Project:
                    result = DependenciesGradleType;
                    break;
                case GradleDependencyPlacement.BuildScript:
                    result = BuildScriptDependenciesGradleType;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get gradle type for placement '{placement}'.");
            }
            
            return result;
        }


        private static bool IsDependencyAlreadyExists(this GradleScript script, string semantic, GradleDependency dependency)
        {
            return script
                .GetElementsEnumerable<GradleDependency>(semantic)
                .Any(p => p.IsSameDependency(dependency) && p.Version >= dependency.Version);
        }
   }
}
