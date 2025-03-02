using Modules.General.Abstraction;
using UnityEditor;
using UnityEngine;


namespace Modules.Advertising.Editor
{
    [CustomPropertyDrawer(typeof(ModuleAttribute))]
    public class ModulePropertyDrawer : PropertyDrawer
    {
        #region Methods

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool enabled = IsPropertyEnabled(property);
            bool wasEnabled = GUI.enabled;
            GUI.enabled = enabled;
 
            if (enabled)
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
 
            GUI.enabled = wasEnabled;
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsPropertyEnabled(property))
            {
                return EditorGUI.GetPropertyHeight(property, label);
            }
            
            return -EditorGUIUtility.standardVerticalSpacing;
        }


        private bool IsPropertyEnabled(SerializedProperty property)
        {
            ModuleAttribute module = attribute as ModuleAttribute;
            AdModule type = (AdModule) property.serializedObject.FindProperty("supportedModules").intValue;

            return (module?.AdModuleType & type) != 0;
        }

        #endregion
    }
}