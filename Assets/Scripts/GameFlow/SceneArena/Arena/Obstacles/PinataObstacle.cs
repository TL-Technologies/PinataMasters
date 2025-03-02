using UnityEngine;


namespace PinataMasters
{
    public class PinataObstacle : Obstacle
    {
        #region Variables

        [SerializeField][Range(0f, 360f)]
        private float startPosAngle = 0f;
        [SerializeField]
        private float circleRadius = 0f;
        [SerializeField]
        private float rotateSpeed = 0f;
        [SerializeField]
        private TrailRenderer trai = null;

        private float angle;
        private Transform target;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            angle = startPosAngle * Mathf.Deg2Rad;
            Health = SelectorLevels.GetLevels.GetHealthPinataObstacle(Player.Level);
        }


        private void Update()
        {
            angle += rotateSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0f) * circleRadius;

            transform.localPosition = target.position + offset;
            transform.rotation = Quaternion.Euler(0f, 0f, - angle * Mathf.Rad2Deg);
        }

        #endregion



        #region Public methods

        public void Init(Transform pinataTransform)
        {
            target = pinataTransform;
        }

        #endregion



        #region Private methods

        protected override void TweenScaleCallback()
        {
            gameObject.AddComponent<PolygonCollider2D>();
            trai.enabled = true;
        }

        #endregion



        #region Editor methods

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;

            Gizmos.DrawWireSphere(transform.position - transform.localPosition, circleRadius);
        }

        #endregion
    }
}