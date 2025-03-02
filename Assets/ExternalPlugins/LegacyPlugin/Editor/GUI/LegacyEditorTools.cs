using UnityEditor;
using UnityEngine;



public class LegacyEditorTools
{
    public static void DrawInvokeData(InvokeData invoke)
    {
        EditorGUILayout.BeginHorizontal();
        if (EditorTools.DrawButton("R", "Reset script", (invoke.script != null), 20f))
        {
            invoke.script = invoke.newScript = null;
        }

        if (invoke.script != null)
        {
            string title = invoke.script.GetType().ToString();
            string tooltip = "Set component " + title;
            GUILayout.Label(new GUIContent(title, tooltip), GUILayout.MinWidth(20f), GUILayout.MaxWidth(90f));
        }

        MonoBehaviour newScript =
            (MonoBehaviour) EditorGUILayout.ObjectField(invoke.script, typeof(MonoBehaviour), true);
        if (newScript != invoke.script)
        {
            invoke.newScript = newScript;
        }

        invoke.method = EditorGUILayout.TextField(invoke.method);
        EditorGUILayout.EndHorizontal();
        if (invoke.newScript != null)
        {
            MonoBehaviour[] components = invoke.newScript.GetComponents<MonoBehaviour>();
            if (components.Length > 1)
            {
                for (int j = 0; j < components.Length; j++)
                {
                    if (EditorTools.DrawButton("Set " + components[j].GetType().ToString() + " script"))
                    {
                        invoke.script = components[j];
                        invoke.newScript = null;
                    }
                }

                if (EditorTools.DrawButton("Undo"))
                {
                    invoke.newScript = null;
                }

                EditorGUILayout.Space();
            }
            else
            {
                invoke.script = components[0];
                invoke.newScript = null;
            }
        }
    }
}