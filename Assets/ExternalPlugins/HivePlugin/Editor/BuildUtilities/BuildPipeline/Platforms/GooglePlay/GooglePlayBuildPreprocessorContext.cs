using Modules.Hive.Editor.BuildUtilities.Android;
using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class GooglePlayBuildPreprocessorContext : AndroidBuildPreprocessorContext, IGooglePlayBuildPreprocessorContext
    {
        public GooglePlayBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            PackageSignOptions signOptions) : 
            base(preprocessorDirectives, buildOptions, buildSettings, signOptions) { }


        protected override void Disposing()
        {
            base.Disposing();

            // Apply app bundle option
            if (BuildSettings.ContainsKey(BuildSetting.AppBundleNecessity))
            {
                EditorUserBuildSettings.buildAppBundle = true;
            }
            else if (IsCiBuild)
            {
                EditorUserBuildSettings.buildAppBundle = false;
            }
        }
    }
}
