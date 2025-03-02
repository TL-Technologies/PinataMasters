using UnityEngine;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class SwitchAndroidTargetDirectiveWindow : EditorWindow
    {
        #region Fields
    
        private static AndroidTarget selected;
    
        #endregion
    
    
    
        #region Methods
    
        [MenuItem("Modules/Hive/SwitchAndroidTarget")]
        public static void Init()
        {
            GetWindow<SwitchAndroidTargetDirectiveWindow>().Show();
        }
        
        
        void OnGUI()
        {
            EditorGUILayout.Space(10);
            
            EditorGUILayout.LabelField("Current android target", PlatformInfo.AndroidTarget.ToString(), EditorStyles.boldLabel);
            
            EditorGUILayout.Space(10);
    
            selected = (AndroidTarget)EditorGUILayout.EnumPopup("Select android target", selected);
            
            EditorGUILayout.Space(10);
    
            EditorGUI.BeginDisabledGroup(selected == AndroidTarget.None || selected == PlatformInfo.AndroidTarget);
            {
                if (GUILayout.Button("Update directives"))
                {
                    SwitchAndroidTarget(selected);
                }
            }
            EditorGUI.EndDisabledGroup();
        }
        
        
        private static void SwitchAndroidTarget(AndroidTarget target)
        {
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                Debug.LogError($"Can't switch android target : need BuildTarget.Android, current {EditorUserBuildSettings.activeBuildTarget}");
                return;
            }
            if (PlatformInfo.AndroidTarget == target)
            {
                Debug.LogError($"Can't switch android target : already {target}");
                return;
            }
            
            BuildPipelineFactory.CreatePipeline(new UpdateDirectivesBuildPipeline(), target).BuildAndSubmit();
        }
    
        #endregion
    }
}