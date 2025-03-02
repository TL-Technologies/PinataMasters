using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class FacebookAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            // Ensure that Resolver doesn't inadvertently pull Facebook's beta versions of the SDK by forcing a specific version.
            // Since FAN SDK depends on older versions of a few support and play serives versions
            // `com.applovin.mediation:facebook-adapter:x.y.z.a` resolves to `com.applovin.mediation:facebook-adapter:+` which pulls down the beta versions of FAN SDK.
            // Note that forcing the adapter is enough to stop Jar Resolver from pulling the latest FAN SDK.
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:facebook-adapter:[6.8.0.12]"
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:recyclerview-v7:28.+"
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:appcompat-v7:28.+"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationFacebookAdapter",
                Version = "6.9.0.7"
            }
        };
    }
}
