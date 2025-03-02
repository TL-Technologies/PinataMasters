using Modules.General.Obsolete;
using UnityEngine;

namespace PinataMasters
{
    public static class EffectPlayer
    {

        #region Variables

        static private ObjectPool poolForObject;
        static private PoolManager poolManager;

        #endregion


        #region Unity lifecycle

        static EffectPlayer()
        {
            poolManager = PoolManager.Instance;
        }

        #endregion


        #region Public methods

        // Костя что за фигня ???
        public static void Play(Effect effect, Vector3 position, Transform parent = null)
        {
            if (effect == null)
            {
                return;
            }

            poolForObject = poolManager.PoolForObject(effect.gameObject);

            poolForObject.Pop((playingEffect) =>
            {
                if (parent != null)
                {
                    playingEffect.transform.localRotation = parent.transform.rotation;
                }

                playingEffect.transform.parent = parent;
                playingEffect.GetComponent<Effect>().Play(position);
            });

        }

        #endregion

    }
}