using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    internal abstract class BuildProcessorContext : IBuildProcessorContext
    {
        /// <inheritdoc/>
        public IReadOnlyCollection<string> PreprocessorDirectives { get; }


        /// <inheritdoc/>
        public BuildPlayerOptions BuildOptions { get; }

        public EnvironmentParameters Env { get; }


        /// <inheritdoc/>
        public bool IsCiBuild => BuildSettings.ContainsKey(BuildSetting.CiBuildFlag);

        
        /// <inheritdoc/>
        public bool IsDevelopmentBuild => BuildOptions.options.HasFlag(UnityEditor.BuildOptions.Development);


        /// <inheritdoc/>
        public Dictionary<BuildSetting, string> BuildSettings { get; }
        
        
        /// <inheritdoc/>
        public BuildTarget BuildTarget => BuildOptions.target;


        /// <inheritdoc/>
        public BuildTargetGroup BuildTargetGroup => BuildOptions.targetGroup;


        /// <summary>
        /// Gets whether the object is disposed
        /// </summary>
        public bool IsDisposed { get; private set; }


        public BuildProcessorContext(
            IReadOnlyCollection<string> preprocessorDirectives,
            BuildPlayerOptions buildOptions,
            Dictionary<BuildSetting, string> buildSettings)
        {
            PreprocessorDirectives = preprocessorDirectives;
            BuildOptions = buildOptions;
            BuildSettings = buildSettings;
        }


        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Disposing();
            IsDisposed = true;
        }


        protected virtual void Disposing() { }
    }
}
