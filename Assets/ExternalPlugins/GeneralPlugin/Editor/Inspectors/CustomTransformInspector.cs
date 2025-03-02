using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;


[CustomEditor(typeof(Transform)), CanEditMultipleObjects]
public class CustomTransformInspector : Editor
{
    #region Fields
    
    Editor defaultTransformInspector = null;
    static readonly Type DefaultTransformInspectorType =
        #if UNITY_2020_1_OR_NEWER
            GetTypeByNameFromAssembly("UnityEditor.TransformInspector", "UnityEditor.CoreModule");
        #else
            GetTypeByNameFromAssembly("UnityEditor.TransformInspector", "UnityEditor");
        #endif

    static Vector3 bufferPosition = Vector3.zero;
    static Quaternion bufferRotation = Quaternion.identity;
    static Vector3 bufferScale = Vector3.one;
    static bool useBuffer;
    static GUIContent emptyGuiContent = new GUIContent();
    SerializedProperty positionProperty;
    SerializedProperty rotationProperty;
    SerializedProperty scaleProperty;

    #endregion



    #region Unity Lifecycle

    void OnEnable()
    {
        defaultTransformInspector = CreateEditor(targets, DefaultTransformInspectorType);
        MethodInfo defaultTransformInspectorOnEnableMethod = DefaultTransformInspectorType.GetMethod("OnEnable");
        defaultTransformInspectorOnEnableMethod.Invoke(defaultTransformInspector, new object[] { });
        
        positionProperty = serializedObject.FindProperty("m_LocalPosition");
        rotationProperty = serializedObject.FindProperty("m_LocalRotation");
        scaleProperty = serializedObject.FindProperty("m_LocalScale");
    }
    
    
    void OnDisable()
    {
        DestroyImmediate(defaultTransformInspector);
    }


    public override void OnInspectorGUI()
    {
        if (CustomEditorPreferences.EnabledCustomTransformEditor)
        {
            DrawCustomEditor();
        }
        else
        {
            defaultTransformInspector.OnInspectorGUI();
        }
    }

    #endregion



    #region Private methods
    
    static Type GetTypeByNameFromAssembly(string typeFullName, string assemblyName)
    {
        Type result = null;
        Assembly assembly = Assembly.Load(assemblyName);
        if (assembly != null)
        {
            Type[] types = assembly.GetTypes();

            foreach (Type t in types)
            {
                if (t.FullName.Equals(typeFullName))
                {
                    result = t;
                    break;
                }
            }
        }
        
        return result;
    }
    

    void DrawCustomEditor()
    {
        serializedObject.Update();

        EditorGUILayout.PrefixLabel("Position");
        EditorGUILayout.BeginHorizontal();
        if (EditorTools.DrawButton("P", "Reset position", IsResetPositionValid(positionProperty.vector3Value), 20f))
        {
            EditorTools.RegisterUndo("Reset Position", target);
            positionProperty.vector3Value = Vector3.zero;
        }
        EditorGUILayout.PropertyField(positionProperty, emptyGuiContent);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PrefixLabel("Rotation");
        EditorGUILayout.BeginHorizontal();
        if (EditorTools.DrawButton("R", "Reset rotation", IsResetRotationValid(rotationProperty.quaternionValue), 20f))
        {
            EditorTools.RegisterUndo("Reset Rotation", target);
            rotationProperty.quaternionValue = new Quaternion();
        }
        RotationPropertyField(rotationProperty, emptyGuiContent);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.PrefixLabel("Scale");
        EditorGUILayout.BeginHorizontal();
        if (EditorTools.DrawButton("S", "Reset scale", IsResetScaleValid(scaleProperty.vector3Value), 20f))
        {
            EditorTools.RegisterUndo("Reset Scale", target);
            scaleProperty.vector3Value = Vector3.one;
        }
        EditorGUILayout.PropertyField(scaleProperty, emptyGuiContent);
        EditorGUILayout.EndHorizontal();

        GUILayout.Space(10f);
        EditorGUILayout.BeginHorizontal();
        useBuffer = EditorTools.DrawToggle(useBuffer, string.Empty, "Use buffer for copy and paste transform values ", true, 20f);
        if (EditorTools.DrawButton("Copy to buffer", useBuffer))
        {
            bufferPosition = positionProperty.vector3Value;
            bufferRotation = rotationProperty.quaternionValue;
            bufferScale = scaleProperty.vector3Value;
        }
        if (EditorTools.DrawButton("Paste from buffer", useBuffer))
        {
            positionProperty.vector3Value = bufferPosition;
            rotationProperty.quaternionValue = bufferRotation;
            scaleProperty.vector3Value = bufferScale;
        }
        EditorGUILayout.EndHorizontal();
        if (GUI.changed)
        {
            EditorTools.RegisterUndo("Transform Change", target);
        }

        GUILayout.Space(5f);

        serializedObject.ApplyModifiedProperties();
    }


    void RotationPropertyField(SerializedProperty rotationProperty, GUIContent content)
    {
        Transform transform = (Transform)targets[0];
        Quaternion localRotation = transform.localRotation;
        foreach (Object t in targets)
        {
            if (!IsSameRotation(localRotation, ((Transform)t).localRotation))
            {
                EditorGUI.showMixedValue = true;
                break;
            }
        }

        EditorGUI.BeginChangeCheck();

        Vector3 eulerAngles = EditorGUILayout.Vector3Field(content, localRotation.eulerAngles);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObjects(targets, "Rotation Changed");
            foreach (Object obj in targets)
            {
                Transform t = (Transform)obj;
                t.localEulerAngles = eulerAngles;
            }
            rotationProperty.serializedObject.SetIsDifferentCacheDirty();
        }

        EditorGUI.showMixedValue = false;
    }


    bool IsSameRotation(Quaternion rot1, Quaternion rot2)
    {
        return Mathf.Approximately(rot1.x, rot2.x) && 
            Mathf.Approximately(rot1.y, rot2.y) && 
            Mathf.Approximately(rot1.z, rot2.z) && 
            Mathf.Approximately(rot1.w, rot2.w);
    }


    bool IsResetPositionValid(Vector3 pos)
    {
        return !Mathf.Approximately(pos.x, 0.0f) || 
            !Mathf.Approximately(pos.y, 0.0f) || 
            !Mathf.Approximately(pos.z, 0.0f);
    }


    bool IsResetRotationValid(Quaternion rotation)
    {
        return !Mathf.Approximately(rotation.x, 0.0f) || 
            !Mathf.Approximately(rotation.y, 0.0f) || 
            !Mathf.Approximately(rotation.z, 0.0f) || 
            !Mathf.Approximately(rotation.w, 1.0f);
    }


    bool IsResetScaleValid(Vector3 scale)
    {
        return !Mathf.Approximately(scale.x, 1f) || 
            !Mathf.Approximately(scale.y, 1f) || 
            !Mathf.Approximately(scale.z, 1f);
    }

    #endregion
}