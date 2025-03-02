using System.Collections.Generic;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
using UnityEditor.iOS.Xcode.Extensions;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public static class PbxProjectExtensions
    {
        #region Was taken from PBXProjectExtension.cs

        // Create a wrapper class so that collection initializers work and we can have a 
        // compact notation. Note that we can't use Dictionary because the keys may be duplicate
        internal class FlagList : List<KeyValuePair<string, string>>
        {
            public void Add(string flag, string value)
            {
                Add(new KeyValuePair<string, string>(flag, value));
            }
        }

        // Taken from PBXProjectExtension.cs
        private static void SetBuildFlagsFromDict(this PBXProject proj, string configGuid, IEnumerable<KeyValuePair<string, string>> data)
        {
            foreach (var kv in data)
            {
                proj.AddBuildPropertyForConfig(configGuid, kv.Key, kv.Value);
            }
        }

        #endregion



        #region Sticker Pack

        //  047655FB1F2890A6002D21AE /* Debug */ = {
        //        isa = XCBuildConfiguration;
        //        buildSettings = {
        //          . . .
        //        };
        //        name = Debug;
        //  };
        //  047655FC1F2890A6002D21AE /* Release */ = {
        //        isa = XCBuildConfiguration;
        //        buildSettings = {
        //          . . .
        //        };
        //        name = Release;
        //  };         

        internal static readonly FlagList StickerPackDebugBuildSettings = new FlagList
        {
            //{ "ASSETCATALOG_COMPILER_APPICON_NAME", "iMessage App Icon" },
            //{ "DEVELOPMENT_TEAM",  },
            //{ "INFOPLIST_FILE", "<path/to/Info.plist>" },
            //{ "PRODUCT_BUNDLE_IDENTIFIER", "<bundle id>" },
            //{ "IPHONEOS_DEPLOYMENT_TARGET", "<version>" },
            //{ "TARGETED_DEVICE_FAMILY", "<family>" },
            { "PRODUCT_NAME", "$(TARGET_NAME)" },
            { "SKIP_INSTALL", "YES" },
        };

        internal static readonly FlagList StickerPackReleaseBuildSettings = new FlagList
        {
            //{ "ASSETCATALOG_COMPILER_APPICON_NAME", "iMessage App Icon" },
            //{ "DEVELOPMENT_TEAM",  },
            //{ "INFOPLIST_FILE", "<path/to/Info.plist>" },
            //{ "PRODUCT_BUNDLE_IDENTIFIER", "<bundle id>" },
            //{ "IPHONEOS_DEPLOYMENT_TARGET", "<version>" },
            //{ "TARGETED_DEVICE_FAMILY", "<family>" },
            { "PRODUCT_NAME", "$(TARGET_NAME)" },
            { "SKIP_INSTALL", "YES" },
        };

        /// <summary>
        /// Creates a sticker pack extension.
        /// </summary>
        /// <returns>The GUID of the new target.</returns>
        /// <param name="proj">A project passed as this argument.</param>
        /// <param name="mainTargetGuid">The GUID of the main target to link the sticker pack extension to.</param>
        /// <param name="targetName">The name of the sticker pack extension target.</param>
        /// <param name="bundleId">The bundle ID of the sticker pack extension. The bundle ID must be
        /// prefixed with the parent sticker pack app bundle ID.</param>
        /// <param name="infoPlistPath">Path to the sticker pack extension Info.plist document.</param>
        /// <param name="iosDeploymentTarget">iPhone OS Deployment Target (10.0 by default).</param>
        /// <param name="developmentTeamId">Apple Developer Team identifier</param>
        /// <param name="assetCatalogAppIconName">The name of xcode assets folder with application icons.</param>
        /// <param name="targetDevice">Target device family</param>
        public static string AddStickerPackExtension(
            this PBXProject proj,
            string mainTargetGuid,
            string targetName,
            string bundleId,
            string infoPlistPath,
            string iosDeploymentTarget = null,
            string developmentTeamId = null,
            string assetCatalogAppIconName = "iMessage App Icon",
            iOSTargetDevice targetDevice = iOSTargetDevice.iPhoneAndiPad)
        {
            var newTargetGuid = proj.AddTarget(targetName, ".appex", "com.apple.product-type.app-extension.messages-sticker-pack");

            foreach (var configName in proj.BuildConfigNames())
            {
                var configGuid = proj.BuildConfigByName(newTargetGuid, configName);
                if (configName.Contains("Debug"))
                {
                    SetBuildFlagsFromDict(proj, configGuid, StickerPackDebugBuildSettings);
                }
                else
                {
                    SetBuildFlagsFromDict(proj, configGuid, StickerPackReleaseBuildSettings);
                }

                proj.SetBuildPropertyForConfig(configGuid, "PRODUCT_BUNDLE_IDENTIFIER", bundleId);
                proj.SetBuildPropertyForConfig(configGuid, "INFOPLIST_FILE", infoPlistPath);
                proj.SetBuildPropertyForConfig(configGuid, "ASSETCATALOG_COMPILER_APPICON_NAME", assetCatalogAppIconName);
                if (!string.IsNullOrEmpty(iosDeploymentTarget))
                {
                    proj.SetBuildPropertyForConfig(configGuid, "IPHONEOS_DEPLOYMENT_TARGET", iosDeploymentTarget);
                }

                if (!string.IsNullOrEmpty(developmentTeamId))
                    proj.SetBuildPropertyForConfig(configGuid, "DEVELOPMENT_TEAM", developmentTeamId);

                switch (targetDevice)
                {
                    case iOSTargetDevice.iPhoneOnly:
                    proj.SetBuildPropertyForConfig(configGuid, "TARGETED_DEVICE_FAMILY", "1");
                        break;
                    case iOSTargetDevice.iPadOnly:
                        proj.SetBuildPropertyForConfig(configGuid, "TARGETED_DEVICE_FAMILY", "2");
                        break;
                    case iOSTargetDevice.iPhoneAndiPad:
                        proj.SetBuildPropertyForConfig(configGuid, "TARGETED_DEVICE_FAMILY", "1,2");
                        break;
                    default:
                        break;
                }
            }

            proj.AddResourcesBuildPhase(newTargetGuid);

            string copyFilesPhaseGuid = proj.AddCopyFilesBuildPhase(mainTargetGuid, "Embed App Extensions", "", "13");
            proj.AddFileToBuildSection(mainTargetGuid, copyFilesPhaseGuid, proj.GetTargetProductFileRef(newTargetGuid));
            proj.AddTargetDependency(mainTargetGuid, newTargetGuid);

            return newTargetGuid;
        }

        #endregion



        #region Embedded Binaries

        internal static readonly FlagList EmbeddedFrameworkBuildSettings = new FlagList
        {
            { "LD_RUNPATH_SEARCH_PATHS", "$(inherited)" },
            { "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks" },
        };

        /// <summary>
        /// Configures file for embed framework section for the given native target.
        ///
        /// This function also internally calls <code>proj.AddFileToBuild(targetGuid, fileGuid)</code>
        /// to ensure that the framework is added to the list of linked frameworks.
        ///
        /// If the target has already configured the given file as embedded framework, this function has
        /// no effect.
        ///
        /// A projects containing multiple native targets, a single file or folder reference can be
        /// configured to be built in all, some or none of the targets. The file or folder reference is
        /// added to appropriate build section depending on the file extension.
        /// </summary>
        /// <param name="proj">A project passed as this argument.</param>
        /// <param name="targetGuid">The GUID of the target as returned by [[TargetGuidByName()]].</param>
        /// <param name="fileGuid">The file GUID returned by [[AddFile]] or [[AddFolderReference]].</param>
        public static void AddFrameworkToEmbeddedBinaries(this PBXProject proj, string targetGuid, string fileGuid)
        {
            proj.AddFileToEmbedFrameworks(targetGuid, fileGuid);
            
            foreach (var configName in proj.BuildConfigNames())
            {
                var configGuid = proj.BuildConfigByName(targetGuid, configName);
                SetBuildFlagsFromDict(proj, configGuid, EmbeddedFrameworkBuildSettings);
            }
        }

        #endregion


        /// <summary>
        /// Adds an xcode assets folder
        /// </summary>
        /// <param name="proj">A project passed as this argument.</param>
        /// <param name="targetGuid">The GUID of the target to work with. Null value means the main Unity target</param>
        /// <param name="path">The physical path to the folder on the filesystem.</param>
        /// <param name="projectPath">The project path to the folder.</param>
        /// <returns>The GUID of the new assets bundle.</returns>
        public static string AddAssetsFolder(this PBXProject proj, string targetGuid, string path, string projectPath = null)
        {
            if (string.IsNullOrEmpty(targetGuid))
            {
                targetGuid = proj.GetUnityMainTargetGuid();
            }

            if (string.IsNullOrEmpty(projectPath))
            {
                projectPath = path;
            }

            string guid = proj.AddFile(path, projectPath);
            proj.AddFileToBuild(targetGuid, guid);

            return guid;
        }
        
        
        public static void AddBuildProperty(this PbxProject proj, PbxProjectTargetType targetType, string name, string value)
        {
            proj.AddBuildProperty(proj.GetTargetGuid(targetType), name, value);
        }
    }
}
