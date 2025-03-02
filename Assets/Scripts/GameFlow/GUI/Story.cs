using DG.Tweening;
using Modules.General.HelperClasses;
using Spine.Unity;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class Story : Unit<UnitResult>
    {
        #region Variables

        const string WAS_STORY_SHOWED = "was_story_showed";

        public static readonly ResourceGameObject<Story> Prefab = new ResourceGameObject<Story>("Game/Game/Story");

        [Header("Animation")]
        [SerializeField] SkeletonAnimation shot;

        [SerializeField] Button skipButton;

        Tweener skipButtonTweener;

        #endregion



        #region Properties

        public static bool WasStoryShowed => CustomPlayerPrefs.GetBool(WAS_STORY_SHOWED); 

        #endregion



        #region Unity Lifecycle

        void OnEnable()
        {
            skipButton.onClick.AddListener(SkipButton_OnClick);
        }


        void OnDisable()
        {
            skipButton.onClick.RemoveListener(SkipButton_OnClick);
        }

        #endregion



        #region Public methods

        public void Show(Action<UnitResult> onHided)
        {
            base.Show(onHided);

            Showed();

            UIBlocker.Prefab.Instance.Show();

            shot.AnimationState.Complete += (_) => Hide();
        }

        #endregion



        #region Private methods

        void Hide()
        {
            CustomPlayerPrefs.SetBool(WAS_STORY_SHOWED, true);
            Destroy(gameObject);
            Hided();

            UIBlocker.Prefab.Instance.Hide();
        }

        #endregion



        #region Events Handler

        void OnMouseDown()
        {
            if (ABTest.InGameAbTestData.skipStory)
            {
                if (!skipButton.gameObject.activeSelf)
                {
                    skipButton.gameObject.SetActive(true);
                    skipButtonTweener = skipButton.gameObject.transform.DOScale(1.1f, 0.7f).SetLoops(-1, LoopType.Yoyo);
                }
            }
        }


        void SkipButton_OnClick()
        {
            skipButtonTweener.Kill();
            Hide();
        }

        #endregion
    }
}
