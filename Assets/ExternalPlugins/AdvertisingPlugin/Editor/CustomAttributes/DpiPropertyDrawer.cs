using UnityEditor;
using UnityEngine;


namespace Modules.Advertising.Editor
{
    [CustomPropertyDrawer(typeof(DeviceDpiAttribute))]
    public class DpiPropertyDrawer : PropertyDrawer
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
            DeviceDpiAttribute dpi = attribute as DeviceDpiAttribute;
            SerializedProperty deviceSettingsProperty = property.serializedObject.FindProperty("customBannerSettings");

            if (deviceSettingsProperty == null)
            {
                return true;
            }

            CustomBannerSettings.DeviceDPI deviceSettingsType = (CustomBannerSettings.DeviceDPI) deviceSettingsProperty.FindPropertyRelative("currentDeviceDPI").intValue;
            return dpi?.DpiType == deviceSettingsType;
        }

        #endregion
    }
}