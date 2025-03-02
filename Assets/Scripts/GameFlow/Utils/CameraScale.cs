using UnityEngine;


namespace PinataMasters
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Camera))]
    public class CameraScale : MonoBehaviour
    {

        #region Types

        private enum Anchor
        {
            Center = 0,
            Top = 1,
            Bottom = 2
        }

        #endregion


        #region Variables;

        [SerializeField]
        float width = 5f;

        [SerializeField]
        Anchor anchor = Anchor.Center;

        private Camera gameCamera;
        private float currentAspect;

        #endregion


        #region Properties

        private Camera GameCamera
        {
            get
            {
                if (gameCamera == null)
                {
                    gameCamera = GetComponent<Camera>();
                }
                return gameCamera;
            }
        }

        #endregion


        #region Unity lifecycle

        private void Awake()
        {
            ChangeCameraSize();
        }


        #if UNITY_EDITOR
        private void Update()
        {
            if (!Mathf.Approximately(currentAspect, GameCamera.aspect))
            {
                ChangeCameraSize();
            }

            currentAspect = GameCamera.aspect;
        }
        #endif

        #endregion


        #region Private methods

        private void ChangeCameraSize()
        {
            float cameraSize = GameCamera.orthographicSize;
            GameCamera.orthographicSize = width / GameCamera.aspect;

            switch (anchor)
            {
                case Anchor.Top:
                    GameCamera.transform.position = Vector3.up * (cameraSize - GameCamera.orthographicSize);
                    break;

                case Anchor.Bottom:
                    GameCamera.transform.position = Vector3.down * (cameraSize - GameCamera.orthographicSize);
                    break;

                case Anchor.Center:
                    GameCamera.transform.position = Vector3.zero;
                    break;
            }
        }

        #endregion

    }
}
