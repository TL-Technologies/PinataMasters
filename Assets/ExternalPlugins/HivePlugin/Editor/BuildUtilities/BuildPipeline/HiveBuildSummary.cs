using Newtonsoft.Json;
using System;
using UnityEditor;
using UnityEditor.Build.Reporting;


namespace Modules.Hive.Editor.BuildUtilities
{
    // This class duplicates UnityEditor.Build.Reporting.BuildSummary for serialization purposes
    // (added setter for properties)
    public class HiveBuildSummary
    {
        /// <summary>
        ///   <para>The time the build was started.</para>
        /// </summary>
        [JsonProperty] public DateTime buildStartedAt { get; private set; }
    
        /// <summary>
        ///   <para>The Application.buildGUID of the build.</para>
        /// </summary>
        [JsonProperty] public GUID guid { get; private set; }
    
        /// <summary>
        ///   <para>The platform that the build was created for.</para>
        /// </summary>
        [JsonProperty] public BuildTarget platform { get; private set; }
    
        /// <summary>
        ///   <para>The platform group the build was created for.</para>
        /// </summary>
        [JsonProperty] public BuildTargetGroup platformGroup { get; private set; }
    
        /// <summary>
        ///   <para>The BuildOptions used for the build, as passed to BuildPipeline.BuildPlayer.</para>
        /// </summary>
        [JsonProperty] public BuildOptions options { get; private set; }
    
        /// <summary>
        ///   <para>The output path for the build, as provided to BuildPipeline.BuildPlayer.</para>
        /// </summary>
        [JsonProperty] public string outputPath { get; private set; }

        /// <summary>
        ///   <para>The total size of the build output, in bytes.</para>
        /// </summary>
        [JsonProperty] public ulong totalSize { get; private set; }
    
        /// <summary>
        ///   <para>The total time taken by the build process.</para>
        /// </summary>
        [JsonProperty] public TimeSpan totalTime { get; private set; }
    
        /// <summary>
        ///   <para>The time the build ended.</para>
        /// </summary>
        [JsonProperty] public DateTime buildEndedAt { get; private set; }
    
        /// <summary>
        ///   <para>The total number of errors and exceptions recorded during the build process.</para>
        /// </summary>
        [JsonProperty] public int totalErrors { get; private set; }
    
        /// <summary>
        ///   <para>The total number of warnings recorded during the build process.</para>
        /// </summary>
        [JsonProperty] public int totalWarnings { get; private set; }
    
        /// <summary>
        ///   <para>The outcome of the build.</para>
        /// </summary>
        [JsonProperty] public BuildResult result { get; private set; }
        
        
        public static explicit operator HiveBuildSummary(BuildSummary summary)
        {
            return new HiveBuildSummary
            {
                buildStartedAt = summary.buildStartedAt,
                buildEndedAt = summary.buildEndedAt,
                guid = summary.guid,
                options = summary.options,
                outputPath = summary.outputPath,
                platform = summary.platform,
                platformGroup = summary.platformGroup,
                result = summary.result,
                totalErrors = summary.totalErrors,
                totalSize = summary.totalSize,
                totalTime = summary.totalTime,
                totalWarnings = summary.totalWarnings
            };
        }
    }
}