using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UISaveProgress : UIUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UISaveProgress> Prefab = new ResourceGameObject<UISaveProgress>("Game/GUI/DialogSaveProgress");

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

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();
            agreeButton.onClick.AddListener(SaveProgress);
            disagreeButton.onClick.AddListener(DisableSynchronize);
        }

        #endregion



        #region Public methods

        public void Show(CloudProgress.Data data)
        {
            base.Show();

            this.data = data;
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

        private void SaveProgress()
        {
            CloudProgress.Save();
            Hide();
        }

        private void DisableSynchronize()
        {
            CloudProgress.IsSynchronizeEnabled = false;
            UISettings.Prefab.Instance.RefreshText();
            Hide();
        }

        #endregion
    }
}
