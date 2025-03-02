using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;


namespace PinataMasters
{
    public class ViewGun : MonoBehaviour
    {
        #region Variables

        public static event Action OnWeaponUpgrade = delegate { };
        public static event Action OnWeaponBuy = delegate { };
        public static event Action<float, string> OnNeedShowMiniBank = delegate { };

        [SerializeField]
        private Image gunImage = null;
        [SerializeField]
        private Image maskImage = null;
        [SerializeField]
        private Button gunButton = null;
        [SerializeField]
        private Image selectImage = null;
        [SerializeField]
        private Image selectSmallImage = null;
        [SerializeField]
        private Image shooterIconImage = null;
        [SerializeField]
        private Image selectShadowImage = null;
        [SerializeField]
        private Image selectShadowSmallImage = null;

        [Header("Buy")]
        [SerializeField]
        private RectTransform buyFrame = null;
        [SerializeField]
        private RectTransform buyTutorial = null;
        [SerializeField]
        private Button buyButton = null;
        [SerializeField]
        private Text buyText = null;
        [SerializeField]
        private Text buyAmmoText = null;
        [SerializeField]
        private Text buyDamageText = null;

        [Header("Upgrade")]
        [SerializeField]
        private RectTransform upgradeFrame = null;
        [SerializeField]
        private RectTransform upgradeTutorial = null;

        [SerializeField]
        private Text currentDamageText = null;
        [SerializeField]
        private Button upgradeDamageButton = null;
        [SerializeField]
        private Text upgradePriceDamageText = null;

        [SerializeField]
        private Text currentAmmoText = null;
        [SerializeField]
        private Button upgradeAmmoButton = null;
        [SerializeField]
        private Text upgradePriceAmmoText = null;

        [Header("Effects")]
        [SerializeField]
        private ParticleSystem upgradeEffect = null;
        [SerializeField]
        private RectTransform unlockEffectAnchor = null;

        [Header("Sounds")]
        [SerializeField]
        private AudioClip upgradeClip = null;
        [SerializeField]
        private AudioClip buyClip = null;
        [SerializeField]
        private AudioClip miniBankClip = null;

        private int weapon;
        private ScrollRect scrollRect;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            buyButton.onClick.AddListener(OnClickBuy);
            upgradeAmmoButton.onClick.AddListener(OnUpgradeAmmo);
            upgradeDamageButton.onClick.AddListener(OnUpgradeDamage);
            gunButton.onClick.AddListener(OnSelect);

            Player.OnChangeCoins += Refresh;
            Player.OnPrefsUpdated += Refresh;
            ViewSkin.OnSkinBought += Refresh;
            ViewSkin.OnSkinUpgrade += Refresh;
            ShooterShadowsConfig.OnShadowsChanged += Refresh;
        }


        private void OnDestroy()
        {
            Player.OnChangeCoins -= Refresh;
            Player.OnPrefsUpdated -= Refresh;
            ViewSkin.OnSkinBought -= Refresh;
            ViewSkin.OnSkinUpgrade -= Refresh;
            ShooterShadowsConfig.OnShadowsChanged -= Refresh;
        }

        #endregion



        #region Public methods

        public void Init(int typeWeapon)
        {
            weapon = typeWeapon;
            Refresh();
        }


        public bool NeedAlert()
        {
            return (!Player.IsWeaponBought(weapon) && Player.Coins >= Arsenal.GetWeaponPrice(weapon)) 
                || (Player.IsWeaponBought(weapon) && (Player.Coins >= Arsenal.GetAmmoUpgradePrice(Player.CurrentWeapon) || Player.Coins >= Arsenal.GetDamageUpgradePrice(Player.CurrentWeapon)));
        }


        public void SetUpgradeTutorialState(ScrollRect scroll)
        {
            scrollRect = scroll;

            TutorialManager.Instance.SetFade(true);
            Canvas ammoUpgradeCanvas = upgradeAmmoButton.gameObject.AddComponent<Canvas>();
            ammoUpgradeCanvas.overrideSorting = true;
            ammoUpgradeCanvas.sortingLayerName = "UI";
            ammoUpgradeCanvas.sortingOrder = 4;
            upgradeAmmoButton.gameObject.AddComponent<GraphicRaycaster>();
            upgradeTutorial.gameObject.SetActive(true);
            scrollRect.enabled = false;

            upgradeAmmoButton.onClick.AddListener(SetNormalUpgradeState);
        }


        public void SetBuyTutorialState(ScrollRect scroll)
        {
            scrollRect = scroll;

            TutorialManager.Instance.SetFade(true);
            Canvas buyCanvas = buyButton.gameObject.AddComponent<Canvas>();
            buyCanvas.overrideSorting = true;
            buyCanvas.sortingLayerName = "UI";
            buyCanvas.sortingOrder = 4;
            buyButton.gameObject.AddComponent<GraphicRaycaster>();
            buyTutorial.gameObject.SetActive(true);
            scrollRect.enabled = false;

            buyButton.onClick.AddListener(SetNormalBuyState);
        }

        #endregion



        #region Private methods

        private void Refresh()
        {
            gunImage.sprite = Arsenal.GetWeaponSprite(weapon);
            gunImage.SetNativeSize();
            maskImage.sprite = Arsenal.GetWeaponMask(weapon);
            maskImage.SetNativeSize();

            bool isShooter = (ShooterShadowsConfig.GetShooterInfo().weapon == weapon);
            shooterIconImage.sprite = ShooterShadowsConfig.GetShadowSprite(weapon);
            selectShadowImage.gameObject.SetActive(shooterIconImage.sprite != null);
            selectShadowSmallImage.gameObject.SetActive(shooterIconImage.sprite != null);
            if (isShooter && shooterIconImage.sprite == null)
            {
                shooterIconImage.sprite = Skins.GetShadowSprite(ShooterShadowsConfig.GetShooterInfo().skin);
            }
            shooterIconImage.enabled = (shooterIconImage.sprite != null);
            selectSmallImage.enabled = isShooter;
            selectImage.enabled = isShooter;

            if (!Player.IsWeaponBought(weapon))
            {
                gunButton.interactable = false;

                buyFrame.gameObject.SetActive(true);
                upgradeFrame.gameObject.SetActive(false);

                buyAmmoText.text = Arsenal.GetWeaponBaseAmmo(weapon).ToString();
                buyDamageText.text = Arsenal.GetWeaponBaseDamage(weapon).ToShortFormat();

                buyText.text = Arsenal.GetWeaponPrice(weapon).ToShortFormat();
                buyButton.GetComponent<MultiImageButton>().Interactable(Player.Coins >= Arsenal.GetWeaponPrice(weapon));
            }
            else
            {
                gunButton.interactable = true;

                buyFrame.gameObject.SetActive(false);
                upgradeFrame.gameObject.SetActive(true);

                currentAmmoText.text = Arsenal.GetWeaponMaxAmmo(weapon).ToString();
                upgradeAmmoButton.GetComponent<MultiImageButton>().Interactable(Player.Coins >= Arsenal.GetAmmoUpgradePrice(weapon));
                upgradePriceAmmoText.text = (Arsenal.GetAmmoUpgradePrice(weapon)).ToShortFormat();

                currentDamageText.text = Arsenal.GetWeaponDamage(weapon).ToShortFormat();
                upgradeDamageButton.GetComponent<MultiImageButton>().Interactable(Player.Coins >= Arsenal.GetDamageUpgradePrice(weapon));
                upgradePriceDamageText.text = (Arsenal.GetDamageUpgradePrice(weapon)).ToShortFormat();
            }
        }


        private void Refresh(int index)
        {
            Refresh();
        }


        private void OnClickBuy()
        {
            if (Player.IsWeaponBought(weapon))
            {
                return;
            }

            if (Player.TryRemoveCoins(Arsenal.GetWeaponPrice(weapon)))
            {
                Player.BuyWeapon(weapon);
                ShooterShadowsConfig.SetShadowsInfo(playerWeapon: weapon);
                Player.CurrentWeapon = weapon;

                buyButton.image.raycastTarget = false;
                StartCoroutine(RefreshAfterBuy());
                Instantiate(Arsenal.GetWeaponUnlockEffect(weapon), unlockEffectAnchor).Play(true);
                OnWeaponBuy();

                AudioManager.Instance.Play(buyClip, AudioType.Sound);
            }
            else
            {
                OnNeedShowMiniBank(Arsenal.GetWeaponPrice(weapon) - Player.Coins, MiniBankPlacement.BUTTON_WEAPON_GET);
                AudioManager.Instance.Play(miniBankClip, AudioType.Sound);
            }
        }


        private IEnumerator RefreshAfterBuy()
        {
            yield return new WaitForSeconds(0.2f);

            Refresh();
        }


        private void OnUpgradeAmmo()
        {
            if (Player.TryRemoveCoins(Arsenal.GetAmmoUpgradePrice(weapon)))
            {
                Player.BuyAmmoUpgrade(weapon);

                AudioManager.Instance.Play(upgradeClip, AudioType.Sound);
                upgradeEffect.Play();

                Refresh();
            }
            else
            {
                OnNeedShowMiniBank(Arsenal.GetAmmoUpgradePrice(weapon) - Player.Coins, MiniBankPlacement.WEAPON_UPGRADE);
                AudioManager.Instance.Play(miniBankClip, AudioType.Sound);
            }

            if (!TutorialManager.Instance.IsUpgradeWeaponTutorialPassed)
            {
                upgradeEffect.Play();
                OnWeaponUpgrade();
            }
        }


        private void OnUpgradeDamage()
        {
            if (Player.TryRemoveCoins(Arsenal.GetDamageUpgradePrice(weapon)))
            {
                Player.BuyDamageUpgrade(weapon);

                AudioManager.Instance.Play(upgradeClip, AudioType.Sound);
                upgradeEffect.Play();

                Refresh();
            }
            else
            {
                OnNeedShowMiniBank(Arsenal.GetDamageUpgradePrice(weapon) - Player.Coins, MiniBankPlacement.WEAPON_UPGRADE);
                AudioManager.Instance.Play(miniBankClip, AudioType.Sound);
            }

            if (!TutorialManager.Instance.IsUpgradeWeaponTutorialPassed)
            {
                upgradeEffect.Play();
                OnWeaponUpgrade();
            }
        }


        private void OnSelect()
        {
            ShooterShadowsConfig.SetShadowsInfo(playerWeapon: weapon);
            Player.CurrentWeapon = weapon;
        }


        private void SetNormalUpgradeState()
        {
            Destroy(upgradeAmmoButton.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(upgradeAmmoButton.gameObject.GetComponent<Canvas>());
            upgradeTutorial.gameObject.SetActive(false);
            scrollRect.enabled = true;

            upgradeAmmoButton.onClick.RemoveListener(SetNormalUpgradeState);

            TutorialManager.Instance.SetFade(false);
            TutorialManager.Instance.ShowTapToStart();
            TutorialManager.Instance.IsUpgradeWeaponTutorialPassed = true;
        }


        private void SetNormalBuyState()
        {
            Destroy(buyButton.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(buyButton.gameObject.GetComponent<Canvas>());
            buyTutorial.gameObject.SetActive(false);
            scrollRect.enabled = true;

            TutorialManager.Instance.SetFade(false);
            TutorialManager.Instance.ShowTapToStart();
            TutorialManager.Instance.IsBuyWeaponTutorialPassed = true;

            buyButton.onClick.RemoveListener(SetNormalBuyState);
        }

        #endregion
    }
}
