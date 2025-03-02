using Modules.General.HelperClasses;
using UnityEngine;


namespace Modules.Legacy.TextureManagement.Batching
{
	public class tmStaticBatchedGameObject : MonoBehaviour
	{
		[SerializeField] [ResourceLink] AssetLink staticLink;

		public AssetLink StaticLink
		{
			get { return staticLink; }
		}
	}
}
