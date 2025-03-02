using System;
using System.Linq;

namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleRepositoryExtensions
    {
        private const string AllProjectsRepositorySemantic = "HIVE_ALL_PROJECTS_REPOSITORIES";
        private const GradleType AllProjectsRepositoryGradleType = GradleType.BaseProject;
        private const string BuildScriptRepositorySemantic = "HIVE_BUILDSCRIPT_REPOSITORIES";
        private const GradleType BuildScriptRepositoryGradleType = GradleType.BaseProject;
        


        public static void AddRepository(this GradleScript script, params GradleRepository[] repositories)
        {
            script.AddRepository(GradleRepositoryPlacement.Default, repositories);
        }


        public static void AddRepository(
            this GradleScript script, 
            GradleRepositoryPlacement placement, 
            params GradleRepository[] repositories)
        {
            string semantic = GetSemantic(placement);
            GradleType gradleType = GetGradleType(placement);

            foreach (var repository in repositories)
            {
                if (!script.IsRepositoryAlreadyExists(semantic, repository))
                {
                    script.AddElement(semantic, gradleType, repository);
                }
            }
        }


        private static string GetSemantic(GradleRepositoryPlacement placement)
        {
            switch (placement)
            {
                case GradleRepositoryPlacement.AllProjects: return AllProjectsRepositorySemantic;
                case GradleRepositoryPlacement.BuildScript: return BuildScriptRepositorySemantic;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
        }
        
        
        private static GradleType GetGradleType(GradleRepositoryPlacement placement)
        {
            GradleType result;
            switch (placement)
            {
                case GradleRepositoryPlacement.AllProjects:
                    result = AllProjectsRepositoryGradleType;
                    break;
                case GradleRepositoryPlacement.BuildScript:
                    result = BuildScriptRepositoryGradleType;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get gradle type for placement '{placement}'.");
            }
            
            return result;
        }


        private static bool IsRepositoryAlreadyExists(this GradleScript script, string semantic, GradleRepository repository)
        {
            return script
                .GetElementsEnumerable<GradleRepository>(semantic)
                .Any(p => p.Reference == repository.Reference);
        }
    }
}
