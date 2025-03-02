using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class GoogleAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            // Ensure that Resolver doesn't inadvertently pull the latest Play Services Ads' SDK that we haven't certified against.
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:google-adapter:[20.6.0.1]"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationGoogleAdapter",
                Version = "8.13.0.7"
            }
        };
    }
}
