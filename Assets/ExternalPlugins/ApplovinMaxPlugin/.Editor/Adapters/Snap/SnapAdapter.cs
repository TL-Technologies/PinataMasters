using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class SnapAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:snap-adapter:2.3.2.0.1"
            },
            new AndroidPackage()
            {
                Spec = "androidx.constraintlayout:constraintlayout:1.1.3"
            },
            new AndroidPackage()
            {
                Spec = "com.squareup.picasso:picasso:2.71828"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinMediationSnapAdapter",
                Version = "2.0.0.0"
            }
        };
    }
}
