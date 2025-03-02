using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class UnityAdsAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:unityads-adapter:4.0.0.2"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationUnityAdsAdapter",
                Version = "4.0.0.1"
            }
        };
    }
}
