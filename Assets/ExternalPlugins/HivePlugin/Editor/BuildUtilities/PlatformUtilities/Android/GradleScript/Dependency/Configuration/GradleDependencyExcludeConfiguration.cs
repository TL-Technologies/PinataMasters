using System.Text;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleDependencyExcludeConfiguration : IGradleDependencyConfiguration
    {
        public string Group { get; }
        public string Module { get; }
        public GradleDependencyConfigurationType ConfigurationType => GradleDependencyConfigurationType.Exclude;
        
        
        public GradleDependencyExcludeConfiguration(
            string group,
            string module)
        {
            Group = group;
            Module = module;
        }
        
        
        public override string ToString()
        {
            if (string.IsNullOrEmpty(Group) && string.IsNullOrEmpty(Module))
            {
                return null;
            }
            
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("exclude ");
            if (!string.IsNullOrEmpty(Group))
            {
                stringBuilder.Append($"group: '{Group}' ");
                if (!string.IsNullOrEmpty(Module))
                {
                    stringBuilder.Append($", module: '{Module}'");
                }
            }
            else
            {
                stringBuilder.Append($"module: '{Module}'");
            }
            
            
            return stringBuilder.ToString();
        }
    }
}
