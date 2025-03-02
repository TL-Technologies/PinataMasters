using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace PinataMasters
{
    public class PrestigePanel : MonoBehaviour
    {
        #region Variables

        private const string LEVEL = "LVL {0}";
        private const string PLUS = "+ {0}";

        private const string DESC_COLOR_BEGIN = "<color=#ffffffff>";
        private const string DESC_COLOR_END = "</color>";

        private const string DESC_LOCK_KEY_FIRST = "ui.prestige.lock";
        private const string DESC_LOCK_KEY_SECOND = "ui.prestige.lock2";

        private const string DESC_UNLOCK_KEY_FIRST = "ui.prestige.resetprogress";
        private const string DESC_UNLOCK_KEY_SECOND = "ui.prestige.earngems";

        [Header("Content")]
        [SerializeField]
        private RectTransform available = null;
        [SerializeField]
        private RectTransform unAvailable = null;
        [SerializeField]
        private Text lockDescription = null;
        [SerializeField]
        private Text unlockDescription = null;
        [SerializeField]
        private Button prestigeButton = null;
        [SerializeField]
        private Text gemsText = null;
        [SerializeField]
        private Text lvlText = null;

        [Header("Tutorial")]
        [SerializeField]
        private Effect lockEffectPrefab = null;
        [SerializeField]
        private Effect shineEffectPrefab = null;
        [SerializeField]
        private Transform effectAnchor = null;
        [SerializeField]
        private Transform lockToMove = null;
        [SerializeField]
        private float lockMoveDuration = 0f;
        [SerializeField]
        private float lockMoveDelay = 0f;
        [SerializeField]
        private Vector3 endLockScale = Vector3.one;
        [SerializeField]
        private AudioClip soundUnlock = null;
        [SerializeField]
        private float delaySoundUnlock = 0f;

        [Header("TutorialShake")]
        [SerializeField]
        [Range(0f, float.MaxValue)]
        private float magnitudeShake = 0f;
        [SerializeField]
        private AnimationCurve curveShake = null;

        private Vector3 savedlockPosition;
        private float tutorialAnimationDuration;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            prestigeButton.onClick.AddListener(ShowPopup);

            Refresh();
            Player.OnResetProgress += Refresh;
            Player.OnLevelUp += Refresh;
            Localisation.OnLanguageChanged += Refresh;
        }


        private void OnDestroy()
        {
            Player.OnResetProgress -= Refresh;
            Player.OnLevelUp -= Refresh;
            Localisation.OnLanguageChanged += Refresh;
        }

        #endregion



        #region Public methods

        public void SetTutorialState()
        {
            TutorialManager.Instance.SetFade(true);

            tutorialAnimationDuration = lockMoveDelay + lockMoveDuration + 1.7f;
            Shake.Play(Camera.main.transform, tutorialAnimationDuration, magnitudeShake, curveShake);

            Canvas lockCanvas = lockToMove.gameObject.AddComponent<Canvas>();
            lockCanvas.overrideSorting = true;
            lockCanvas.sortingLayerName = "UI";
            lockCanvas.sortingOrder = 4;

            savedlockPosition = lockToMove.position;
            lockToMove.DOScale(endLockScale, lockMoveDelay);

            lockToMove.DOMove(effectAnchor.position, lockMoveDuration).
                            SetDelay(lockMoveDelay).SetEase(Ease.OutSine).
                            OnComplete(() => 
                            { 
                                lockToMove.gameObject.SetActive(false);
                                EffectPlayer.Play(lockEffectPrefab, effectAnchor.position);
                                AudioManager.Instance.Play(soundUnlock, AudioType.Sound, delay: delaySoundUnlock);
                            });

            StartCoroutine(SetNormalState());
        }

        #endregion



        #region Private methods

        private void ShowPopup()
        {
            GameAnalytics.ResetLevelClick();
            UIPrestige.Prefab.Instance.Show();
        }


        private void Refresh()
        {
            if (PlayerConfig.IsResetAllow() && !TutorialManager.Instance.IsPrestigeTutorialCanStart)
            {
                available.gameObject.SetActive(true);
                unAvailable.gameObject.SetActive(false);

                lvlText.text = string.Format(LEVEL, Player.Level + 1);
                gemsText.text = string.Format(PLUS, PlayerConfig.GetPrestigeReward().ToShortFormat());

                unlockDescription.text = DESC_COLOR_BEGIN + string.Format(Localisation.LocalizedStringOrSource(DESC_UNLOCK_KEY_FIRST), PlayerConfig.GetMinLevelForReset() + 1)
                                         + DESC_COLOR_END + Localisation.LocalizedStringOrSource(DESC_UNLOCK_KEY_SECOND);
            }
            else
            {
                available.gameObject.SetActive(false);
                unAvailable.gameObject.SetActive(true);

                gemsText.text = string.Format(PLUS, PlayerConfig.GetMinPrestigeReward().ToShortFormat());

                lockDescription.text = DESC_COLOR_BEGIN + string.Format(Localisation.LocalizedStringOrSource(DESC_LOCK_KEY_FIRST), PlayerConfig.GetMinLevelForReset() + 1)
                                       + DESC_COLOR_END + Localisation.LocalizedStringOrSource(DESC_LOCK_KEY_SECOND);

            }
        }


        private IEnumerator SetNormalState()
        {
            yield return new WaitForSeconds(tutorialAnimationDuration);

            Destroy(lockToMove.gameObject.GetComponent<Canvas>());

            TutorialManager.Instance.SetFade(false);
            TutorialManager.Instance.IsPrestigeTutorialPassed = true;
            lockToMove.localScale = Vector3.one;
            lockToMove.position = savedlockPosition;
            lockToMove.gameObject.SetActive(true);

            EffectPlayer.Play(shineEffectPrefab, transform.position);

            Refresh();
        }

        #endregion
    }
}
