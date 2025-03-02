using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    // Artifacts mapping
    // https://developer.android.com/jetpack/androidx/migrate/artifact-mappings
    public class AndroidXDependency : GradleDependency
    {
        public static readonly string FallbackVersionKey = string.Empty;

        // (18.09.2019) https://developer.android.com/jetpack/androidx/versions/
        public static readonly IReadOnlyDictionary<string, ExtendedVersion> DefaultPackageVersions = new Dictionary<string, ExtendedVersion>
        {
            { FallbackVersionKey, new ExtendedVersion("1.0.0") },
            { "androidx.annotation", new ExtendedVersion("1.1.0") },
            { "androidx.appcompat", new ExtendedVersion("1.1.0") },
            { "androidx.arch", new ExtendedVersion("2.1.0") },
            { "androidx.browser", new ExtendedVersion("1.0.0") },
            { "androidx.collection", new ExtendedVersion("1.1.0") },
            { "androidx.constraintlayout", new ExtendedVersion("1.1.3") },
            { "androidx.core", new ExtendedVersion("1.1.0") },
            { "androidx.databinding", new ExtendedVersion("3.5.0") },
            { "androidx.documentfile", new ExtendedVersion("1.0.1") },
            { "androidx.fragment", new ExtendedVersion("1.1.0") },
            { "androidx.legacy", new ExtendedVersion("1.0.0") },
            { "androidx.lifecycle", new ExtendedVersion("2.1.0") },
            { "androidx.media", new ExtendedVersion("1.1.0") },
            { "androidx.mediarouter", new ExtendedVersion("1.1.0") },
            { "androidx.multidex", new ExtendedVersion("2.0.1") },
            { "androidx.navigation", new ExtendedVersion("2.1.0") },
            { "androidx.paging", new ExtendedVersion("2.1.0") },
            { "androidx.preference", new ExtendedVersion("1.1.0") },
            { "androidx.recyclerview", new ExtendedVersion("1.1.0") },
            { "androidx.room", new ExtendedVersion("2.1.0") },
            { "androidx.sqlite", new ExtendedVersion("2.0.1") },
            { "androidx.test", new ExtendedVersion("1.2.0") },
            { "androidx.transition", new ExtendedVersion("1.1.0") },
            { "androidx.vectordrawable", new ExtendedVersion("1.1.0") },
            { "androidx.versionedparcelable", new ExtendedVersion("1.1.0") },
            { "androidx.work", new ExtendedVersion("2.2.0") },
        };


        public AndroidXDependency(string reference) : base(reference)
        {
            if (!HasVersion)
            {
                Version = DefaultPackageVersions.TryGetValue(GroupId, out ExtendedVersion version) ? 
                    version : 
                    DefaultPackageVersions[FallbackVersionKey];
            }
        }
    }
}
