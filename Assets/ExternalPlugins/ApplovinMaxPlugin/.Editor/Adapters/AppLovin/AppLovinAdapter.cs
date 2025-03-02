using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class AppLovinAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin:applovin-sdk:11.4.6"
            },
            new AndroidPackage()
            {
                Spec = "com.google.android.exoplayer:exoplayer:2.13.3"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
            new IosPod()
            {
                Name = "AppLovinSDK",
                Version = "11.2.0-beta2",
                Source = "https://github.com/AppLovin/Cocoapods-Specs.git"
            }
        };
    }
}
