using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class WeaponPanel : MenuPlayerPanel
    {
        #region Variables

        [SerializeField]
        private RectTransform contentAnchor = null;
        [SerializeField]
        private ViewGun prefabViewGun = null;
        [SerializeField]
        private RectTransform prefabLockGun = null;
        [SerializeField]
        private uint lockGunCount = 1;
        [SerializeField]
        private RectTransform separator = null;

        private List<ViewGun> viewGuns = new List<ViewGun>();
        private float viewGunHeight;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            UIPlayerMenu.OnPlayerMenuShow += OnPlayerMenuShow;
            Player.OnResetProgress += ResetFocusWeapon;

            viewGunHeight = prefabViewGun.GetComponent<RectTransform>().rect.height;

            for (int i = 0; i < Arsenal.Count; i++)
            {
                ViewGun view = Instantiate(prefabViewGun, contentAnchor);
                view.Init(i);
                viewGuns.Add(view);

                if (i < Arsenal.Count - 1)
                {
                    Instantiate(separator, contentAnchor);
                }
            }

            for (int i = 0; i < lockGunCount; i++)
            {
                Instantiate(separator, contentAnchor);
                Instantiate(prefabLockGun, contentAnchor);
            }
        }


        private void OnDestroy()
        {
            UIPlayerMenu.OnPlayerMenuShow -= OnPlayerMenuShow;
            Player.OnResetProgress -= ResetFocusWeapon;
        }

        #endregion



        #region Public methods

        public override bool NeedShowAlert()
        {
            return viewGuns.Exists(v => v.NeedAlert());
        }


        public void SetBuyTutorialState()
        {
            viewGuns[1].SetBuyTutorialState(GetComponent<ScrollRect>());
        }


        public void SetUpgradeTutorialState()
        {
            viewGuns[0].SetUpgradeTutorialState(GetComponent<ScrollRect>());
        }

        #endregion



        #region Private methods

        private void OnPlayerMenuShow()
        {
            contentAnchor.anchoredPosition = new Vector2(contentAnchor.anchoredPosition.x, viewGunHeight * Player.CurrentWeapon);
        }


        private void ResetFocusWeapon()
        {
            contentAnchor.anchoredPosition = new Vector2(contentAnchor.anchoredPosition.x, 0f);
        }

        #endregion
    }
}