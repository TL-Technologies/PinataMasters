using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class ParallaxAdapteBottom : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private float maxTopValue = 0f;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            transform.position = new Vector3(transform.position.x, -Camera.main.orthographicSize, transform.position.z);

            if (transform.position.y > maxTopValue)
            {
                transform.position = new Vector3(transform.position.x, maxTopValue, transform.position.z);
            }
        }
        #endregion
    }
}
