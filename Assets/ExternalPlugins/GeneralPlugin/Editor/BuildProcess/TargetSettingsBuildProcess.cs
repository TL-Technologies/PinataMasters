using Modules.General.HelperClasses;
using Modules.Hive.Editor.BuildUtilities;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Modules.General.Editor.BuildProcess
{
    internal class TargetSettingsBuildProcess : 
        IBuildPreprocessor<IAndroidBuildPreprocessorContext>,
        IBuildPreprocessor<IIosBuildPreprocessorContext>
    {
        #region Fields

        private const string DefaultProjectSettingsPath = "ProjectSettings/ProjectSettings.asset";
        private const string PrefixCloudProjectId = "cloudProjectId: ";

        #endregion
        
        
        
        #region Properties

        private static string CloudProjectId
        {
            get
            {
                // When working without connection to Unity Services it can return empty string even if id is presented in ProjectSettings
                string result = CloudProjectSettings.projectId;

                if (string.IsNullOrEmpty(result))
                {
                    string pathTargetSettings = Application.dataPath.RemoveLastPathComponent()
                        .AppendPathComponent(DefaultProjectSettingsPath);
                    string content = string.Empty;
                    using (FileStream fs = new FileStream(pathTargetSettings, FileMode.Open))
                    {
                        using (StreamReader sr = new StreamReader(fs))
                        {
                            content = sr.ReadToEnd();
                        }
                    }

                    int startIndex = content.IndexOf(PrefixCloudProjectId, StringComparison.InvariantCulture) +
                        PrefixCloudProjectId.Length;
                    int length = content.Substring(startIndex).IndexOf("\n", StringComparison.InvariantCulture);
                    result = content.Substring(startIndex, length);
                }

                return result;
            }
        }

        #endregion
        
        
        
        #region Build process
        
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            ChangeProjectTargetSettings(BuildTargetGroup.Android);
        }
        

        public void OnPreprocessBuild(IIosBuildPreprocessorContext context)
        {
            ChangeProjectTargetSettings(BuildTargetGroup.iOS);
        }
        
        #endregion
        


        #region Unity lifecycle

        [InitializeOnLoadMethod]
        private static void InitializeOnLoadMethod()
        {
            EditorApplication.delayCall -= EditorApplication_DelayCall;
            EditorApplication.delayCall += EditorApplication_DelayCall;
        }

        #endregion



        #region Methods

        private static void ChangeProjectTargetSettings(BuildTargetGroup targetGroup)
        {
            if (TargetSettings.DoesInstanceExist)
            {
                if (!string.IsNullOrEmpty(TargetSettings.ApplicationName))
                {
                    PlayerSettings.productName = TargetSettings.ApplicationName;
                }

                if (!string.IsNullOrEmpty(TargetSettings.ApplicationCompany))
                {
                    PlayerSettings.companyName = TargetSettings.ApplicationCompany;
                }

                if (!string.IsNullOrEmpty(TargetSettings.ApplicationIdentifier))
                {
                    #if UNITY_2018_1_OR_NEWER
                        PlayerSettings.SetApplicationIdentifier(targetGroup, TargetSettings.ApplicationIdentifier);
                    #elif UNITY_5_6_OR_NEWER
                        PlayerSettings.applicationIdentifier = TargetSettings.TargetApplicationIdentifier;
                    #else
                        PlayerSettings.bundleIdentifier = TargetSettings.TargetApplicationIdentifier;
                    #endif
                }

                if (!string.IsNullOrEmpty(TargetSettings.VersionName))
                {
                    PlayerSettings.bundleVersion = TargetSettings.VersionName;
                }
                
                // It's need to save player settings changes before modifying it as usual file 
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                if (!string.IsNullOrEmpty(TargetSettings.UnityProjectId))
                {
                    string targetProjectId = TargetSettings.UnityProjectId;
                    string currentCloudProjectId = CloudProjectId;
                    if (targetProjectId != currentCloudProjectId)
                    {
                        string pathTargetSettings = Application.dataPath.RemoveLastPathComponent()
                            .AppendPathComponent(DefaultProjectSettingsPath);
                        string content;
                        using (FileStream fileStream = new FileStream(pathTargetSettings, FileMode.Open))
                        {
                            using (StreamReader streamReader = new StreamReader(fileStream))
                            {
                                content = streamReader.ReadToEnd();
                            }
                        }

                        string contentCloudId = PrefixCloudProjectId + currentCloudProjectId;
                        string contentTargetId = PrefixCloudProjectId + targetProjectId;
                        content = content.Replace(contentCloudId, contentTargetId);
                        
                        using (FileStream fileStream = new FileStream(pathTargetSettings, FileMode.Create))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(fileStream))
                            {
                                streamWriter.Write(content);
                            }
                        }
                    }
                }
                
                AssetDatabase.Refresh();
                AssetDatabase.SaveAssets();
            }
        }

        #endregion



        #region Events handlers

        private static void EditorApplication_DelayCall()
        {
            if (!Application.isPlaying && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                #if UNITY_IOS
                    ChangeProjectTargetSettings(BuildTargetGroup.iOS);
                #elif UNITY_ANDROID
                    ChangeProjectTargetSettings(BuildTargetGroup.Android);
                #endif
            }
        }

        #endregion
    }
}
