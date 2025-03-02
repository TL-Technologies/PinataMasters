using UnityEngine;
using DG.Tweening;


namespace PinataMasters
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SpriteRenderer), typeof(BoxCollider2D))]
    public class ArenaObstacle : Obstacle
    {
        #region Variables

        private const float height = 0.64f;
        private const float RANDOM_OFFSET = 0.2f;

        [SerializeField]
        private bool canMove = false;

        [SerializeField]
        private Transform end = null;

        [Header("Parameters")]
        [SerializeField]
        private float width = 0;
        [SerializeField]
        private float moveDuration = 0f;
        [SerializeField]
        private float delayStart = 0f;
        [SerializeField]
        private float delayFinish = 0f;
        [SerializeField]
        private bool isSlowerSpeedShooter = false;

        [SerializeField]
        private bool canStop = false;
        [SerializeField]
        private float stopDuration = 0f;

        private SpriteRenderer spriteRenderer;
        private BoxCollider2D boxCollider2D;

        private Vector3 startPosition;
        private Vector3 endPosition;

        private Tweener tweener;
        private Sequence sequence;

        private bool forward = true;
        private float delay;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            Health = SelectorLevels.GetLevels.GetHealthArenaObstacle(Player.Level);
        }

        protected override void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.size = new Vector2(width, height);

            boxCollider2D = GetComponent<BoxCollider2D>();
            boxCollider2D.size = new Vector2(width, height);


            if (canMove)
            {
                startPosition = transform.position;
                endPosition = end.position;
                moveDuration = CalcDuration();
                Move();
            }

            base.Start();
        }


#if UNITY_EDITOR
        private void Update()
        {
            spriteRenderer.size = new Vector2(width, height);
            boxCollider2D.size = new Vector2(width, height);
        }
#endif

        private void OnDestroy()
        {
            sequence.Kill();
        }

        #endregion



        #region Private methods

        private float CalcDuration()
        {
            if (!isSlowerSpeedShooter)
            {
                return moveDuration;
            }

            float period = (delayStart + moveDuration + delayFinish + moveDuration);
            float periodShooter = FindObjectOfType<ShooterLegs>().Period;

            float offset = period * SelectorLevels.GetLevels.FactorAround;
            bool slower = period - offset < periodShooter && periodShooter < period + offset;

            return moveDuration / (slower ? (1f - SelectorLevels.GetLevels.FactorSlower) : 1f);
        }


        private void Move()
        {
            Vector3 beginning = forward ? startPosition : endPosition;
            Vector3 destination = forward ? endPosition : startPosition;

            delay = forward ? delayStart : delayFinish;

            float stopPart = 0f;
            sequence = DOTween.Sequence();
            Ease curveType = canStop ? Ease.InOutCubic : Ease.Linear;

            if (canStop)
            {
                stopPart = Random.Range(RANDOM_OFFSET, 1f - RANDOM_OFFSET);
                float randomX = Mathf.Lerp(beginning.x, destination.x, stopPart);
                float randomY = Mathf.Lerp(beginning.y, destination.y, stopPart);
                Vector3 stopPoint = new Vector3(randomX, randomY, transform.position.z);

                sequence.Append(transform.DOMove(stopPoint, moveDuration * stopPart).SetEase(curveType)).AppendInterval(stopDuration);
            }

            sequence.Append(transform.DOMove(destination, moveDuration * (1f - stopPart)).SetEase(curveType)).AppendCallback(() => Move()).SetDelay(delay);
            forward = !forward;
        }

        #endregion



        #region Editor methods

        private void OnDrawGizmos()
        {
            if (canMove)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(new Vector2(transform.position.x, transform.position.y), new Vector2(end.position.x, end.position.y));
                Gizmos.DrawCube(end.transform.position, new Vector2(width, height));
            }
        }

        #endregion
    }
}
