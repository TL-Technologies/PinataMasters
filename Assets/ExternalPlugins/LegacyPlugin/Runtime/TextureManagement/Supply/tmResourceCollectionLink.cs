using Modules.Legacy.TextureManagement.Collections;
using UnityEngine;


namespace Modules.Legacy.TextureManagement.Supply
{
	public class tmResourceCollectionLink : ScriptableObject
	{
		#if UNITY_EDITOR
			public tmTextureCollectionPlatform collectionInEditor;
		#endif

		public tmTextureCollectionPlatform collection;
	}
}
