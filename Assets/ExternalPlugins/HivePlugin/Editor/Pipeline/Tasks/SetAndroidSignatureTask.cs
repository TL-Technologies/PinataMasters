using HivePlugin.Editor.BuildUtilities.PlatformUtilities.Android;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Pipeline;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;



namespace Modules.Hive.Editor.Pipeline
{
    public class SetAndroidSignatureTask : EditorPipelineTask<BuildPipelineContext>
    {
        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, BuildPipelineContext context)
        {
            BuildTarget buildTarget = context.BuildOptions.target;

            if (buildTarget != BuildTarget.Android)
            {
                return SetStatus(PipelineTaskStatus.Succeeded);
            }

            AndroidSignatureSettings currentProjectSettings = FindSettings();
            if (currentProjectSettings == null)
            {
                return SetStatus(PipelineTaskStatus.Failed);
            }

            if (context.IsCiBuild && currentProjectSettings.PreferCliSettings)
            {
                return SetStatus(PipelineTaskStatus.Succeeded);
            }

            return SetStatus(SetSignatureInProjectSettings(currentProjectSettings) 
                    ? PipelineTaskStatus.Succeeded 
                    : PipelineTaskStatus.Failed);
            
        }
        
        
        private AndroidSignatureSettings FindSettings()
        {
            string[] guids = AssetDatabase.FindAssets("t:"+ nameof(AndroidSignatureSettings));

            if (guids.Length != 1)
            {
                Debug.LogError("Found 0 or more than 1 instance of AndroidSignatureSettings in project.");
                return null;
            }
            
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<AndroidSignatureSettings>(path);
        }

        
        private bool SetSignatureInProjectSettings(AndroidSignatureSettings settings)
        {
            PlayerSettings.Android.useCustomKeystore = true;

            switch (PlatformInfo.AndroidTarget)
            {
                case AndroidTarget.Amazon:
                    if (!settings.AmazonSettingsValid)
                    {
                        return false;
                    }
                    
                    PlayerSettings.Android.keystoreName = UnityPath.Combine(Application.dataPath,
                                                                            settings.AmazonKeystoreName);
                    PlayerSettings.Android.keystorePass = settings.HuaweiKeyaliasName;
                    PlayerSettings.Android.keyaliasName = settings.AmazonKeyaliasName;
                    PlayerSettings.Android.keyaliasPass = settings.AmazonKeyaliasPass;
                    break;
                    
                case AndroidTarget.Huawei:
                    if (!settings.HuaweiSettingsValid)
                    {
                        return false;
                    }
                    
                    PlayerSettings.Android.keystoreName = UnityPath.Combine(Application.dataPath,
                                                                            settings.HuaweiKeystoreName);
                    PlayerSettings.Android.keystorePass = settings.HuaweiKeystorePass;
                    PlayerSettings.Android.keyaliasName = settings.HuaweiKeyaliasName;
                    PlayerSettings.Android.keyaliasPass = settings.HuaweiKeyaliasPass;
                    break;
                
                case AndroidTarget.None:
                case AndroidTarget.GooglePlay:
                    if (!settings.GooglePlaySettingsValid)
                    {
                        return false;
                    }
                    
                    PlayerSettings.Android.keystoreName = UnityPath.Combine(Application.dataPath,
                                                                             settings.GooglePlayKeystoreName);
                    PlayerSettings.Android.keystorePass = settings.GooglePlayKeystorePass;
                    PlayerSettings.Android.keyaliasName = settings.GooglePlayKeyaliasName;
                    PlayerSettings.Android.keyaliasPass = settings.GooglePlayKeyaliasPass;
                    break;

                case AndroidTarget.Samsung:
                default:
                    return true;
            }

            return true;
        }

    }
}