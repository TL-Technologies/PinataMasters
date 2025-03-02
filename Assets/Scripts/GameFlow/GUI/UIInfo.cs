using System;
using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class UIInfo : UITweensUnit<UnitResult>
    {
        public enum Type
        {
            NoInternet,
            NoAds,
            SomethingWrong,
            Restore,
        }

        #region Variables

        public static readonly ResourceGameObject<UIInfo> Prefab = new ResourceGameObject<UIInfo>("Game/GUI/DialogInfo");

        private const string OOPS_HEADER_KEY = "ui.dialog.noads.oops";
        private const string NO_INTERNET_DESC_KEY = "ui.dialog.nointernet";
        private const string NO_ADS_DESC_KEY = "ui.dialog.noads.adsproblem";
        private const string SOMETHING_WRONG_DESC_KEY = "ui.dialog.somethingwrong";
        private const string RESTORE_HEADER_KEY = "ui.dialog.restore";
        private const string RESTORE_DESC_KEY = "ui.dialog.restore.info";

        [SerializeField]
        private TweenImageColor tweenColor = null;

        [Header("Content")]
        [SerializeField]
        private Button okButton = null;

        [SerializeField]
        private TextLocalizator header = null;
        [SerializeField]
        private TextLocalizator description = null;

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();
            okButton.onClick.AddListener(() => Hide());
        }

        #endregion



        #region Public methods

        public void Show(Type type, Action<UnitResult> onHide = null)
        {
            Show(onHide);

            switch(type)
            {
                case Type.NoAds:
                    header.SetKey(OOPS_HEADER_KEY);
                    description.SetKey(NO_ADS_DESC_KEY);
                    break;
                case Type.NoInternet:
                    header.SetKey(OOPS_HEADER_KEY);
                    description.SetKey(NO_INTERNET_DESC_KEY);
                    break;
                case Type.SomethingWrong:
                    header.SetKey(OOPS_HEADER_KEY);
                    description.SetKey(SOMETHING_WRONG_DESC_KEY);
                    break;
                case Type.Restore:
                    header.SetKey(RESTORE_HEADER_KEY);
                    description.SetKey(RESTORE_DESC_KEY);
                    break;
                default:
                    throw new ArgumentException("Unexpected argument " + type);
            }

            tweenColor.Duration = durationShow;
            tweenColor.Play();

        }


        public override void Hide(UnitResult result = null)
        {
            base.Hide(result);

            tweenColor.Duration = durationHide;
            tweenColor.Play(forward: false);
        }

        #endregion
    }
}

