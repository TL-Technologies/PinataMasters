using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class AdColonyAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:adcolony-adapter:4.6.5.1"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationAdColonyAdapter",
                Version = "4.7.2.0.0"
            }
        };
    }
}
