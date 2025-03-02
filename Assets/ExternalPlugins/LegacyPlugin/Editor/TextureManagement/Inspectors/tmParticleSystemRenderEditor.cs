﻿using Modules.Legacy.TextureManagement.Renders;
using UnityEditor;


namespace Modules.Legacy.TextureManagement.Editor.Inspectors
{
	[CustomEditor(typeof(tmParticleSystemRender))]
	public class tmParticleSystemRenderEditor : tmTextureRenderBaseEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			tmParticleSystemRender system = target as tmParticleSystemRender;
			EditorGUILayout.BeginHorizontal();
			{
				system.UseRenderQueue = EditorGUILayout.Toggle("Render Queue", system.UseRenderQueue);
				if (system.UseRenderQueue)
				{
					system.RenderQueue = EditorGUILayout.IntField(system.RenderQueue);
				}
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
