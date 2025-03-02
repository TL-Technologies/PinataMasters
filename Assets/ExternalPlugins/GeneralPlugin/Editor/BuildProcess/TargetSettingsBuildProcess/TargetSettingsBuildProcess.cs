using Modules.General.HelperClasses;
using Modules.Hive;
using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.Pipeline;
using System.IO;


namespace Modules.BuildProcess
{
    [EditorPipelineOptions(AppHostLayer = AppHostLayer.PreApplication)]
    public class TargetSettingsBuildProcess : IBuildPreprocessor<IBuildPreprocessorContext>
    {
        #region Fields

        private const string TargetSettingsFolderName = "TargetSettings";
        private const string ChinaFolderName = "China";
        private const string CommonFolderName = "Common";

        #endregion



        #region Methods

        public void OnPreprocessBuild(IBuildPreprocessorContext context)
        {
            string targetSettingsPath = 
                UnityPath.Combine(UnityPath.ExternalPluginsSettingsAssetPath, TargetSettingsFolderName);
            if (Directory.Exists(targetSettingsPath))
            {
                ITargetSettingsActualizer targetSettingsActualizer;
                if (BuildInfo.IsChinaBuild)
                {
                    targetSettingsActualizer = new ConcreteTargetSettingsActualizer(ChinaFolderName);
                }
                else
                {
                    targetSettingsActualizer = new ConcreteTargetSettingsActualizer(CommonFolderName);
                }

                if (!targetSettingsActualizer.TryActualizeSettingsAtPath(targetSettingsPath, context))
                {
                    targetSettingsActualizer = new DefaultTargetSettingsActualizer();
                    targetSettingsActualizer.TryActualizeSettingsAtPath(targetSettingsPath, context);
                }
            }
        }

        #endregion
    }
}
