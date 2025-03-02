using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class WindowsBuildPreprocessorContext : BuildPreprocessorContext, IWindowsBuildPreprocessorContext
    {
        public WindowsBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings) :
            base(preprocessorDirectives, buildOptions, buildSettings) { }
    }
}
