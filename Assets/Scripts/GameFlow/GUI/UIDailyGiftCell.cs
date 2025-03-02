using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIDailyGiftCell : MonoBehaviour
    {
        #region Variables

        private const string CLAIMED_KEY = "ui.dialog.claimed";
        private const string TODAY_KEY = "ui.dialog.today";
        private const string FUTURE_KEY = "ui.dialog.daylower";

        private const int sizeFontClaimped = 36;
        private const int sizeFontToday = 40;
        private const int sizeFontFuture = 36;

        private static readonly Color colorClaimped = new Color(209, 176, 255);
        private static readonly Color colorToday = Color.white;
        private static readonly Color colorFuture = Color.white;


        [SerializeField]
        private Image back = null;
        [SerializeField]
        private Image coins = null;
        [SerializeField]
        private Text info = null;

        [SerializeField]
        private GameObject label = null;

        [SerializeField]
        private Text labelText = null;

        #endregion



        #region Public methods

        public void Init(int day)
        {
            DailyGiftConfig type = DailyGifts.GetConfigsByDay(day);

            TextLocalizator textLocalizator = info.GetComponent<TextLocalizator>();

            if (day < DailyGifts.DailyGiftDay)
            {
                back.sprite = type.BackSimple;
                coins.sprite = type.CoinsClaimed;

                textLocalizator.SetKey(CLAIMED_KEY);
                info.color = colorClaimped;
                info.fontSize = sizeFontClaimped;
                label.SetActive(false);
            }
            else if (day == DailyGifts.DailyGiftDay)
            {
                back.sprite = type.BackToday;
                coins.sprite = type.CoinsToday;

                textLocalizator.SetKey(TODAY_KEY);

                info.color = colorToday;
                info.fontSize = sizeFontToday;
                label.SetActive(true);
                labelText.text = DailyGifts.GetCoins(day).ToShortFormat();
            }
            else
            {
                back.sprite = type.BackSimple;
                coins.sprite = type.CoinsFuture;

                textLocalizator.SetKey(FUTURE_KEY);
                textLocalizator.SetParams((day + 1).ToString());
                info.color = colorFuture;
                info.fontSize = sizeFontFuture;
                label.SetActive(true);
                labelText.text = DailyGifts.GetCoins(day).ToShortFormat();
            }
        }

        #endregion
    }
}
