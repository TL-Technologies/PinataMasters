using Modules.General.Obsolete;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PinataMasters
{
    public class AnnouncerPlayer : MonoBehaviour
    {
        #region Variables

        [Header("Parameters")]
        [SerializeField]
        private float lifeTime = 1f;
        [SerializeField]
        private uint count = 1;
        [SerializeField]
        private float delayBetween = 0f;

        [Header("Sprites")]
        [SerializeField]
        private List<Sprite> spriteList = null;
        [SerializeField]
        private bool isRandomSprites = true;

        [Header("Move")]
        [SerializeField]
        private List<Vector2> startPositions = new List<Vector2>();
        [SerializeField]
        private bool isRandomStartPosition;
        [SerializeField]
        private List<Vector2> finishPositions = new List<Vector2>();
        [SerializeField]
        private bool isRandomFinishPosition;

        [Space]
        [SerializeField]
        private AnimationCurve moveCurve = null;
        [SerializeField]
        private AnimationCurve scaleCurve = null;
        [SerializeField]
        private AnimationCurve alphaCurve = null;


        private GameObject announcerPrefab;
        private ObjectPool poolForObject;

        private Vector3 startPosition;
        private Vector3 finishPosition;

        private Sprite sprite;
        private List<Sprite> unUsedSprites;

        #endregion


#if UNITY_EDITOR
        public List<Vector2> StartPositions { get { return startPositions; } }
        public List<Vector2> FinishPositions { get { return finishPositions; } }
#endif


        #region Public methods

        public void Play(float delay = 0f, Transform fromObject = null, Transform toObject = null)
        {
            if (announcerPrefab == null)
            {
                announcerPrefab = new GameObject("Announcer");
                announcerPrefab.AddComponent<Announcer>();

                PoolManager poolManager = PoolManager.Instance;
                poolForObject = poolManager.PoolForObject(announcerPrefab);
            }

            Sheduler.PlayCoroutine(Show(delay, fromObject, toObject));
        }

        #endregion



        #region Private methods

        private IEnumerator Show(float delay = 0f, Transform fromObject = null, Transform toObject = null)
        {
            yield return new WaitForSeconds(delay);

            if (isRandomSprites)
            {
                unUsedSprites = new List<Sprite>(spriteList);
            }

            for (int i = 0; i < count; i++)
            {
                poolForObject.Pop((announcer) =>
                {
                    if (fromObject != null)
                    {
                        startPosition = fromObject.position;
                    }
                    else
                    {
                        startPosition = isRandomStartPosition ? startPositions.RandomObject() : startPositions[i % startPositions.Count];
                    }

                    if (toObject != null)
                    {
                        finishPosition = toObject.position;
                    }
                    else
                    {
                        finishPosition = isRandomFinishPosition ? finishPositions.RandomObject() : finishPositions[i % finishPositions.Count];
                    }

                    if (isRandomSprites)    
                    {
                        if (unUsedSprites.Count == 0)
                        {
                            unUsedSprites = new List<Sprite>(spriteList);
                        }

                        sprite = unUsedSprites.RandomObject();
                        unUsedSprites.Remove(sprite);
                    }
                    else
                    {
                        sprite = spriteList[i % spriteList.Count];
                    }

                    announcer.GetComponent<Announcer>().Init(sprite, startPosition, finishPosition, moveCurve, scaleCurve, alphaCurve, lifeTime);
                });

                yield return new WaitForSeconds(delayBetween);
            }
        }

        #endregion



        #region Editor methods

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < startPositions.Count; i++)
            {
                Gizmos.DrawSphere(startPositions[i], 0.5f);

                if (!isRandomStartPosition && !isRandomFinishPosition && i < finishPositions.Count)
                {
                    Gizmos.DrawLine(startPositions[i], finishPositions[i]);
                }
            }

            Gizmos.color = Color.magenta;
            for (int i = 0; i < finishPositions.Count; i++)
            {
                Gizmos.DrawSphere(finishPositions[i], 0.5f);
            }
        }

        #endregion
    }
}
