using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class SkinsPanel : MenuPlayerPanel
    {
        #region Variables

        [SerializeField]
        private PrestigePanel prestigePanel = null;
        [SerializeField]
        private RectTransform contentAnchor = null;
        [SerializeField]
        private ViewSkin prefabView = null;
        [SerializeField]
        private RectTransform prefabLock = null;
        [SerializeField]
        private uint lockCount = 1;
        [SerializeField]
        private RectTransform separator = null;


        private List<ViewSkin> viewSkins = new List<ViewSkin>();

        #endregion



        #region Public methods

        public override void SetTutorialState()
        {
            contentAnchor.anchoredPosition = new Vector2(contentAnchor.anchoredPosition.x, 0f);
            prestigePanel.SetTutorialState();
        }


        public override bool NeedShowAlert()
        {
            return (PlayerConfig.IsResetAllow() || viewSkins.Exists(v => v.NeedAlert())) && !TutorialManager.Instance.IsPrestigeTutorialCanStart;
        }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            for (int i = 0; i < Skins.Count; i++)
            {
                ViewSkin view = Instantiate(prefabView, contentAnchor);
                view.Init(i);
                viewSkins.Add(view);

                if (i < Skins.Count - 1)
                {
                    Instantiate(separator, contentAnchor);
                }
            }

            for (int i = 0; i < lockCount; i++)
            {
                Instantiate(separator, contentAnchor);
                Instantiate(prefabLock, contentAnchor);
            }

            if (!TutorialManager.Instance.IsBuySkinTutorialPassed)
            {
                Player.OnResetProgress += SetSkinTutorialState;
            }
        }

        #endregion



        #region Private methods

        private void SetSkinTutorialState()
        {
            if (!TutorialManager.Instance.IsBuySkinTutorialPassed)
            {
                contentAnchor.anchoredPosition = new Vector2(contentAnchor.anchoredPosition.x, 0f);
                viewSkins[0].SetBuyTutorialState(GetComponent<ScrollRect>());
            }

            Player.OnResetProgress -= SetSkinTutorialState;
        }

        #endregion
    }
}
