using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class InMobiAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:inmobi-adapter:10.0.3.1"
            },
            new AndroidPackage()
            {
                Spec = "com.squareup.picasso:picasso:2.71828"
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:recyclerview-v7:28.+"
            },
            new AndroidPackage()
            {
                Spec = "com.android.support:customtabs:28.+"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationInMobiAdapter",
                Version = "10.0.2.0"
            }
        };
    }
}
