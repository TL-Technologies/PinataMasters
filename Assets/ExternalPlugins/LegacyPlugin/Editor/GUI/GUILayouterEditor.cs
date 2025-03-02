using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(GUILayouter))]
public class GUILayouterEditor : Editor 
{
    public override void OnInspectorGUI()
	{		
        GUILayouter targetLayouter = (GUILayouter)target;
        if (Event.current.type == EventType.Layout)
        {
            CheckRootLayouter(); 
        }

		base.OnInspectorGUI();		

        if (targetLayouter.IsRootLayouter)
        {
            EditorGUILayout.BeginHorizontal();
            targetLayouter.RotationType = (GUILayouterRotationType)EditorGUILayout.EnumPopup("Rotation Type: ", targetLayouter.RotationType);
            EditorGUILayout.EndHorizontal();
        }

		GUILayout.Space(10);
		EditorGUILayout.Separator();
		GUILayout.Space(10);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Create\nFlexible cell", GUILayout.MinWidth(20))) 
		{
            CreateCell(targetLayouter, "__FlexSpace", GUILayoutCellType.Flexible, 1);
		}
		if (GUILayout.Button ("Create\nFixedSize cell", GUILayout.MinWidth(20))) 
		{
            CreateCell(targetLayouter, "__FixedSpace", GUILayoutCellType.FixedSize, 0);
		}
		if (GUILayout.Button ("Create\nRelativeFixed cell", GUILayout.MinWidth(20))) 
		{
            CreateCell(targetLayouter, "__RelativeFixedSpace", GUILayoutCellType.RelativeFixedSize, 0);
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button ("Verify Hierarchy", GUILayout.MinWidth(20))) 
		{
			bool isHierarchyCorrupted = false;
			GUILayoutCell[] allCells = targetLayouter.CachedTransform.GetComponentsInChildren<GUILayoutCell> ();

			foreach (var cell in allCells) 
			{
				foreach (var handlerObj in cell.LayoutHandlerObjects) 
				{
					if (handlerObj == null) 
					{
                        CustomDebug.LogError("NULL Handler Found!");
						Selection.activeGameObject = cell.gameObject;
						isHierarchyCorrupted = true;
						break;
					}
				}	

				if (isHierarchyCorrupted) 
				{
					break;
				}
			}

			if (!isHierarchyCorrupted) 
			{
                CustomDebug.Log("NO NULL Handlers Found!");
			}
		}
		EditorGUILayout.EndHorizontal();
	}


    void CheckRootLayouter()
    {
        GUILayouter targetLayouter = (GUILayouter)target;
        targetLayouter.IsRootLayouter = true;

        GUILayoutCell[] parentCells = targetLayouter.GetComponentsInParent<GUILayoutCell>(true);
        foreach (var cell in parentCells)
        {
            if (cell.LayoutHandlerObjects.Contains(targetLayouter.gameObject))
            {
                targetLayouter.IsRootLayouter = false;
            }
        }
    }


    void CreateCell(GUILayouter parentLayouter, string cellName, GUILayoutCellType cellType, float cellSize)
    {
        GameObject newCellObj = new GameObject(cellName);
        GUILayoutCell newCell = newCellObj.AddComponent<GUILayoutCell>();
        newCell.Type = cellType;
        newCell.SizeValue = cellSize;

        newCellObj.transform.parent = parentLayouter.CachedTransform;
        newCellObj.transform.localPosition = Vector3.zero;
        newCellObj.layer = parentLayouter.gameObject.layer;
    }
}