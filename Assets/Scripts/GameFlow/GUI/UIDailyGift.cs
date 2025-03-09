using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIDailyGift : UIUnit<UnitResult>
    {
        #region Variables

        public static readonly ResourceGameObject<UIDailyGift> Prefab = new ResourceGameObject<UIDailyGift>("Game/GUI/DialogDailyGift");

        [Header("Content")]
        [SerializeField]
        private RectTransform pavelTop = null;
        [SerializeField]
        private Button buttonCollect = null;
        [SerializeField]
        private TextLocalizator todayDate = null;
        [SerializeField]
        private Text todayReward = null;
        [SerializeField]
        private Image bigImage = null;
        [SerializeField]
        private UIDailyGiftCell[] dayCells = null;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            pavelTop.anchorMax = new Vector2(pavelTop.anchorMax.x, pavelTop.anchorMax.y - SafeOffset.GetSafeTopRatio(GetComponent<RectTransform>().rect));

            buttonCollect.onClick.AddListener(Collect);
        }

        #endregion



        #region Public methods

        public override void Show(Action<UnitResult> onHided, Action onShowed = null)
        {
            base.Show(onHided, onShowed);
            int day = DailyGifts.DailyGiftDay;
    
            todayDate.SetParams((day + 1).ToString());
           
            todayReward.text = DailyGifts.GetCoins(day).ToShortFormat();
            bigImage.sprite = DailyGifts.GetConfigsByDay(day).BigCoins;
            bigImage.SetNativeSize();

            for (int i = 0; i < dayCells.Length; i++)
            {
                 dayCells[i].Init(i);
            }

            AdvertisingHelper.HideBanner();
            Showed();

            buttonCollect.enabled = true;
        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            AdvertisingHelper.ShowBanner();

            Hided();
        }

        #endregion



        #region Private methods

        private void Collect()
        {
            Player.AddCoins((uint)DailyGifts.GetCoins(DailyGifts.DailyGiftDay));
            DailyGifts.TakeGift();
            StartCoroutine(HideScreen());
            buttonCollect.enabled = false;
        }


        private IEnumerator HideScreen()
        {
            yield return new WaitForSeconds(0.7f);
            
            Hide();
        }

        #endregion
    }
}
