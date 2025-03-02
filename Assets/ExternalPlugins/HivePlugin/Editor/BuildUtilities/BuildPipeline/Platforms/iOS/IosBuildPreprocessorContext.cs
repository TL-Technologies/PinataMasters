using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal class IosBuildPreprocessorContext : BuildPreprocessorContext, IIosBuildPreprocessorContext
    {
        public IosBuildPreprocessorContext(
            IReadOnlyCollection<string> preprocessorDirectives, 
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings) : 
            base(preprocessorDirectives, buildOptions, buildSettings) { }
    }
}
