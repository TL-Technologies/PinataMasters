using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using System.IO;


namespace Modules.BuildProcess
{
    public class ConcreteTargetSettingsActualizer : DefaultTargetSettingsActualizer, IConcreteTargetSettingsActualizer
    {
        #region Properties

        public string FolderName { get; }

        #endregion



        #region Class lifecycle

        public ConcreteTargetSettingsActualizer(string folderName)
        {
            FolderName = folderName;
        }

        #endregion



        #region Methods

        public override bool TryActualizeSettingsAtPath(string settingsPath, IBuildPreprocessorContext context)
        {
            bool result = false;
            string concreteSettingsFolderPath = UnityPath.Combine(settingsPath, FolderName);
            
            if (Directory.Exists(concreteSettingsFolderPath))
            {
                foreach (string directory in Directory.GetDirectories(settingsPath))
                {
                    if (directory.Contains(FolderName))
                    {
                        result = base.TryActualizeSettingsAtPath(concreteSettingsFolderPath, context);
                    }
                    else
                    {
                        ProjectSnapshot.CurrentSnapshot?.MoveAssetToStash(directory);
                    }
                }
            }

            return result;
        }

        #endregion
    }
}
