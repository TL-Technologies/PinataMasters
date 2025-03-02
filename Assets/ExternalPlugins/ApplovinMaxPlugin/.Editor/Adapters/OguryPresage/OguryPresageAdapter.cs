using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class OguryPresageAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:ogury-presage-adapter:5.0.10.1",
                Repository = "https://maven.ogury.co"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationOguryPresageAdapter",
                Version = "2.5.1.0"
            }
        };
    }
}
