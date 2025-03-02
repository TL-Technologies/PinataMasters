using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Android;
using System.Xml;
using UnityEditor;


namespace Modules.General.Editor.BuildProcess
{
    internal class AndroidBuildProcess : IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            // Add necessary dependency
            context.GradleScript.AddDependency(new GradleDependency("org.jsoup:jsoup:1.8.3"));
            
            // Set custom class for main activity
            XmlElement mainActivityElement = context.AndroidManifest.MainActivityElement;
            mainActivityElement.SetAttribute("name", context.AndroidManifest.AndroidNamespace, "com.lllibset.LLActivity.LLActivity");

            if (!string.IsNullOrEmpty(TargetSettings.ApplicationIdentifier))
            {
                context.AndroidManifest.AddPackageElement(TargetSettings.ApplicationIdentifier);
            }

            // Change network security config
            if (context.BuildOptions.options.HasFlag(BuildOptions.Development))
            {
                context.NetworkSecurityConfig.AddTrustedCertificates(NetworkSecurityConfigSection.DebugOverrides, "user");
            }
        }
    }
}
