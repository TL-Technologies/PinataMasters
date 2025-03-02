using Modules.Legacy.TextureManagement.Collections;
using UnityEditor;
using UnityEngine;


namespace Modules.Legacy.TextureManagement.Editor.Inspectors
{
	[CustomEditor(typeof(tmTextureCollection))]
	public class tmTextureCollectionEditor : UnityEditor.Editor
	{

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			if (GUILayout.Button("Build"))
			{
				tmCollectionBuilder.BuildCollection(target as tmTextureCollection);
			}
		}
	}
}
