using Modules.Legacy.TextureManagement.Renders;
using UnityEditor;


namespace Modules.Legacy.TextureManagement.Editor.Inspectors
{
	[CustomEditor(typeof(tmBatchRender))]
	public class tmBatchRenderEditor : tmTextureRenderBaseEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
		}
	}
}