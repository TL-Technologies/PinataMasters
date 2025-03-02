#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


// Documentation: https://developer.apple.com/documentation/bundleresources/information_property_list/nsapptransportsecurity
namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public class AppTransportSecurity
    {
        private const string AppTransportSecurityKey = "NSAppTransportSecurity";

        private readonly PlistElementDict root;


        public bool AllowsArbitraryLoads
        {
            get => root?.GetDict(AppTransportSecurityKey).GetBoolean("NSAllowsArbitraryLoads", false) ?? false;
            set => root.GetDict(AppTransportSecurityKey, true).SetBoolean("NSAllowsArbitraryLoads", value);
        }


        public bool AllowsArbitraryLoadsForMedia
        {
            get => root?.GetDict(AppTransportSecurityKey).GetBoolean("NSAllowsArbitraryLoadsForMedia", false) ?? false;
            set => root.GetDict(AppTransportSecurityKey, true).SetBoolean("NSAllowsArbitraryLoadsForMedia", value);
        }


        public bool AllowsArbitraryLoadsInWebContent
        {
            get => root?.GetDict(AppTransportSecurityKey).GetBoolean("NSAllowsArbitraryLoadsInWebContent", false) ?? false;
            set => root.GetDict(AppTransportSecurityKey, true).SetBoolean("NSAllowsArbitraryLoadsInWebContent", value);
        }


        public bool AllowsLocalNetworking
        {
            get => root?.GetDict(AppTransportSecurityKey).GetBoolean("NSAllowsLocalNetworking", false) ?? false;
            set => root.GetDict(AppTransportSecurityKey, true).SetBoolean("NSAllowsLocalNetworking", value);
        }


        public void AddExceptionDomain(params ExceptionDomain[] domains)
        {
            PlistElementDict dictionary = root.GetDict(AppTransportSecurityKey, true);

            foreach (var domain in domains)
            {
                dictionary.SetDict(domain.Name, domain.ToPlistDictionary());
            }
        }


        internal AppTransportSecurity(PlistElementDict root)
        {
            this.root = root;
        }
    }
}
