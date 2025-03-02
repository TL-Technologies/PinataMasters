#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace HivePlugin.Editor.BuildUtilities.PlatformUtilities.Android
    {
        [CustomEditor(typeof(AndroidSignatureSettings))]
        public class AndroidSignatureSettingsEditor : UnityEditor.Editor
        {
            private AndroidSignatureSettings targetSettings;
            
            private void OnEnable()
            {
                targetSettings = (AndroidSignatureSettings)target;
            }

            public override void OnInspectorGUI()
            {
                targetSettings.PreferCliSettings = GUILayout.Toggle(targetSettings.PreferCliSettings, "Prefer CI signature settings");
                
                GUILayout.Label($"Amazon Keystore file: {targetSettings.AmazonKeystoreName}");
                if (GUILayout.Button("Select Amazon Keystore"))
                {
                    targetSettings.AmazonKeystoreName =
                        EditorUtility.OpenFilePanel("Select Keystore file", Application.dataPath, "keystore");
                }

                targetSettings.AmazonKeystorePass =
                    EditorGUILayout.TextField("Amazon KeyStore Pass: ", targetSettings.AmazonKeystorePass);
                targetSettings.AmazonKeyaliasName =
                    EditorGUILayout.TextField("Amazon Keyalias Name: ", targetSettings.AmazonKeyaliasName);
                targetSettings.AmazonKeyaliasPass =
                    EditorGUILayout.TextField("Amazon Keyalias Pass: ", targetSettings.AmazonKeyaliasPass);
                
                GUILayout.Label($"Google Play Keystore file: {targetSettings.GooglePlayKeystoreName}");
                if (GUILayout.Button("Select Google Play Keystore"))
                {
                    targetSettings.GooglePlayKeystoreName =
                        EditorUtility.OpenFilePanel("Select Keystore file", Application.dataPath, "keystore");
                }

                targetSettings.GooglePlayKeystorePass =
                    EditorGUILayout.TextField("Google Play KeyStore Pass: ", targetSettings.GooglePlayKeystorePass);
                targetSettings.GooglePlayKeyaliasName =
                    EditorGUILayout.TextField("Google Play Keyalias Name: ", targetSettings.GooglePlayKeyaliasName);
                targetSettings.GooglePlayKeyaliasPass =
                    EditorGUILayout.TextField("Google Play Keyalias Pass: ", targetSettings.GooglePlayKeyaliasPass);

                GUILayout.Label($"Huawei Keystore file: {targetSettings.HuaweiKeystoreName}");
                if (GUILayout.Button("Select Huawei Keystore"))
                {
                    targetSettings.HuaweiKeystoreName =
                        EditorUtility.OpenFilePanel("Select Keystore file", Application.dataPath, "keystore");
                }
                targetSettings.HuaweiKeystorePass =
                    EditorGUILayout.TextField("Huawei KeyStore Pass: ", targetSettings.HuaweiKeystorePass);
                targetSettings.HuaweiKeyaliasName =
                    EditorGUILayout.TextField("Huawei Keyalias Name: ", targetSettings.HuaweiKeyaliasName);
                targetSettings.HuaweiKeyaliasPass =
                    EditorGUILayout.TextField("Huawei Keyalias Pass: ", targetSettings.HuaweiKeyaliasPass);
                
                EditorUtility.SetDirty(target);
            }
        }
    }

#endif