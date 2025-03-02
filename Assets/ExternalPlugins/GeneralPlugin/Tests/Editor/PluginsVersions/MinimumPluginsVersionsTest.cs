using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Modules.General.Editor.Tests
{
    public class MinimumPluginsVersionsTest
    {
        [SetUp]
        public void Setup()
        {
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                CustomAssert.Inconclusive("Can't verify plugins versions without internet!");
            }
        }
        
        
        [Test]
        public void GetAndroidPluginsVersions_CurrentVersions_GreaterThanMinimumRequired()
        {
            TestPluginsVersions(BuildTargetGroup.Android);
        }
        
        
        [Test]
        public void GetIosPluginsVersions_CurrentVersions_GreaterThanMinimumRequired()
        {
            TestPluginsVersions(BuildTargetGroup.iOS);
        }
        
        
        private void TestPluginsVersions(BuildTargetGroup buildTargetGroup)
        {
            // Arrange
            BuildTargetGroup currentBuildTargetGroup =
                Application.isBatchMode ?
                CommandLineUtilities.GetBuildTargetGroup() :
                BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
            Dictionary<string, string> projectPluginsVersions = PluginsVersionsUtilities.GetCurrentPluginsVersions();
            Dictionary<string, string> requiredVersions = PluginsVersionsUtilities.GetMinimumPluginsVersions(buildTargetGroup);
            
            // Act
            bool result = PluginsVersionsUtilities.IsVersionsRequirementsSatisfied(
                projectPluginsVersions, 
                requiredVersions, 
                out string comparisonReport);
            
            // Assert
            if (buildTargetGroup == currentBuildTargetGroup)
            {
                if (result)
                {
                    CustomAssert.Pass(comparisonReport);
                }
                else
                {
                    Assert.Fail(comparisonReport);
                }
            }
            else
            {
                CustomAssert.Inconclusive(comparisonReport);
            }
        }
    }
}
