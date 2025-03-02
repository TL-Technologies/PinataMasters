using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Android;
using Modules.Hive.Editor.BuildUtilities.Ios;
using System.Xml;


namespace Modules.AppsFlyer.Editor.BuildProcess
{
    internal class AppsFlyerBuildProcess :
        IBuildPreprocessor<IAndroidBuildPreprocessorContext>,
        IBuildPreprocessor<IAmazonBuildPreprocessorContext>,
        IBuildPostprocessor<IIosBuildPostprocessorContext>
    {
        private const string XcodeSdkFolder = "AppsFlyer";
        private const string AppsFlyerLibFramework = "AppsFlyerLib.framework.zip";
        private const string AmazonStoreChannelName = "Amazon";
        private const string SkAdAttributionKey = "NSAdvertisingAttributionReportEndpoint";
        private const string SkAdReportEndpoint = "https://www.appsflyer-skadnetwork.com";
        
        
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            context.GradleScript.AddRepository(GradleRepository.MavenCentral);
            
            context.GradleScript.AddDependency(
                new GradleDependency("com.appsflyer:af-android-sdk:6.1.4"),
                new GradleDependency("com.android.installreferrer:installreferrer:2.2"),
                new AndroidXDependency("androidx.appcompat:appcompat"));

            context.AndroidManifest.AddPermissionElement(Permission.Internet);
            context.AndroidManifest.AddPermissionElement(Permission.AccessNetworkState);
            context.AndroidManifest.AddPermissionElement(Permission.AccessWifiState);
            
            // Optional parameter
            // context.AndroidManifest.AddPermissionElement(Permission.ReadPhoneState);
            
            // <receiver android:name="com.appsflyer.MultipleInstallBroadcastReceiver" android:exported="true">
            //     <intent-filter>
            //         <action android:name="com.android.vending.INSTALL_REFERRER" />
            //     </intent-filter>
            // </receiver>
            string androidNamespace = context.AndroidManifest.AndroidNamespace;
            XmlElement element = context.AndroidManifest.AddReceiverElement(
                "com.appsflyer.MultipleInstallBroadcastReceiver");
            element.SetAttribute("exported", androidNamespace, true.ToString());
            XmlElement intentFilterElement = context.AndroidManifest.Xml.CreateElement("intent-filter");
            XmlElement actionElement = context.AndroidManifest.Xml.CreateElement("action");
            actionElement.SetAttribute(
                "name",
                androidNamespace, 
                "com.android.vending.INSTALL_REFERRER");
            actionElement.IsEmpty = true;
            intentFilterElement.AppendChild(actionElement);
            element.AppendChild(intentFilterElement);
            
            context.BackupRules.AddBackupRule(
                AndroidBackupRuleType.Exclude,
                AndroidBackupDomain.SharedPref,
                "appsflyer-data");
        }
        
        
        public void OnPreprocessBuild(IAmazonBuildPreprocessorContext context)
        {
            context.AndroidManifest.AddApplicationMetaDataElement(
                "CHANNEL",
                AmazonStoreChannelName);
        }


        public void OnPostprocessBuild(IIosBuildPostprocessorContext context)
        {
            string xcodePath = context.GetDestinationPath(XcodeSdkFolder);
            string platformRoot = AppsFlyerPluginHierarchy.Instance.GetPlatformPath(PlatformAlias.Ios);

            FileSystemUtilities.ExtractArchive(
                UnityPath.Combine(platformRoot, AppsFlyerLibFramework),
                xcodePath);

            context.PbxProject.AddItemsRecursively(xcodePath);
            
            // Add references to system frameworks
            context.PbxProject.AddSystemFramework(Framework.AdSupport);
            context.PbxProject.AddSystemFramework(Framework.IAd);
            context.PbxProject.AddSystemFramework(Framework.AdServices, true);
            
            context.InfoPlist.SetString(SkAdAttributionKey, SkAdReportEndpoint);
        }
    }
}
