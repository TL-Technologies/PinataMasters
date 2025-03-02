using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace Modules.Max.Editor
{
    [CustomEditor(typeof(LLMaxSettings))]
    public class MaxSettingsEditor : UnityEditor.Editor
    {
        private LLMaxSettings settings;
        private List<Type> adapters;
        
        
        private void OnEnable()
        {
            settings = target as LLMaxSettings;
            adapters = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(wh => wh.IsSubclassOf(typeof(MaxAdapter))).ToList();
        }
        

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            GUILayout.Space(10);
            
            GUILayout.Label("Adapters", EditorStyles.boldLabel);

            EditorGUI.BeginDisabledGroup (true);
            EditorGUILayout.Toggle("AppLovin", true);
            EditorGUI.EndDisabledGroup();
            
            EditorGUI.BeginChangeCheck();
            {
                foreach (var adapter in adapters)
                {
                    if (adapter.Name == nameof(AppLovinAdapter))
                    {
                        continue;
                    }
                    
                    bool wasEnabled = settings.EnabledAdapters.Contains(adapter.Name);
                    EditorGUILayout.BeginHorizontal();
                    {
                        bool isEnabled = EditorGUILayout.Toggle(adapter.Name, wasEnabled);
                        if (isEnabled && !wasEnabled)
                        {
                            settings.EnabledAdapters.Add(adapter.Name);
                            settings.EnabledAdapters.Sort();
                        }
                        else if (!isEnabled && wasEnabled)
                        {
                            settings.EnabledAdapters.Remove(adapter.Name);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(settings);
            }
            

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("Consent classes list", EditorStyles.boldLabel);
            EditorGUI.BeginDisabledGroup(true);
            foreach (var consentClassesInfo in settings.ConsentApiClassesNamesIncludingAssemblies)
            {
                EditorGUILayout.TextField(consentClassesInfo);
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Update consent classes"))
            {
                ActualizeConsentClasses(settings);
            }
        }


        private static void ActualizeConsentClasses(LLMaxSettings settings)
        {
            settings.ConsentApiClassesNamesIncludingAssemblies.Clear();
        
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                string asmName = asm.GetName().Name;
                settings.ConsentApiClassesNamesIncludingAssemblies.AddRange(asm.DefinedTypes
                    .Where(p => settings.ConsentApiClassesNames.Contains(p.Name))
                    .Select(p => $"{p.FullName}, {asmName}"));
            }

            EditorUtility.SetDirty(settings);
        }
    }
    
    
}
