using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dTextMeshClickable))]
class tk2dTextMeshClickableEditor : tk2dTextMeshEditor
{
    #region Unity Lifecycle

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();

        SerializedProperty clickableColor = serializedObject.FindProperty("clickableColor");
        EditorGUILayout.PropertyField(clickableColor, true);

        SerializedProperty clickables = serializedObject.FindProperty("clickables");
        EditorGUILayout.PropertyField(clickables, true);

        SerializedProperty colliderExtends = serializedObject.FindProperty("colliderExtends");
        EditorGUILayout.PropertyField(colliderExtends, true);

        SerializedProperty lineThickness = serializedObject.FindProperty("lineThickness");
        EditorGUILayout.PropertyField(lineThickness, true);

        SerializedProperty lineUpOffset = serializedObject.FindProperty("lineUpOffset");
        EditorGUILayout.PropertyField(lineUpOffset, true);

        serializedObject.ApplyModifiedProperties ();
    }

    #endregion
}
