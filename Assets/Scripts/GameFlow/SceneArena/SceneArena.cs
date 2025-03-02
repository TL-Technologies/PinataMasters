using Modules.General;
using Modules.General.Abstraction;
using System;
using UnityEngine;


namespace PinataMasters
{
    public class SceneArena : MonoBehaviour
    {
        #region Variables

        public static event Action OnPlay = delegate { };
        public static event Action OnShowLevelResult = null;

        [SerializeField]
        private UITransition uiTransition = null;

        private Arena arena = null;
        private Action onFinishArena = null;


        private Arena.Result arenaResult;

        private bool isWaitingPopup;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            arena = GetComponentInChildren<Arena>();

            UIOfferDialog.OnActiveStateChanged += UIOfferDialog_OnActiveStateChanged;
            UISettings.OnHide += UISettings_OnHide;
            CustomAdvertisingManager.OnFullScreenAdStarted += CustomAdvertisingManagerOnFullScreenAdStarted;
            CustomAdvertisingManager.OnFullScreenAdFinished += CustomAdvertisingManagerOnFullScreenAdFinished;
            LLApplicationStateRegister.OnApplicationEnteredBackground += LLApplicationStateRegister_OnApplicationEnteredBackground;
        }


        private void Start()
        {
            arena.Init();
        }


        private void OnDestroy()
        {
            UIOfferDialog.OnActiveStateChanged -= UIOfferDialog_OnActiveStateChanged;
            UISettings.OnHide -= UISettings_OnHide;
            CustomAdvertisingManager.OnFullScreenAdStarted -= CustomAdvertisingManagerOnFullScreenAdStarted;
            CustomAdvertisingManager.OnFullScreenAdFinished -= CustomAdvertisingManagerOnFullScreenAdFinished;
            LLApplicationStateRegister.OnApplicationEnteredBackground -= LLApplicationStateRegister_OnApplicationEnteredBackground;
        }

        #endregion



        #region Public methods

        public void Play(Action onFinish)
        {
            onFinishArena = onFinish;

            arena.Show(FinishArena);

            UILevel.Prefab.Instance.Show();
            UIAnnouncers.Prefab.Instance.Show();

            if (TutorialManager.Instance.IsShootTutorialPassed)
            {
                GameAnalytics.SendMatchStartEvent(Arsenal.GetWeaponDesc(Player.CurrentWeapon).ToLower(),
                                             (Player.Level + 1).ToString(),
                                             (Arsenal.GetWeaponMaxAmmo(Player.CurrentWeapon) *
                                              Arsenal.GetWeaponParameters(Player.CurrentWeapon).SizeMagazine).ToString());
            }

            OnPlay();

            IngameOffersController.Prefab.Instance.Configure();

            if (TutorialManager.Instance.IsShootTutorialPassed)
            {
                AudioManager.Instance.NormalizeAudio();
            }

            ShooterUpgradesBooster.asset.Value.TryToResumeBooster();
            CoinsBooster.asset.Value.TryToResumeBooster();
        }

        #endregion



        #region Private methods

        private void FinishArena(Arena.Result result)
        {
            arenaResult = result;

            if (result.IsForcedRestart)
            {
                arena.Show(FinishArena);
                Transition(true);
            }
            else if (UIOfferDialog.Prefab.Instance.IsShowing || UISettings.Prefab.Instance.IsShowing)
            {
                isWaitingPopup = true;
            }
            else
            {
                ShowLevelResult();
            }

            ShooterUpgradesBooster.asset.Value.DisableBooster();
            CoinsBooster.asset.Value.DisableBooster();
        }


        private void ShowLevelResult()
        {
            OnShowLevelResult?.Invoke();
            UILevelResult.Prefab.Instance.Show(arenaResult, (_) => Transition());
            AudioManager.Instance.MuffleAudio();
            AdvertisingHelper.ShowInterstitial(AdPlacementType.BeforeResult);
        }


        private void Transition(bool isForced = false)
        {
            if (isForced)
            {
                uiTransition.DecreaseScale(arena.Transition, ShowGameMenu);
            }
            else if (uiTransition.NeedShow)
            {
                uiTransition.DecreaseScale(arena.Transition, onFinishArena);
            }
            else
            {
                onFinishArena();
            }
        }

        
        private void TryShowLevelResultAfterPopup()
        {
            if (isWaitingPopup)
            {
                isWaitingPopup = false;
                ShowLevelResult();
            }
        }


        private void ShowGameMenu()
        {
            UILevel.Prefab.Instance.Show();
        }

        #endregion



        #region Events handlers

        void UIOfferDialog_OnActiveStateChanged(bool _isShown)
        {
            if (!_isShown)
            {
                TryShowLevelResultAfterPopup();
            }
        }


        void UISettings_OnHide()
        {
            TryShowLevelResultAfterPopup();
        }
        
        
        void LLApplicationStateRegister_OnApplicationEnteredBackground(bool isEnteredBackground)
        {
            if (isEnteredBackground)
            {
                ShooterUpgradesBooster.asset.Value.DisableBooster();
                CoinsBooster.asset.Value.DisableBooster();
            }
            else
            {
                ShooterUpgradesBooster.asset.Value.TryToResumeBooster();
                CoinsBooster.asset.Value.TryToResumeBooster();
            }
        }
        
        
        private void CustomAdvertisingManagerOnFullScreenAdFinished(AdModule adModule)
        {
            ShooterUpgradesBooster.asset.Value.TryToResumeBooster();
            CoinsBooster.asset.Value.TryToResumeBooster();
        }

        
        private void CustomAdvertisingManagerOnFullScreenAdStarted(AdModule adModule)
        {
            ShooterUpgradesBooster.asset.Value.DisableBooster();
            CoinsBooster.asset.Value.DisableBooster();
        }

        #endregion
    }
}
