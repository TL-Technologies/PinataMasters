using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class MacOsBuildPreprocessorContext : BuildPreprocessorContext, IMacOsBuildPreprocessorContext
    {
        public MacOsBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings) :
            base(preprocessorDirectives, buildOptions, buildSettings) { }
    }
}
