using Modules.Hive.Editor.BuildUtilities.Android;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class AmazonBuildPreprocessorContext : AndroidBuildPreprocessorContext, IAmazonBuildPreprocessorContext
    {
        public AmazonBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings,
            PackageSignOptions signOptions) :
            base(preprocessorDirectives, buildOptions, buildSettings, signOptions) { }


        protected override void Disposing()
        {
            base.Disposing();
            
            // We can't use app bundle for Amazon
            if (EditorUserBuildSettings.buildAppBundle)
            {
                Debug.LogWarning("Amazon build are not allowed to be AppBundle! Changing build type to apk.");
                EditorUserBuildSettings.buildAppBundle = false;
            }
        }
    }
}
