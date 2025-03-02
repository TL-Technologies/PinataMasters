using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PinataMasters
{
    [RequireComponent(typeof(Image))]
    public class TweenImageColor : MonoBehaviour
    {
        #region Variables

        public delegate void OnFinish();

        [SerializeField]
        private Color begin = Color.white;
        [SerializeField]
        private Color end = Color.black;
        [SerializeField]
        private float duration = 1f;
        [SerializeField]
        private bool ignoreTimeScale = false;

        private Image image;
        private Coroutine move;

        #endregion


        #region Properties

        public Color Begin
        {
            get
            {
                return begin;
            }

            set
            {
                begin = value;
            }
        }


        public Color End
        {
            get
            {
                return end;
            }

            set
            {
                end = value;
            }
        }


        public float Duration
        {
            get
            {
                return duration;
            }

            set
            {
                duration = value;
            }
        }

        #endregion


        #region Unity lifecycle

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        #endregion



        #region Public methods

        public void Play(OnFinish onFinish = null, bool forward = true)
        {
            if (move != null)
            {
                StopCoroutine(move);
            }

            onFinish = onFinish ?? delegate { };
            move = StartCoroutine(ChangeColor(onFinish, forward));
        }

        #endregion



        #region Private methods

        private IEnumerator ChangeColor(OnFinish onFinish, bool forward)
        {
            float time = 0f;

            while (true)
            {
                if (time < duration)
                {
                    time += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                    float delta = !forward ? 1 - time / duration : time / duration;

                    image.color = new Color(Mathf.Lerp(Begin.r, End.r, delta), Mathf.Lerp(Begin.g, End.g, delta),
                                      Mathf.Lerp(Begin.b, End.b, delta), Mathf.Lerp(Begin.a, End.a, delta));
                }
                else
                {
                    image.color = forward ? End : Begin;

                    move = null;
                    onFinish();
                    yield break;
                }

                yield return null;
            }
        }

        #endregion
    }
}