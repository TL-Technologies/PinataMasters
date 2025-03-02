using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class FyberAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:fyber-adapter:8.1.2.2"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationFyberAdapter",
                Version = "8.1.3.1"
            }
        };
    }
}
