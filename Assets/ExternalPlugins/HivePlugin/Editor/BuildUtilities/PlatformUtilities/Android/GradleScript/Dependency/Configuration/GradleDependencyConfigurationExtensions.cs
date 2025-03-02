namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public static class GradleDependencyConfigurationExtensions
    {
        public static GradleDependency AddConfiguration(
            this GradleDependency dependency, 
            params IGradleDependencyConfiguration[] configurations)
        {
            dependency.AddConfiguration(configurations);
            
            return dependency;
        }
    }
}
