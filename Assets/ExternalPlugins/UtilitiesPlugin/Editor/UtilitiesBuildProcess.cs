#if HIVE
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Ios;


namespace Modules.Utilities.Editor
{
    public class UtilitiesBuildProcess : IBuildPostprocessor<IIosBuildPostprocessorContext>
    {
        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            context.PbxProject.AddSystemFramework(Framework.WebKit);
            
            // Configure app transport security (global rule for iOS 9)
            context.InfoPlist.AppTransportSecurity.AllowsArbitraryLoads = true;

            // Configure app transport security (by domain rules for iOS 10 and later)
            context.InfoPlist.AppTransportSecurity.AddExceptionDomain(
                new ExceptionDomain("api.playgendary.com")
                {
                    AllowsInsecureHttpLoads = true,
                    RequiresForwardSecrecy = false,
                    IncludesSubdomains = true
                },
                new ExceptionDomain("s3.eu-central-1.amazonaws.com")
                {
                    AllowsInsecureHttpLoads = true,
                    RequiresForwardSecrecy = false,
                    IncludesSubdomains = true
                });
        }
    }
}
#endif
