using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GUILayoutCell))]
public class GUILayouterCellEditor : Editor 
{
	public override void OnInspectorGUI()
	{		
		base.OnInspectorGUI();

		GUILayoutCell targetLayouterCell = (GUILayoutCell)target;

		GUILayout.Space(10);
		EditorGUILayout.Separator();
		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create VerticalLayouter", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<GUILayouter>(targetLayouterCell, "VerticalLayout", delegate(GUILayouter result)
                {
                    result.Type = GUILayouterType.Vertical;
                });
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create HorizontalLayouter", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<GUILayouter>(targetLayouterCell, "HorizontalLayout", delegate(GUILayouter result)
                {
                    result.Type = GUILayouterType.Horizontal;
                });
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create TextMesh", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<tk2dTextMesh>(targetLayouterCell, "Label");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create Independent Sprite", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<tk2dSprite>(targetLayouterCell, "Sprite", null, false);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create Fill Sprite", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<tk2dSprite>(targetLayouterCell, "Sprite");
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create SlicedSprite", GUILayout.MinWidth(20))) 
		{
            CreateHandlerObject<tk2dSlicedSprite>(targetLayouterCell, "SlicedSprite");
		}
		EditorGUILayout.EndHorizontal();
	}


    void CreateHandlerObject<T>(GUILayoutCell targetCell, string objectName, 
        System.Action<T> onCreateCallback = null, bool isNeedHandle = true) where T : MonoBehaviour
    {
        GameObject newObject = new GameObject(objectName);
        T newHandlerObject = newObject.AddComponent<T>();

        newObject.transform.parent = targetCell.CachedTransform;
        newObject.transform.localPosition = Vector3.zero;
        newObject.layer = targetCell.gameObject.layer;

        if (isNeedHandle)
        {
            targetCell.LayoutHandlerObjects.Add(newObject);
        }

        if (onCreateCallback != null)
        {
            onCreateCallback(newHandlerObject);
        }
    }
}
