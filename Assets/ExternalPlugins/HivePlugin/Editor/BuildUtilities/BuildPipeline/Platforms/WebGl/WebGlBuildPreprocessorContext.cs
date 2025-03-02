using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class WebGlBuildPreprocessorContext : BuildPreprocessorContext, IWebGlBuildPreprocessorContext
    {
        public WebGlBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings) :
            base(preprocessorDirectives, buildOptions, buildSettings) { }
    }
}
