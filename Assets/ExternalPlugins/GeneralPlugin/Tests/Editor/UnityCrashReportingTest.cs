using Modules.Hive;
using Modules.Hive.Editor;
using Modules.Hive.Editor.Reflection;
using NUnit.Framework;
using UnityEditor.CrashReporting;
using UnityEngine;


namespace Modules.General.Editor.Tests
{
    public class UnityCrashReportingTest
    {
        [SetUp]
        public void Setup()
        {
            ExtendedVersion maxVersionWithoutCrashReporting = new ExtendedVersion(1, 0, 0);
            string buildVersion = Application.version;
            
            if (new ExtendedVersion(buildVersion) <= maxVersionWithoutCrashReporting)
            {
                CustomAssert.Pass($"Unity Crash Reporting integration not necessary on version {buildVersion}");
            }
        }
        
        
        [Test]
        public void GetCloudProjectId_CurrentString_MatchesFormat()
        {
            const string correctFormatRegex = "^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$";
            string cloudProjectId = PlayerSettingsHelper.GetString("cloudProjectId");
            StringAssert.IsMatch(correctFormatRegex, cloudProjectId);
        }
        
        
        [Test]
        public void GetOrganizationId_CurrentString_NotEmpty()
        {
            string organizationId = PlayerSettingsHelper.GetString("organizationId");
            Assert.False(string.IsNullOrEmpty(organizationId));
        }
        
        
        [Test]
        public void GetCrashReporting_CurrentValue_Enabled()
        {
            Assert.True(CrashReportingSettings.enabled);
        }
    }
}
