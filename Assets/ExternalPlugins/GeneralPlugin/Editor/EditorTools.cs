using UnityEditor;
using UnityEngine;



public static class EditorTools 
{
    public static Vector3 Validate(Vector3 v) 
    {
        v.x = float.IsNaN(v.x) ? 0f : v.x;
        v.y = float.IsNaN(v.y) ? 0f : v.y;
        v.z = float.IsNaN(v.z) ? 0f : v.z;
        
        return v;
    }
    
    
    public static void RegisterUndo(string name, params Object[] objects) 
    {
        if ((objects != null) && (objects.Length > 0)) 
        {
            foreach (Object obj in objects) 
            {
                if (obj == null) 
                {
                    continue;
                }
                Undo.RecordObject(obj, name);
                EditorUtility.SetDirty(obj);
            }
        } 
        else 
        {
//            Undo.RegisterSceneUndo(name);
        }
    }

    
    
    public static Vector3 DrawVector3(Vector3 value) 
    {
        GUILayoutOption opt = GUILayout.MinWidth(30f);
        value.x = EditorGUILayout.FloatField("X", value.x, opt);
        value.y = EditorGUILayout.FloatField("Y", value.y, opt);
        value.z = EditorGUILayout.FloatField("Z", value.z, opt);
        
        return value;
    }

    
    
    public static Vector2 DrawVector2(Vector2 value) 
    {
        GUILayoutOption opt = GUILayout.MinWidth(45f);
        value.x = EditorGUILayout.FloatField("X", value.x, opt);
        value.y = EditorGUILayout.FloatField("Y", value.y, opt);
        
        return value;
    }
    
    
    
    public static float DrawFloatField(float value, string title, string tooltip, bool enabled, params GUILayoutOption[] options) 
    {
        if (enabled)
        {
            return EditorGUILayout.FloatField(new GUIContent(title, tooltip), value, options);
        }
        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.FloatField(new GUIContent(title, tooltip), value, options);
        GUI.color = saveColor;
        
        return value;
    }


    public static float DrawFloatField(float value, string title, bool enabled, params GUILayoutOption[] options)
    {
        if (enabled)
        {
            return EditorGUILayout.FloatField(title, value, options);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.FloatField(title, value, options);
        GUI.color = saveColor;

        return value;
    }


    public static float DrawFloatField(float value, string title, bool enabled = true)
    {
        if (enabled)
        {
            return EditorGUILayout.FloatField(title, value);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.FloatField(title, value);
        GUI.color = saveColor;

        return value;
    }


    public static System.Enum DrawEnumField(System.Enum value, string title, string tooltip, bool enabled,
        params GUILayoutOption[] options)
    {
        if (enabled)
        {
            return EditorGUILayout.EnumPopup(new GUIContent(title, tooltip), value, options);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.EnumPopup(new GUIContent(title, tooltip), value, options);
        GUI.color = saveColor;

        return value;
    }


    public static System.Enum DrawEnumField(System.Enum value, string title, bool enabled,
        params GUILayoutOption[] options)
    {
        if (enabled)
        {
            return EditorGUILayout.EnumPopup(title, value, options);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.EnumPopup(title, value, options);
        GUI.color = saveColor;

        return value;
    }


    public static System.Enum DrawEnumField(System.Enum value, string title, bool enabled = true)
    {
        if (enabled)
        {
            return EditorGUILayout.EnumPopup(title, value);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        EditorGUILayout.EnumPopup(title, value);
        GUI.color = saveColor;

        return value;
    }


    public static bool DrawButton(string title, string tooltip, bool enabled, float width)
    {
        if (enabled)
        {
            return GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Button(new GUIContent(title, tooltip), GUILayout.Width(width));
        GUI.color = saveColor;

        return false;
    }


    public static bool DrawButton(string title, bool enabled, float width)
    {
        if (enabled)
        {
            return GUILayout.Button(title, GUILayout.Width(width));
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Button(title, GUILayout.Width(width));
        GUI.color = saveColor;

        return false;
    }
    

    public static bool DrawButton(string title, string tooltip, bool enabled)
    {
        if (enabled)
        {
            return GUILayout.Button(new GUIContent(title, tooltip));
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Button(new GUIContent(title, tooltip));
        GUI.color = saveColor;

        return false;
    }


    public static bool DrawButton(string title, bool enabled = true)
    {
        if (enabled)
        {
            return GUILayout.Button(title);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Button(title);
        GUI.color = saveColor;

        return false;
    }
    

    public static bool DrawToggle(bool value, string title, string tooltip, bool enabled, float width)
    {
        if (enabled)
        {
            return GUILayout.Toggle(value, new GUIContent(title, tooltip), GUILayout.Width(width));
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Toggle(value, new GUIContent(title, tooltip), GUILayout.Width(width));
        GUI.color = saveColor;

        return value;
    }
    

    public static bool DrawToggle(bool value, string title, bool enabled, float width)
    {
        if (enabled)
        {
            return GUILayout.Toggle(value, title, GUILayout.Width(width));
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Toggle(value, title, GUILayout.Width(width));
        GUI.color = saveColor;

        return value;
    }

    
    public static bool DrawToggle(bool value, string title, bool enabled = true)
    {
        if (enabled)
        {
            return GUILayout.Toggle(value, title);
        }

        Color saveColor = GUI.color;
        GUI.color = new Color(1f, 1f, 1f, 0.25f);
        GUILayout.Toggle(value, title);
        GUI.color = saveColor;

        return value;
    }
    

    public static void DrawLabel(string title, string tooltip, bool enabled, params GUILayoutOption[] options)
    {
        Color saveColor = GUI.color;
        if (!enabled)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.25f);
        }

        EditorGUILayout.LabelField(new GUIContent(title, tooltip), options);
        GUI.color = saveColor;
    }
    

    public static void DrawLabel(string title, bool enabled = true, params GUILayoutOption[] options)
    {
        Color saveColor = GUI.color;
        if (!enabled)
        {
            GUI.color = new Color(1f, 1f, 1f, 0.25f);
        }

        EditorGUILayout.LabelField(title, options);
        GUI.color = saveColor;
    }
}