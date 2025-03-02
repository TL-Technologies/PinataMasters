using Modules.Hive;
using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using System;
using System.IO;
using UnityEditor;


namespace Modules.BuildProcess
{
    public class DefaultTargetSettingsActualizer : ITargetSettingsActualizer
    {
        #region Fields

        private const string IOSFolderName = "iOS";
        private const string AndroidFolderName = "Android";
        private const string AmazonFolderName = "Amazon";
        private const string HuaweiFolderName = "Huawei";
        private const string StandaloneFolderName = "Standalone";

        #endregion



        #region Methods

        public virtual bool TryActualizeSettingsAtPath(string settingsPath, IBuildPreprocessorContext context)
        {
            if (Directory.Exists(settingsPath))
            {
                string targetSettingsDirectoryName;

                if (context.BuildTarget == BuildTarget.iOS)
                {
                    targetSettingsDirectoryName = IOSFolderName;
                }
                else if (context.BuildTarget == BuildTarget.Android)
                {
                    AndroidTarget androidTarget = PlatformInfo.AndroidTarget;
                    if (androidTarget == AndroidTarget.GooglePlay)
                    {
                        targetSettingsDirectoryName = AndroidFolderName;
                    }
                    else if (androidTarget == AndroidTarget.Amazon)
                    {
                        targetSettingsDirectoryName = AmazonFolderName;
                    }
                    else if (androidTarget == AndroidTarget.Huawei)
                    {
                        targetSettingsDirectoryName = HuaweiFolderName;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                else if (context.BuildTarget == BuildTarget.StandaloneWindows ||
                         context.BuildTarget == BuildTarget.StandaloneWindows64 ||
                         context.BuildTarget == BuildTarget.StandaloneLinux64 ||
                         context.BuildTarget == BuildTarget.StandaloneOSX)
                {
                    targetSettingsDirectoryName = StandaloneFolderName;
                }
                else
                {
                    throw new NotImplementedException();
                }

                foreach(DirectoryInfo directoryInfo in FileSystemUtilities.EnumerateDirectories(settingsPath, SearchOption.TopDirectoryOnly))
                {
                    if (directoryInfo.Name == targetSettingsDirectoryName || directoryInfo.Name == UnityPath.ResourcesDirectoryName)
                    {
                        ProjectSnapshot.CurrentSnapshot?.CopyAssetToStash(UnityPath.GetAssetPathFromFullPath(directoryInfo.FullName));
                    }
                    else
                    {
                        ProjectSnapshot.CurrentSnapshot?.MoveAssetToStash(UnityPath.GetAssetPathFromFullPath(directoryInfo.FullName));
                    }
                }

                return true;
            }

            return false;
        }

        #endregion
    }
}
