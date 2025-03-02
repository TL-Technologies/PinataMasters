using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GoogleMobileServicesDependency : GradleDependency
    {
        public static readonly string FallbackVersionKey = string.Empty;

        // https://developers.google.com/android/guides/setup
        public static readonly IReadOnlyDictionary<string, ExtendedVersion> DefaultPackageVersions = new Dictionary<string, ExtendedVersion>
        {
            { FallbackVersionKey, new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-ads", new ExtendedVersion("20.3.0") },
            { "com.google.android.gms:play-services-analytics", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-auth", new ExtendedVersion("18.0.0") },
            { "com.google.android.gms:play-services-awareness", new ExtendedVersion("18.0.0") },
            { "com.google.android.gms:play-services-base", new ExtendedVersion("17.5.0") },
            { "com.google.android.gms:play-services-cast", new ExtendedVersion("18.1.0") },
            { "com.google.android.gms:play-services-drive", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-fitness", new ExtendedVersion("18.0.0") },
            { "com.google.android.gms:play-services-games", new ExtendedVersion("19.0.0") },
            { "com.google.android.gms:play-services-gcm", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-identity", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-location", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-nearby", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-panorama", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-plus", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-safetynet", new ExtendedVersion("17.0.0") },
            { "com.google.android.gms:play-services-vision", new ExtendedVersion("20.0.0") },
            { "com.google.android.gms:play-services-wallet", new ExtendedVersion("18.0.0") },
            { "com.google.android.gms:play-services-wearable", new ExtendedVersion("17.0.0") },
        };


        public GoogleMobileServicesDependency(string libraryName) : base("com.google.android.gms:" + libraryName)
        {
            if (!HasVersion)
            {
                Version = DefaultPackageVersions.TryGetValue(Identifier, out ExtendedVersion version) ? 
                    version : 
                    DefaultPackageVersions[FallbackVersionKey];
            }
        }

        
        public GoogleMobileServicesDependency(string libraryName, string version) : this($"{libraryName}:{version}") { }
    }
}
