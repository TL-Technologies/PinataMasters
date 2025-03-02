using Modules.General.Obsolete;
using UnityEngine;
using System;
using System.Threading;


namespace PinataMasters
{
    public class IngameOffer : MonoBehaviour, IInteractionHandler, IPoolCallback
    {
        #region Fields

        [Header("Components")]
        [SerializeField] IngameOfferReward offerReward;
        [SerializeField] IngameOfferAnimation animationComponent;
        [SerializeField] BoxCollider2D offerCollider;

        [Header("Settings")]
        [SerializeField] bool isAnimatedDestoy;

        IngameOfferSettings settings;
        IngameOfferHandler offerHandler;

        #endregion



        #region Unity Lifecycle

        void Awake()
        {
            IngameOfferFlyAnimation.OnBirdLeaveFlyZone += IngameOfferFlyAnimation_OnBirdLeaveFlyZone;
            ShooterBody.OnOutOfAmmo += ShooterBody_OnOutOfAmmo;
            Pinata.OnPinataDead += Pinata_OnPinataDead;
            Arena.OnStartLevel += Arena_OnStartLevel;
        }


        void OnDestroy()
        {
            IngameOfferFlyAnimation.OnBirdLeaveFlyZone -= IngameOfferFlyAnimation_OnBirdLeaveFlyZone;
            ShooterBody.OnOutOfAmmo -= ShooterBody_OnOutOfAmmo;
            Pinata.OnPinataDead -= Pinata_OnPinataDead;
            Arena.OnStartLevel -= Arena_OnStartLevel;
        }

        #endregion



        #region Public methods

        public void Initialize(IngameOfferSettings settings, IngameOfferHandler offerHandler, IngameOfferAnimationSettings animationSettings)
        {
            this.settings = settings;
            this.offerHandler = offerHandler;
            offerReward.Initialize(settings.RewardSettings);
            animationComponent.Initialize(animationSettings, offerHandler);
        }


        public void Spawn(Transform root)
        {
            offerCollider.enabled = true;
            transform.parent = root;
            animationComponent.ApplyAnimation(IngameOfferAnimation.Type.Spawn);
            settings.ApplyReaction(false);
            GameAnalytics.IngameBonusShow(CustomAdPlacementType.GetIngameOfferPlacement(settings.RewardSettings.OfferType));
        }


        public void Remove(bool animated, Action callback)
        {
            if (animated && isAnimatedDestoy)
            {
                animationComponent.ApplyAnimation(IngameOfferAnimation.Type.Remove, () =>
                {
                    callback?.Invoke();

                    gameObject.ReturnToPool();
                });
            }
            else
            {
                callback?.Invoke();

                gameObject.ReturnToPool();
            }
        }

        #endregion



        #region Private methods

        public void OnMouseDown()
        {
            if (offerHandler.Controller.IsOfferTouchesAvailable)
            {
                GameAnalytics.IngameBonusClick(CustomAdPlacementType.GetIngameOfferPlacement(settings.RewardSettings.OfferType));

                offerCollider.enabled = false;

                offerReward.TryClaimReward((result) =>
                {
                    offerCollider.enabled = true;
                    settings.ApplyReaction(result.claimed);

                    if (result.claimed)
                    {
                        offerHandler.RemoveOffer(result.animated, true);
                    }
                });
            }
        }

        #endregion



        #region Events Handler

        void Arena_OnStartLevel()
        {
            offerCollider.enabled = true;
        }


        void IngameOfferFlyAnimation_OnBirdLeaveFlyZone()
        {
            offerCollider.enabled = false;
        }


        void ShooterBody_OnOutOfAmmo()
        {
            offerCollider.enabled = false;
        }


        void Pinata_OnPinataDead()
        {
            offerCollider.enabled = false;
        }

        #region IPoolCallback

        public void OnCreateFromPool()
        {

        }


        public void OnReturnToPool()
        {
            offerCollider.enabled = true;
        }


        public void OnPop()
        {

        }


        public void OnPush()
        {

        }

        #endregion

        #endregion
    }
}
