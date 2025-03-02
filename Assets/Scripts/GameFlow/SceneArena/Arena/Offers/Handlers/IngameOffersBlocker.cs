using UnityEngine;
using System;


namespace PinataMasters
{
    public class IngameOffersBlocker : MonoBehaviour
    {
        #region Nested types

        [Flags]
        public enum BlockingType
        {
            None = 0,
            Touches = 1 << 1,
            Update = 1 << 2,
            All = Touches | Update
        }

        #endregion



        #region Fields

        [SerializeField] BlockingType blockingType;

        #endregion



        #region Unity lifecycle

        void OnEnable()
        {
            if (IngameOffersController.Prefab.Instance)
            {
                IngameOffersController.Prefab.Instance.Block(blockingType);
            }
        }


        void OnDisable()
        {
            if (IngameOffersController.Prefab.Instance)
            {
                IngameOffersController.Prefab.Instance.Unblock(blockingType);
            }
        }

        #endregion
    }
}
