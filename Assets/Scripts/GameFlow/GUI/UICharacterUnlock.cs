using System;
using System.Collections;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


namespace PinataMasters
{
    public class UICharacterUnlock : UIUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UICharacterUnlock> Prefab = new ResourceGameObject<UICharacterUnlock>("Game/GUI/DialogCharacterUnlock");

        private const int ANIM_INDEX = 0;

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Button closeButton = null;
        [SerializeField]
        private float timeForSkipAvailable = 0f;
        [SerializeField]
        private ParticleSystem appearEffect = null;
        [SerializeField]
        private ParticleSystem idleEffect = null;

        [Header("Character")]
        [SerializeField][SpineAnimation(dataField: "body")]
        private string appearAnim = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string idleAnim = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string disappearAnim = null;

        [SerializeField]
        private SkeletonGraphic body = null;
        [SerializeField]
        private SkeletonGraphic legs = null;

        [SerializeField]
        private float scaleDuration = 0f;
        [SerializeField]
        private AudioClip sound = null;
        [SerializeField]
        private float delaySound = 0f;

        private Vector3 baseIdleEffectScale = Vector3.zero;

        #endregion



        #region Public methods

        protected override void Awake()
        {
            base.Awake();

            closeButton.onClick.AddListener(PlayCloseAnim);
            baseIdleEffectScale = idleEffect.transform.localScale;
        }


        public void Show(int skin, Action<UnitResult> onHided, Action onShowed)
        {
            base.Show(onHided, onShowed);

            tweenColor.Play(() => Showed());
            closeButton.gameObject.SetActive(false);

            body.skeletonDataAsset = Skins.GetBodySkeletonAsset(skin);
            body.Initialize(true);

            legs.skeletonDataAsset = Skins.GetLegsSkeletonAsset(skin);
            legs.Initialize(true);

            appearEffect.Play(true);
            idleEffect.Play(true);
            body.AnimationState.SetAnimation(ANIM_INDEX, appearAnim, false);
            legs.AnimationState.SetAnimation(ANIM_INDEX, appearAnim, false).Complete += PlayIdleAnim;

            idleEffect.transform.localScale = Vector3.zero;
            idleEffect.transform.DOScale(baseIdleEffectScale, scaleDuration);

            StartCoroutine(AllowToClose());

            AudioManager.Instance.Play(sound, AudioType.PrioritySound, delay: delaySound);
        }


        public void Hide(TrackEntry trackEntry)
        {
            base.Hide();

            idleEffect.Pause();
            trackEntry.Complete -= Hide;
            tweenColor.Play(() => Hided(), false);
        }


        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(PlayCloseAnim);
        }

        #endregion



        #region Private methods

        private void PlayIdleAnim(TrackEntry trackEntry)
        {
            trackEntry.Complete -= PlayIdleAnim;



            body.AnimationState.SetAnimation(ANIM_INDEX, idleAnim, true);
            legs.AnimationState.SetAnimation(ANIM_INDEX, idleAnim, true);
        }


        private void PlayCloseAnim()
        {
            body.AnimationState.SetAnimation(ANIM_INDEX, disappearAnim, false);
            legs.AnimationState.SetAnimation(ANIM_INDEX, disappearAnim, false).Complete += Hide;
        }


        private IEnumerator AllowToClose()
        {
            yield return new WaitForSeconds(timeForSkipAvailable);
            closeButton.gameObject.SetActive(true);
        }

        #endregion
    }
}
