using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIPrestige : UITweensUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UIPrestige> Prefab = new ResourceGameObject<UIPrestige>("Game/GUI/DialogPrestige");

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Button closeButton = null;
        [SerializeField]
        private Button resetButton = null;
        [SerializeField]
        private Text levelText = null;
        [SerializeField]
        private Text gemsText = null;
         
        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            closeButton.onClick.AddListener(Close);
            resetButton.onClick.AddListener(ResetProgress);
        }


        private void OnDestroy()
        {
            closeButton.onClick.RemoveListener(Close);
            resetButton.onClick.RemoveListener(ResetProgress);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided = null, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            tweenColor.Duration = durationShow;
            tweenColor.Play();

            levelText.text = (Player.Level + 1).ToString();
            gemsText.text = PlayerConfig.GetPrestigeReward().ToShortFormat();
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }

        #endregion



        #region Private methods

        private void Close()
        {
            GameAnalytics.ResetLevelSkip();
            Hide();
        }


        private void ResetProgress()
        {
            float gems = PlayerConfig.GetPrestigeReward();
            uint lvl = Player.Level;

            Player.AddGems(gems);
            Player.ResetProgress();
            ShooterShadowsConfig.SetShadowsInfo();
            Player.ResetWeapons();

            Hide();
            GameAnalytics.ResetLevelDone(lvl + 1, gems);
        }

        #endregion

    }
}
