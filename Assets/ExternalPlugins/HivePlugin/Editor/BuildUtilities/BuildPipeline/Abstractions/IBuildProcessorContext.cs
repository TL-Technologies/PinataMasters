using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public interface IBuildProcessorContext : IDisposable
    {
        /// <summary>
        /// Gets a set of preprocessor directives applied to the project.
        /// </summary>
        IReadOnlyCollection<string> PreprocessorDirectives { get; }

        /// <summary>
        /// Gets a build options of Unity Build pipeline.
        /// </summary>
        BuildPlayerOptions BuildOptions { get; }
        
        /// <summary>
        /// Gets a system environment parameter by key.
        /// </summary>
        EnvironmentParameters Env { get; }
        
        /// <summary>
        /// Gets whether build was started manually or by CI.
        /// </summary>
        bool IsCiBuild { get; }
        
        /// <summary>
        /// Gets whether build is development or release.
        /// </summary>
        bool IsDevelopmentBuild { get; }

        /// <summary>
        /// Gets a build target. This property is similar to <see cref="BuildOptions"/>.target.
        /// </summary>
        BuildTarget BuildTarget { get; }

        /// <summary>
        /// Gets a build target. This property is similar to <see cref="BuildOptions"/>.targetGroup.
        /// </summary>
        BuildTargetGroup BuildTargetGroup { get; }
        
        /// <summary>
        /// Gets custom build settings.
        /// </summary>
        [JsonConverter(typeof(DictionarySerializationConverter<BuildSetting, string>))]
        Dictionary<BuildSetting, string> BuildSettings { get; }
    }
}
