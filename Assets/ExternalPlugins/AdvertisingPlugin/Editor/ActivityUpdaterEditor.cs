using UnityEditor;
using UnityEngine;

namespace Modules.Advertising.Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(InactivityTimer))]
    public class ActivityUpdaterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.LabelField($"Inactivity timer : {(target as InactivityTimer).InactivityTime.ToString()}");
        }
    }
}