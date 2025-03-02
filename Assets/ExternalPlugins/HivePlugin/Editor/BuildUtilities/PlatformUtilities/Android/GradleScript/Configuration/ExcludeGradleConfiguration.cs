using System;
using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class ExcludeGradleConfiguration : GradleConfiguration
    {
        public ExcludeGradleConfiguration(
            string group, 
            string module, 
            ExcludeGradleConfigurationTarget target = ExcludeGradleConfigurationTarget.Default) : 
            base(GetConfigurationForExclude(group, module, target)) { }
        
        
        private static string GetConfigurationForExclude(
            string group, 
            string module, 
            ExcludeGradleConfigurationTarget target = ExcludeGradleConfigurationTarget.Default)
        {
            if (string.IsNullOrEmpty(group) && string.IsNullOrEmpty(module))
            {
                return null;
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(GetTarget(target));
            stringBuilder.Append(".exclude ");
            if (!string.IsNullOrEmpty(group))
            {
                stringBuilder.Append($"group: '{group}' ");
            }
            if (!string.IsNullOrEmpty(module))
            {
                stringBuilder.Append($"module: '{module}'");
            }
            
            return stringBuilder.ToString();
        }
        
        
        private static string GetTarget(ExcludeGradleConfigurationTarget target)
        {
            switch (target)
            {
                case ExcludeGradleConfigurationTarget.All: 
                    return "all*";
                case ExcludeGradleConfigurationTarget.Compile:
                    return "compile";
                case ExcludeGradleConfigurationTarget.Runtime:
                    return "runtime";
                default:
                    throw new NotImplementedException($"Unable to get target for '{target}'.");
            }
        }
    }
}