using System;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(Collider))]
    public class TapZone : MonoBehaviour
    {
        #region Variables

        public static event Action OnTap = delegate { };
        public static event Action OnUp = delegate { };

        private static bool isLocked = false;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            TutorialManager.OnLockShooter += Lock;
        }


        private void OnDestroy()
        {
            TutorialManager.OnLockShooter -= Lock;
        }

        #endregion



        #region Public methods

        public static void Lock(bool value)
        {
            isLocked = value;
        }

        #endregion



        #region Private methods

        private void OnMouseDown()
        {
            if (!isLocked)
            {
                OnTap();
            }
        }


        private void OnMouseUp()
        {
            if (!isLocked)
            {
                OnUp();
            }
        }

        #endregion
    }
}
