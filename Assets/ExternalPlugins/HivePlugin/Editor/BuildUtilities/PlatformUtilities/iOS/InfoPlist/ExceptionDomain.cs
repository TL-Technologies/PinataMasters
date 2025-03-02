using System;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


// Documentation: https://developer.apple.com/documentation/bundleresources/information_property_list/nsapptransportsecurity/nsexceptiondomains
namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public class ExceptionDomain
    {
        /// <summary>
        /// Gets the domain name.
        /// </summary>
        public string Name { get; }

        public bool? IncludesSubdomains { get; set; }

        public bool? AllowsInsecureHttpLoads { get; set; }

        public bool? RequiresForwardSecrecy { get; set; }

        public bool? RequiresCertificateTransparency { get; set; }

        public string MinimumTlsVersion { get; set; }


        public ExceptionDomain(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The name argument should not be null or empty.");
            }

            Name = name;
        }


        internal PlistElementDict ToPlistDictionary()
        {
            var data = new PlistElementDict();

            if (IncludesSubdomains.HasValue)
            {
                data.SetBoolean("NSIncludesSubdomains", IncludesSubdomains.Value);
            }

            if (AllowsInsecureHttpLoads.HasValue)
            {
                data.SetBoolean("NSExceptionAllowsInsecureHTTPLoads", AllowsInsecureHttpLoads.Value);
            }

            if (RequiresForwardSecrecy.HasValue)
            {
                data.SetBoolean("NSExceptionRequiresForwardSecrecy", RequiresForwardSecrecy.Value);
            }

            if (RequiresCertificateTransparency.HasValue)
            {
                data.SetBoolean("NSRequiresCertificateTransparency", RequiresCertificateTransparency.Value);
            }

            if (!string.IsNullOrEmpty(MinimumTlsVersion))
            {
                data.SetString("NSExceptionMinimumTLSVersion", MinimumTlsVersion);
            }

            return data;
        }
    }
}
