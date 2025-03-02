using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class TapjoyAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:tapjoy-adapter:12.9.0.3",
                Repository = "https://sdk.tapjoy.com/"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationTapjoyAdapter",
                Version = "12.9.0.1"
            }
        };
    }
}
