using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UILoadProgress : UIUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UILoadProgress> Prefab = new ResourceGameObject<UILoadProgress>("Game/GUI/DialogLoadProgress");

        [SerializeField]
        private float durationAppear = 0f;
        [SerializeField]
        private TweenImageColor tweenColor = null;
        [SerializeField]
        private Button agreeButton = null;
        [SerializeField]
        private Button disagreeButton = null;
        [SerializeField]
        private Transform body = null;

        private CloudProgress.Data data;
        private bool showSave;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            agreeButton.onClick.AddListener(LoadProgress);
            disagreeButton.onClick.AddListener(ShowSaveProgress);
        }

        #endregion



        #region Public methods

        public void Show(CloudProgress.Data data)
        {
            base.Show();

            this.data = data;
            showSave = false;
            tweenColor.Duration = durationAppear;
            body.localScale = new Vector3(0f, 0f, body.position.z);
            tweenColor.Play();
            body.DOScale(new Vector3(1f, 1f, body.position.z), durationAppear).OnComplete(Showed);
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tweenColor.Play(forward: false);
            body.DOScale(new Vector3(0f, 0f, body.position.z), durationAppear).OnComplete(() => Hided());
        }

        #endregion



        #region Private methods

        protected override void Hided(UnitResult result = null)
        {
            if (showSave)
            {
                UISaveProgress.Prefab.Instance.Show(data);
            }

            CloudProgress.IsLoginInProcess = false;

            base.Hided(result);
        }

        private void LoadProgress()
        {
            Hide();
            TutorialManager.Instance.UpdatePrefs(data.Tutorial);
            Player.UpdatePrefs(data.Player);
        }

        private void ShowSaveProgress()
        {
            showSave = true;
            Hide();
        }

        #endregion
    }
}
