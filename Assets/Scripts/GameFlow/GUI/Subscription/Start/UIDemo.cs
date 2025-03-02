using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIDemo : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private Demo demoPrefab = null;
        [SerializeField]
        private Vector2 demoPos = new Vector2();
        [SerializeField]
        private Camera demoCameraPrefab = null;
        [SerializeField]
        private RawImage rawImage = null;
        [SerializeField]
        private Material material;

        private Demo demo;
        private Camera demoCamera;

        #endregion



        #region Unity lifecycle

        private void OnEnable()
        {
            demo = Instantiate(demoPrefab, demoPos, Quaternion.identity);
            demo.Init();

            demoCamera = Instantiate(demoCameraPrefab, demoPos, Quaternion.identity, demo.transform);
            demoCamera.aspect = 4 / 3f;

            RenderTexture texture = new RenderTexture(2048, 1536, 0);
            demoCamera.targetTexture = texture;
            material.mainTexture = texture;

            rawImage.color = Color.white;
            rawImage.texture = demoCamera.targetTexture;
        }

        #endregion



        #region Public methods

        public void DestroyPinata()
        {
            if (demo != null)
            {
                demo.DestroyPinata();
            }
        }

        #endregion
    }
}
