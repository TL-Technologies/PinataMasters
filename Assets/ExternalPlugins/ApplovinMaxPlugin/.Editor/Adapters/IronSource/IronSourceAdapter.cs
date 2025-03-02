using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class IronSourceAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:ironsource-adapter:7.2.0.0.0",
                Repository = "https://android-sdk.is.com/"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationIronSourceAdapter",
                Version = "7.2.0.0.0"
            }
        };
    }
}
