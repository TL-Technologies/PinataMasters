using System.Collections.Generic;


namespace Modules.Max.Editor
{
    public class MintegralAdapter : MaxAdapter
    {
        public override List<AndroidPackage> AndroidPackages { get; set; } = new List<AndroidPackage>()
        {
            new AndroidPackage()
            {
                Spec = "com.applovin.mediation:mintegral-adapter:16.0.11.0",
                Repository = "https://dl-maven-android.mintegral.com/repository/mbridge_android_sdk_oversea"
            }
        };
        
        
        public override List<IosPod> IosPods { get; set; } = new List<IosPod>()
        {
             new IosPod()
             {
                 Name = "AppLovinMediationMintegralAdapter",
                 Version = "7.1.0.0.0"
             }
        };
    }
}
