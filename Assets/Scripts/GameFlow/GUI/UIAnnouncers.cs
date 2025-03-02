using Modules.General.Obsolete;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Spine.Unity;


namespace PinataMasters
{
    public class UIAnnouncers : MonoBehaviour
    {
        #region Types

        [Serializable]
        private struct PerfectAnnouncer
        {
            public List<Sprite> Sprites;
            public AnimationCurve ApearScaleCurve;
            public AnimationCurve DisapearScaleCurve;
            public float ScaleDuration;
            public float LifeTime;
            public float Delay;
            public float Angle;
            public float MoveTime;
            public AudioClip Sound;
            public float DelaySound;
            public float Volume;
        }

        #endregion



        #region Variables

        public static readonly ResourceGameObject<UIAnnouncers> Prefab = new ResourceGameObject<UIAnnouncers>("Game/GUI/PanelAnnouncers");

        [Header("PerfectAnnouncerPrefab")]
        [SerializeField]
        private Image perfectAnnounerPrefab = null;
        [SerializeField]
        private Transform perfectAnnouncerParent = null;

        [Header("HitAnnouncer")]
        [SerializeField]
        private PerfectAnnouncer hitAnnouncer = new PerfectAnnouncer();
        [Space]
        [SerializeField]
        private float hitAnnouncerCooldown = 0f;
        [SerializeField]
        private Vector3 centerPosition = Vector3.zero;
        [SerializeField]
        private Vector3 rightPosition = Vector3.zero;
        [SerializeField]
        private Vector3 leftPosition = Vector3.zero;

        [Header("CritAnnouncer")]
        [SerializeField]
        private PerfectAnnouncer critAnnouncer = new PerfectAnnouncer();
        [Space]
        [SerializeField]
        private bool isAlphaDisapearCritAnnouncer = true;
        [SerializeField]
        private float alphaDisapearCritAnnouncerTime = 1f;
        [SerializeField]
        private AnimationCurve alphaDisapearCritAnnouncerCurve = null;
        [SerializeField]
        private RectTransform leftCritTarget = null;
        [SerializeField]
        private RectTransform rightCritTarget = null;
        [SerializeField]
        private float maxRandomCritShiftY = 0f;

        [Header("FinishAnnouncer")]
        [SerializeField]
        private PerfectAnnouncer finishAnnouncer;
        [Space]
        [SerializeField]
        private float finishAnnouncerCooldown = 0f;
        [SerializeField]
        private bool isFinishAnnouncersFromShooter = true;
        [SerializeField]
        private List<RectTransform> finishAnnouncersStartPositions = null;
        [SerializeField]
        private List<RectTransform> finishAnnouncersEndPositions = null;
        [SerializeField]
        private List<AudioClip> finishAnnouncersSounds = null;
        [SerializeField]
        private List<float> delayFinishAnnouncersSounds = null;

        [Header("CompletedAnnouncers")]
        [SerializeField]
        private SkeletonGraphic completedAnnouncer;
        [SerializeField]
        [SpineAnimation]
        private string COMPLETE = null;
        [SerializeField]
        private float completeAnnouncerDelay = 1f;

        [Header("StageAnnouncers")]
        [SerializeField]
        private Image bossImage = null;
        [SerializeField]
        private Image stageImage = null;
        [SerializeField]
        private Image numImageFirst = null;
        [SerializeField]
        private Image numImageSecond = null;
        [SerializeField]
        private Image numImageThird = null;
        [SerializeField]
        private List<Sprite> numSprites = null;


        [SerializeField]
        private RectTransform boss = null;
        [SerializeField]
        private bool isBossScaleAppear = true;
        [ConditionalHide("isBossScaleAppear")]
        [SerializeField]
        private AnimationCurve bossAppearScaleCurve = null;
        [ConditionalHide("isBossScaleAppear")]
        [SerializeField]
        private float bossAppearDuration = 1f;


        [SerializeField]
        private AnimationCurve bossPositionCurve = null;
        [SerializeField]
        private AnimationCurve bossScaleCurve = null;
        [SerializeField]
        private AudioClip soundBoss = null;
        [SerializeField]
        private float delaySoundBoss = 0f;


        [SerializeField]
        private RectTransform stage = null;
        [SerializeField]
        private AnimationCurve stagePositionCurve = null;
        [SerializeField]
        private AnimationCurve stageScaleCurve = null;

        [SerializeField]
        private RectTransform nums = null;
        [SerializeField]
        private AnimationCurve numPositionCurve = null;
        [SerializeField]
        private AnimationCurve numScaleCurve = null;

        [SerializeField]
        private float tweensDuration = 1.9f;
        [SerializeField]
        private float startTweenPosition = -1500f;
        [SerializeField]
        private float finishTweenPosition = 1500f;

        private bool isHitCooldown;
        private float timer;

        private bool isRightPositionSet;
        private bool isLeftPositionSet;

        private Transform shooterTransform;
        private Canvas announcersCanvas;

        private Vector3 standartScale;

        private List<Sprite> unusedFinishSprites;

        private ObjectPool poolForObject;

        #endregion



        #region Properties

        private Canvas AnnouncersCanvas
        {
            get
            {
                announcersCanvas = announcersCanvas ?? GetComponent<Canvas>();
                return announcersCanvas;
            }
        }


        private Transform ShooterTransform
        {
            get
            {
                shooterTransform = shooterTransform ?? FindObjectOfType<Arena>().GetComponentInChildren<Shooter>().transform;
                return shooterTransform;
            }
        }


        public static bool ShouldShow { get; set; } = true;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            GetComponent<Canvas>().worldCamera = Camera.main;
            Pinata.NeedHitAnnouncer += ShowHitAnnouncer;
            Pinata.NeedCritAnnouncer += ShowCritAnnouncer;
            Pinata.NeedPinataDestroyAnnouncer += PinataDestroyAnnouncer;
            SceneArena.OnPlay += OnArenaPlay;

            PoolManager poolManager = PoolManager.Instance;
            poolForObject = poolManager.PoolForObject(perfectAnnounerPrefab.gameObject);
        }


        private void OnDestroy()
        {
            Pinata.NeedHitAnnouncer -= ShowHitAnnouncer;
            Pinata.NeedCritAnnouncer -= ShowCritAnnouncer;
            Pinata.NeedPinataDestroyAnnouncer -= PinataDestroyAnnouncer;
            SceneArena.OnPlay -= OnArenaPlay;
        }


        private void Update()
        {
            if (isHitCooldown)
            {
                timer += Time.deltaTime;

                if (timer >= hitAnnouncerCooldown)
                {
                    isHitCooldown = false;
                    timer = 0;
                }
            }
        }

        #endregion



        #region Public methods

        public void Show()
        {
            gameObject.SetActive(true);
            isRightPositionSet = false;
            isLeftPositionSet = false;
        }


        #endregion



        #region Private methods

        private void OnArenaPlay()
        {
            ShowStageAnnouncer();
            unusedFinishSprites = new List<Sprite>(finishAnnouncer.Sprites);
        }


        private void ShowStageAnnouncer()
        {
            if (!ShouldShow)
            {
                return;
            }

            bool isBossStage = Player.IsBossLevel;

            bossImage.gameObject.SetActive(isBossStage);
            stageImage.gameObject.SetActive(!isBossStage);
            numImageFirst.gameObject.SetActive(!isBossStage);
            numImageSecond.gameObject.SetActive(!isBossStage);
            numImageThird.gameObject.SetActive(!isBossStage);

            if (!isBossStage)
            {
                if (Player.Level + 1 < 10)
                {
                    numImageFirst.sprite = numSprites[(int)Player.Level + 1];
                    numImageSecond.gameObject.SetActive(false);
                    numImageThird.gameObject.SetActive(false);
                }

                if (Player.Level + 1 >= 10 && Player.Level + 1 < 100)
                {
                    numImageFirst.sprite = numSprites[(int)((Player.Level + 1) / 10)];
                    numImageSecond.sprite = numSprites[(int)((Player.Level + 1) % 10)];
                    numImageThird.gameObject.SetActive(false);
                }

                if (Player.Level + 1 >= 100)
                {
                    numImageFirst.sprite = numSprites[(int)((Player.Level + 1) / 100)];
                    numImageSecond.sprite = numSprites[(int)((Player.Level + 1) % 100 / 10)];
                    numImageThird.sprite = numSprites[(int)((Player.Level + 1) % 10)];
                }

                stage.anchoredPosition = new Vector2(startTweenPosition, stage.anchoredPosition.y);
                stage.DOAnchorPosX(finishTweenPosition, tweensDuration).SetEase(stagePositionCurve);

                standartScale = stage.localScale;
                stage.localScale = Vector3.zero;
                stage.DOScale(standartScale, tweensDuration).SetEase(stageScaleCurve);

                nums.anchoredPosition = new Vector2(finishTweenPosition, nums.anchoredPosition.y);
                nums.DOAnchorPosX(startTweenPosition, tweensDuration).SetEase(numPositionCurve);

                standartScale = nums.localScale;
                nums.localScale = Vector3.zero;
                nums.DOScale(standartScale, tweensDuration).SetEase(numScaleCurve);
            }
            else
            {
                if (isBossScaleAppear)
                {
                    boss.anchoredPosition = new Vector2(0f, boss.anchoredPosition.y);
                    boss.localScale = Vector3.zero;
                    boss.DOScale(Vector3.one, bossAppearDuration).SetEase(bossAppearScaleCurve);
                }
                else
                {
                    boss.anchoredPosition = new Vector2(startTweenPosition, boss.anchoredPosition.y);
                    boss.DOAnchorPosX(finishTweenPosition, tweensDuration).SetEase(bossPositionCurve);

                    standartScale = boss.localScale;
                    boss.localScale = Vector3.zero;
                    boss.DOScale(standartScale, tweensDuration).SetEase(bossScaleCurve);
                }

                AudioManager.Instance.Play(soundBoss, AudioType.Sound, delay: delaySoundBoss);
            }

        }


        private void ShowHitAnnouncer(Vector3 pinataPosition)
        {
            if (ShouldShow)
            {
                if (!isHitCooldown)
                {
                    StartCoroutine(ShowHitAnnouncer(hitAnnouncer, pinataPosition));
                    isHitCooldown = true;
                }
            }

        }


        private void ShowCritAnnouncer(Vector3 pinataPosition)
        {
            if (!ShouldShow)
            {
                return;
            }

            StartCoroutine(ShowCritAnnouncer(critAnnouncer, pinataPosition));
        }


        private void PinataDestroyAnnouncer()
        {
            if (ShouldShow)
            {
                for (int i = 0; i < finishAnnouncersEndPositions.Count; i++)
                {
                    StartCoroutine(ShowFinishAnnouncer(finishAnnouncer,
                        finishAnnouncersStartPositions[i].anchoredPosition,
                        finishAnnouncersEndPositions[i].anchoredPosition,
                        finishAnnouncerCooldown * (i + 1),
                        finishAnnouncersSounds[i],
                        delayFinishAnnouncersSounds[i]));

                    StartCoroutine(PlayCompleted());
                }
            }
        }


        private IEnumerator PlayCompleted()
        {
            if (ShouldShow)
            {
                yield return new WaitForSeconds(completeAnnouncerDelay);
                completedAnnouncer.gameObject.SetActive(true);

                var tracker = completedAnnouncer.AnimationState.SetAnimation(0, COMPLETE, true);
                tracker.Complete += (_) => completedAnnouncer.gameObject.SetActive(false);
            }
            isHitCooldown = true;
        }


        private Vector3 GetAnnouncerPosition(Vector3 pinataPosition)
        {

            if (isRightPositionSet && isLeftPositionSet)
            {
                return centerPosition;
            }

            if (pinataPosition.x < 0f)
            {
                if (!isRightPositionSet)
                {
                    isRightPositionSet = true;
                    StartCoroutine(ResetSide(true));
                    return new Vector3(rightPosition.x, 0f, 0f);
                }
                else
                {
                    isLeftPositionSet = true;
                    StartCoroutine(ResetSide(false));
                    return new Vector3(leftPosition.x, 0f, 0f);
                }
            }

            if (pinataPosition.x > 0f)
            {
                if (!isLeftPositionSet)
                {
                    isLeftPositionSet = true;
                    StartCoroutine(ResetSide(false));
                    return new Vector3(leftPosition.x, 0f, 0f);
                }
                else
                {
                    isRightPositionSet = true;
                    StartCoroutine(ResetSide(true));
                    return new Vector3(rightPosition.x, 0f, 0f);
                }
            }

            return centerPosition;
        }
 

        private Vector3 WorldToUISpace(Canvas canvas, Vector3 worldPosition)
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition);
            Vector2 uiSpacePosition;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, screenPos, canvas.worldCamera, out uiSpacePosition);
            return uiSpacePosition;
        }


        private IEnumerator ShowHitAnnouncer(PerfectAnnouncer announcer, Vector3 pinataPosition)
        {
            yield return new WaitForSeconds(announcer.Delay);
            poolForObject.Pop((announcerHit) =>
            {
                announcerHit.transform.SetParent(perfectAnnouncerParent);
                Image announcerImage = announcerHit.GetComponent<Image>();

                announcerImage.sprite = announcer.Sprites[UnityEngine.Random.Range(0, announcer.Sprites.Count)];
                announcerImage.SetNativeSize();
                announcerImage.color = Color.white;

                announcerImage.rectTransform.anchoredPosition = WorldToUISpace(AnnouncersCanvas, ShooterTransform.position);
                announcerImage.rectTransform.DOAnchorPos(GetAnnouncerPosition(pinataPosition), announcer.MoveTime);

                announcerImage.rectTransform.localScale = Vector3.zero;
                announcerImage.rectTransform.DOScale(1f, announcer.ScaleDuration).SetEase(announcer.ApearScaleCurve);

                announcerImage.transform.eulerAngles = new Vector3(0f, 0f, announcer.Angle * (announcerImage.rectTransform.anchoredPosition.x < 0f ? -1f : 1f));

                Sheduler.PlayCoroutine(HideAnnouncer(announcer, announcerImage, announcer.LifeTime));
            });
        }


        private IEnumerator ShowCritAnnouncer(PerfectAnnouncer announcer, Vector3 pinataPosition)
        {
            yield return new WaitForSeconds(announcer.Delay);

            poolForObject.Pop((announcerCrit) =>
            {
                announcerCrit.transform.SetParent(perfectAnnouncerParent);
                Image announcerImage = announcerCrit.GetComponent<Image>();

                announcerImage.sprite = announcer.Sprites[UnityEngine.Random.Range(0, announcer.Sprites.Count)];
                announcerImage.SetNativeSize();
                announcerImage.color = Color.white;

                announcerImage.rectTransform.anchoredPosition = WorldToUISpace(AnnouncersCanvas, pinataPosition);
                Vector2 randomShift = new Vector2(0f, UnityEngine.Random.Range(-maxRandomCritShiftY, maxRandomCritShiftY));
                announcerImage.rectTransform.DOAnchorPos((pinataPosition.x > 0f ? leftCritTarget.anchoredPosition : rightCritTarget.anchoredPosition) + randomShift, announcer.MoveTime);

                announcerImage.rectTransform.localScale = Vector3.zero;
                announcerImage.rectTransform.DOScale(1f, announcer.ScaleDuration).SetEase(announcer.ApearScaleCurve);

                if (isAlphaDisapearCritAnnouncer)
                {
                    announcerImage.DOColor(new Color(1f, 1f, 1f, 0f), alphaDisapearCritAnnouncerTime).SetEase(alphaDisapearCritAnnouncerCurve);
                }

                announcerImage.transform.eulerAngles = new Vector3(0f, 0f, announcer.Angle * (announcerImage.rectTransform.anchoredPosition.x < 0f ? -1f : 1f));

                Sheduler.PlayCoroutine(HideAnnouncer(announcer, announcerImage, announcer.LifeTime));

                AudioManager.Instance.Play(announcer.Sound, AudioType.Sound, announcer.Volume, announcer.DelaySound);
            });
        }


        private IEnumerator ShowFinishAnnouncer(PerfectAnnouncer announcer, Vector3 startPosition, Vector3 finishPosition, float delay, AudioClip sound, float soundDelay)
        {
            yield return new WaitForSeconds(delay);

            poolForObject.Pop((announcerFinish) =>
            {
                announcerFinish.transform.SetParent(perfectAnnouncerParent);
                Image announcerImage = announcerFinish.GetComponent<Image>();

                int randomSpriteIndex = UnityEngine.Random.Range(0, unusedFinishSprites.Count);
                announcerImage.sprite = unusedFinishSprites[randomSpriteIndex];
                unusedFinishSprites.RemoveAt(randomSpriteIndex);

                announcerImage.SetNativeSize();
                announcerImage.color = Color.white;

                announcerImage.rectTransform.anchoredPosition = isFinishAnnouncersFromShooter ? WorldToUISpace(AnnouncersCanvas, ShooterTransform.position) : startPosition;
                announcerImage.rectTransform.DOAnchorPos(finishPosition, announcer.MoveTime);

                announcerImage.rectTransform.localScale = Vector3.zero;
                announcerImage.rectTransform.DOScale(1f, announcer.ScaleDuration).SetEase(announcer.ApearScaleCurve);

                announcerImage.transform.eulerAngles = new Vector3(0f, 0f, announcer.Angle * (announcerImage.rectTransform.anchoredPosition.x < 0f ? -1f : 1f));

                Sheduler.PlayCoroutine(HideAnnouncer(announcer, announcerImage, announcer.LifeTime));

                AudioManager.Instance.Play(sound, AudioType.PrioritySound, announcer.Volume, soundDelay);
            });
        }


        private IEnumerator HideAnnouncer(PerfectAnnouncer announcer, Image announcerImage, float lifeTime)
        {
            yield return new WaitForSeconds(lifeTime);

            announcerImage.rectTransform.localScale = Vector3.one;
            announcerImage.rectTransform.DOScale(0f, announcer.ScaleDuration).SetEase(announcer.DisapearScaleCurve).OnComplete(() => announcerImage.gameObject.ReturnToPool());
        }


        private IEnumerator ResetSide(bool isRightSide)
        {
            yield return new WaitForSeconds(hitAnnouncer.LifeTime);

            if (isRightSide)
            {
                isRightPositionSet = false;
            }
            else
            {
                isLeftPositionSet = false;
            }
        }

        #endregion
    }
}
