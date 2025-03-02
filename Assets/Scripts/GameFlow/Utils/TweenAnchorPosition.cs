using System.Collections;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(RectTransform))]
    public class TweenAnchorPosition : MonoBehaviour
    {
        #region Variables

        public delegate void OnFinish();

        [Header("Blue")]
        [SerializeField]
        private Vector2 begin = Vector2.zero;
        [Header("Red")]
        [SerializeField]
        private Vector2 end = Vector2.zero;
        [SerializeField]
        AnimationCurve curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        [SerializeField]
        private float duration = 0f;
        [SerializeField]
        private bool showGizmos = false;
        [SerializeField]
        private bool ignoreTimeScale = false;

        private RectTransform rectTransform;
        private Vector2 anchoredPosition;
        private Coroutine move;

        #endregion


        #region Properties

        public float Duration 
        {
            get { return duration; }
            set { duration = value; }
        }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            anchoredPosition = rectTransform.anchoredPosition;
        }

        #endregion



        #region Public methods

        public void Play(OnFinish onFinish = null, bool forward = true)
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            if (move != null)
            {
                StopCoroutine(move);
            }

            onFinish = onFinish ?? delegate { };
            move = StartCoroutine(Move(onFinish, forward));
        }

        #endregion



        #region Private methods

        private IEnumerator Move(OnFinish onFinish, bool forward)
        {

            Vector2 savedBegin = anchoredPosition + (forward ? begin : end);
            Vector2 savedEnd =  anchoredPosition + (forward ? end : begin);

            float time = 0f;

            while (time < duration)
            {
                rectTransform.anchoredPosition = savedBegin + curve.Evaluate(time / duration) * (savedEnd - savedBegin);
                time += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

                yield return null;
            }

            rectTransform.anchoredPosition = savedEnd;
            move = null;
            onFinish();
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;

            RectTransform rect = GetComponent<RectTransform>();
            Rect tmpRect = rect.rect;

            float x0 = tmpRect.x + begin.x;
            float y0 = tmpRect.y + begin.y;
            float x1 = tmpRect.xMax + begin.x;
            float y1 = tmpRect.yMax + begin.y;

            Vector3[] anchors = new Vector3[4];

            anchors[0] = new Vector3(x0, y0, 0f);
            anchors[1] = new Vector3(x0, y1, 0f);
            anchors[2] = new Vector3(x1, y1, 0f);
            anchors[3] = new Vector3(x1, y0, 0f);

            Matrix4x4 mat = transform.localToWorldMatrix;
            for (int i = 0; i < 4; i++)
            {
                anchors[i] = mat.MultiplyPoint(anchors[i]);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(anchors[0], anchors[1]);
            Gizmos.DrawLine(anchors[1], anchors[2]);
            Gizmos.DrawLine(anchors[2], anchors[3]);
            Gizmos.DrawLine(anchors[3], anchors[0]);

            x0 = tmpRect.x + end.x;
            y0 = tmpRect.y + end.y;
            x1 = tmpRect.xMax + end.x;
            y1 = tmpRect.yMax + end.y;

            anchors[0] = new Vector3(x0, y0, 0f);
            anchors[1] = new Vector3(x0, y1, 0f);
            anchors[2] = new Vector3(x1, y1, 0f);
            anchors[3] = new Vector3(x1, y0, 0f);

            for (int i = 0; i < 4; i++)
            {
                anchors[i] = mat.MultiplyPoint(anchors[i]);
            }

            Gizmos.color = Color.red;
            Gizmos.DrawLine(anchors[0], anchors[1]);
            Gizmos.DrawLine(anchors[1], anchors[2]);
            Gizmos.DrawLine(anchors[2], anchors[3]);
            Gizmos.DrawLine(anchors[3], anchors[0]);
        }
    }
}
