using System;
using System.Linq;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleApplyPluginExtensions
    {
        private const string ApplyPluginAtTopSemantic = "APPLY_PLUGINS";
        private const GradleType ApplyPluginAtTopGradleType = GradleType.Main;
        private const string ApplyPluginAtBottomSemantic = "HIVE_APPLY_PLUGINS_BOTTOM";
        private const GradleType ApplyPluginAtBottomGradleType = GradleType.Main;


        public static void AddApplyPlugin(this GradleScript script, params GradleApplyPlugin[] plugins)
        {
            script.AddApplyPlugin(GradleApplyPluginPlacement.Default, plugins);
        }


        public static void AddApplyPlugin(
            this GradleScript script, 
            GradleApplyPluginPlacement placement, 
            params GradleApplyPlugin[] plugins)
        {
            string semantic = GetSemantic(placement);
            GradleType gradleType = GetGradleType(placement);

            foreach (var plugin in plugins)
            {
                if (!script.IsPluginAlreadyExists(plugin))
                {
                    script.AddElement(semantic, gradleType, plugin);
                }
            }
        }
        
        
        private static string GetSemantic(GradleApplyPluginPlacement placement)
        {
            string result;
            switch (placement)
            {
                case GradleApplyPluginPlacement.Top:
                    result = ApplyPluginAtTopSemantic;
                    break;
                case GradleApplyPluginPlacement.Bottom:
                    result = ApplyPluginAtBottomSemantic;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
            
            return result;
        }


        private static GradleType GetGradleType(GradleApplyPluginPlacement placement)
        {
            GradleType result;
            switch (placement)
            {
                case GradleApplyPluginPlacement.Top:
                    result = ApplyPluginAtTopGradleType;
                    break;
                case GradleApplyPluginPlacement.Bottom:
                    result = ApplyPluginAtBottomGradleType;
                    break;
                default:
                    throw new NotImplementedException($"Unable to get semantic for placement '{placement}'.");
            }
            
            return result;
        }


        private static bool IsPluginAlreadyExists(this GradleScript script, GradleApplyPlugin plugin)
        {
            return script
                .GetElementsEnumerable<GradleApplyPlugin>(ApplyPluginAtTopSemantic, ApplyPluginAtBottomSemantic)
                .Any(p => p.Package == plugin.Package);
        }
    }
}
