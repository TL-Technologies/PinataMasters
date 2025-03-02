using System;
using UnityEditor;
using UnityEngine;


public static class CustomEditorPreferences
{
    #region Nested types
    
    abstract class EditorPreferencesItem<T>
    {
        public string key;
        public GUIContent label;
        public T defaultValue;

        protected EditorPreferencesItem(string key, GUIContent label, T defaultValue)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("Key");
            }

            this.key = key;
            this.label = label;
            this.defaultValue = defaultValue;
        }

        public abstract T Value 
        {
            get; 
            set;
        }

        public abstract void Draw();

        public static implicit operator T(EditorPreferencesItem<T> s)
        {
            return s.Value;
        }
    }
    
    
    class EditorPreferencesBool : EditorPreferencesItem<bool>
    {
        public EditorPreferencesBool(string key, GUIContent label, bool defaultValue) : base(key, label, defaultValue) { }

        public override bool Value
        {
            get => EditorPrefs.GetBool(key, defaultValue); 
            set => EditorPrefs.SetBool(key, value); 
        }

        public override void Draw()
        {
            EditorGUIUtility.labelWidth = 150f;
            Value = EditorGUILayout.Toggle(label, Value);
        }
    }
    
    #endregion
    
    
    
    #region Fields
    
    static readonly EditorPreferencesItem<bool> CustomTransform;

    public static bool EnabledCustomTransformEditor;
    
    #endregion
    
    
    
    #region Properties
    
    static string ProjectName
    {
        get
        {
            var s = Application.dataPath.Split('/');
            var p = s[s.Length - 2];
            return p;
        }
    }
    
    #endregion

    
    
    #region Class lifecycle
    
    static CustomEditorPreferences()
    {
        GUIContent transformEditor = new GUIContent("Custom transform editor", 
            "Enable / disable custom editor for Transform component.");
        CustomTransform = new EditorPreferencesBool("Modules.Editor." + ProjectName, transformEditor, true);
        EnabledCustomTransformEditor = CustomTransform.Value;
    }
    
    #endregion
    
    
    
    #region Private methods
    
    [PreferenceItem("Custom Preferences")]
    static void EditorPreferences()
    {
        EditorGUILayout.Separator();
        CustomTransform.Draw();
        EnabledCustomTransformEditor = CustomTransform.Value;
    }
    
    #endregion
}
