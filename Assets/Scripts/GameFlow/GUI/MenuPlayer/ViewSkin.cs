using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;


namespace PinataMasters
{
    public class ViewSkin : MonoBehaviour
    {
        #region Variables

        public static event Action<float, string> OnNeedShowMiniBank = delegate { };
        public static event Action<int> OnSkinBought = delegate { };
        public static event Action<int> OnSkinUpgrade = delegate { };

        private const string SKILL_FORMATED_COLOR_BEGIN = "<color=#52e900ff>";
        private const string SKILL_FORMATED_COLOR_END = "</color>";
        private const string SKILL_LVL = "lvl {0}";

        [SerializeField]
        private Image skinImage = null;
        [SerializeField]
        private Text skinName = null;
        [SerializeField]
        private Text skinDesc = null;
        [SerializeField]
        private Image select = null;
        [SerializeField]
        private Button skinButton = null;
        [SerializeField]
        private TextMeshProUGUI skinLvl = null;

        [Header("LockMaterial")]
        [SerializeField]
        private Material grayscaleMaterial = null;
        [SerializeField]
        private Color grayscaleColor = Color.white;

        [Header("Buy")]
        [SerializeField]
        private Button chooseButton = null;
        [SerializeField]
        private Button buyButton = null;
        [SerializeField]
        private Text buyText = null;

        [Header("Upgrade")]
        [SerializeField]
        private Button upgradeButton = null;
        [SerializeField]
        private Text upgradePrice = null;

        [Header("Effects")]
        [SerializeField]
        private Effect upgradeEffectPrefab = null;
        [SerializeField]
        private RectTransform buyEffectAnchor = null;

        [Header("Tutorial")]
        [SerializeField]
        private RectTransform buyTutorialHand = null;

        [Header("Sounds")]
        [SerializeField]
        private AudioClip upgradeClip = null;
        [SerializeField]
        private AudioClip buyClip = null;
        [SerializeField]
        private float buyClipDelay = 0f;
        [SerializeField]
        private AudioClip unactiveClip = null;

        private int skin;
        private ScrollRect scrollRect;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            buyButton.onClick.AddListener(BuySkin);
            upgradeButton.onClick.AddListener(UpgradeSkin);
            chooseButton.onClick.AddListener(SelectSkin);
            skinButton.onClick.AddListener(SelectSkin);

            Player.OnChangeGems += Refresh;
            Player.OnPrefsUpdated += Refresh;
            Player.OnChangeSkin += TrySetSelectedImage;
            Localisation.OnLanguageChanged += Refresh;
        }


        private void OnDestroy()
        {
            Player.OnChangeGems -= Refresh;
            Player.OnPrefsUpdated -= Refresh;
            Player.OnChangeSkin -= TrySetSelectedImage;
            Localisation.OnLanguageChanged -= Refresh;
        }

        #endregion



        #region Public methods

        public void Init(int index)
        {
            skin = index;
            Refresh();
        }


        public void SetBuyTutorialState(ScrollRect scroll)
        {
            scrollRect = scroll;

            TutorialManager.Instance.SetFade(true);
            Canvas buyCanvas = upgradeButton.gameObject.AddComponent<Canvas>();
            buyCanvas.overrideSorting = true;
            buyCanvas.sortingLayerName = "UI";
            buyCanvas.sortingOrder = 4;
            upgradeButton.gameObject.AddComponent<GraphicRaycaster>();
            buyTutorialHand.gameObject.SetActive(true);
            scrollRect.enabled = false;

            upgradeButton.onClick.AddListener(SetNormalBuyState);
        }


        public bool NeedAlert()
        {
            return (!Player.IsSkinBought(skin) && Player.Gems >= Skins.GetSkinPrice(skin)) 
                || (Player.IsSkinBought(skin) && !Player.IsSkinMaxLevelReached(skin) && Player.Gems >= Skins.GetUpgradePrice(skin));
        }

        #endregion



        #region Private methods

        private void Refresh()
        {
            skinImage.sprite = Skins.GetSprite(skin);
            skinImage.SetNativeSize();

            skinName.text = Skins.GetName(skin);

            string formatedSkillDesc = string.Format(Skins.GetFormat(skin), Skins.GetPassiveSkillBonusForText(skin));
            string fullSkillDesc = string.Format(Skins.GetDesc(skin), SKILL_FORMATED_COLOR_BEGIN + formatedSkillDesc + SKILL_FORMATED_COLOR_END);
            skinDesc.text = fullSkillDesc;
            skinLvl.text = string.Format(SKILL_FORMATED_COLOR_BEGIN + SKILL_LVL + SKILL_FORMATED_COLOR_END, Player.GetSkinLevel(skin) + 1);

            TrySetSelectedImage();

            if (!Player.IsSkinBought(skin))
            {
                buyButton.gameObject.SetActive(true);
                upgradeButton.gameObject.SetActive(false);
                chooseButton.gameObject.SetActive(false);

                skinImage.material = grayscaleMaterial;
                skinImage.color = grayscaleColor;

                buyText.text = Skins.GetSkinPrice(skin).ToShortFormat();
                buyButton.GetComponent<MultiImageButton>().Interactable(Player.Gems >= Skins.GetSkinPrice(skin));

                skinButton.interactable = false;
            }
            else
            {
                buyButton.gameObject.SetActive(false);

                skinImage.material = null;
                skinImage.color = Color.white;

                bool isMaxUpgrade = Player.IsSkinMaxLevelReached(skin);
                upgradeButton.gameObject.SetActive(!isMaxUpgrade);
                chooseButton.gameObject.SetActive(isMaxUpgrade);

                if (!isMaxUpgrade)
                {
                    upgradePrice.text = Skins.GetUpgradePrice(skin).ToShortFormat();
                    upgradeButton.GetComponent<MultiImageButton>().Interactable(Player.Gems >= Skins.GetUpgradePrice(skin));
                }
                skinButton.interactable = true;
            }
        }


        private void BuySkin()
        {
            if (Player.IsSkinBought(skin))
            {
                return;
            }

            if (Player.TryRemoveGems(Skins.GetSkinPrice(skin)))
            {
                Player.BuySkin(skin);
                SelectSkin();

                buyButton.image.raycastTarget = false;
                StartCoroutine(RefreshAfterBuy());

                AudioManager.Instance.Play(buyClip, AudioType.Sound, delay: buyClipDelay);

                OnSkinBought(skin);
            }
            else
            {
                AudioManager.Instance.Play(unactiveClip, AudioType.Sound);
            }
        }


        private IEnumerator RefreshAfterBuy()
        {
            yield return new WaitForSeconds(0.2f);

            Refresh();
        }


        private void UpgradeSkin()
        {
            if (Player.TryRemoveGems(Skins.GetUpgradePrice(skin)))
            {
                Player.UpgradeSkin(skin);
                Refresh();
                EffectPlayer.Play(upgradeEffectPrefab, buyEffectAnchor.position);
                AudioManager.Instance.Play(upgradeClip, AudioType.Sound);

                OnSkinUpgrade(skin);
            }
            else
            {
                AudioManager.Instance.Play(unactiveClip, AudioType.Sound);
            }
        }


        private void SelectSkin()
        {
            ShooterShadowsConfig.SetShadowsInfo(playerSkin: skin);
            Player.CurrentSkin = skin;
        }


        private void TrySetSelectedImage()
        {
            select.enabled = (Player.CurrentSkin == skin);
        }


        private void SetNormalBuyState()
        {
            Destroy(upgradeButton.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(upgradeButton.gameObject.GetComponent<Canvas>());
            buyTutorialHand.gameObject.SetActive(false);
            scrollRect.enabled = true;

            TutorialManager.Instance.SetFade(false);
            TutorialManager.Instance.ShowTapToStart();
            TutorialManager.Instance.IsBuySkinTutorialPassed = true;

            upgradeButton.onClick.RemoveListener(SetNormalBuyState);
        }

        #endregion
    }
}

