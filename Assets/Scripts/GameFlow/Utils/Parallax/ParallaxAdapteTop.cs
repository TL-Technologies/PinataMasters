using UnityEngine;

namespace PinataMasters
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxAdapteTop : MonoBehaviour
    {
        #region Unity lifecycle

        private void Awake()
        {
            transform.position = new Vector3(transform.position.x, Camera.main.orthographicSize, transform.position.z);
        }

        #endregion
    }
}

