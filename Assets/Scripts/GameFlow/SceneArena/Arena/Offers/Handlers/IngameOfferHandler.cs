using Modules.General.Obsolete;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PinataMasters
{
    using HelperTypes;


    public enum IngameOfferType
    {
        FreeCoins = 0,
        X2CoinBonus = 1,
        CoinBonus = 2,
        GemsBonus = 3,
        SpeedBonus = 4
    }


    public class IngameOfferHandler
    {
        #region Fields

        IngameOffersSettings settings;

        ObjectPool poolForObject;

        bool canSpawnOffer = true;

        #endregion



        #region Properties

        public IngameOffersController Controller { get; private set; }


        private SimpleTimer IngameOfferLifetimeTimer { get; set; }


        private IngameOffer IngameOffer { get; set; }


        private SimpleTimer SpawnTimer { get; set; }

        #endregion



        #region Class lifecycle

        public IngameOfferHandler(IngameOffersController controller, IngameOffersSettings settings)
        {
            Controller = controller;
            this.settings = settings;

            InitialiseOfferSpawnTimer(settings.DelayBeforeSpawn);
        }

        #endregion



        #region Public methods

        public void ConfigureHandler(IngameOffersController controller, IngameOffersSettings settings)
        {
            Controller = controller;
            this.settings = settings;

            if (IngameOffer == null && SpawnTimer != null)
            {
                InitialiseOfferSpawnTimer(Mathf.Max(SpawnTimer.RemainingTime, 0.0f));
            }
        }


        public void CustomUpdate(float deltaTime, bool isUpdateOnlyCooldown = false)
        {
            if (SpawnTimer != null)
            {
                SpawnTimer.CustomUpdate(deltaTime);
            }

            if (!isUpdateOnlyCooldown)
            {
                IngameOfferLifetimeTimer?.CustomUpdate(deltaTime);
            }
        }


        public void RemoveOffer(bool animated, bool positive = true)
        {
            IngameOfferLifetimeTimer = null;
            IngameOffer.Remove(animated, () =>
            {
                IngameOffer = null;
                canSpawnOffer = true;
                InitialiseOfferSpawnTimer((positive) ? (settings.PositiveCooldown) : (settings.NegativeCooldown));
            });
        }

        #endregion



        #region Private Methods

        void TrySpawnOffer()
        {
            Pinata.OnPinataDead += Pinata_OnPinataDead;
            ShooterBody.OnOutOfAmmo += ShooterBody_OnOutOfAmmo;
            Arena.OnStartLevel += Arena_OnStartLevel;

            IngameOfferSettings ingameOfferSettings = GetIngameOffer(settings);

            if (ingameOfferSettings != null && IngameOffer == null && canSpawnOffer)
            {
                poolForObject = PoolManager.Instance.PoolForObject(ingameOfferSettings.IngameOffer.gameObject);

                IngameOffer = poolForObject.Pop().GetComponent<IngameOffer>();
                IngameOffer.Initialize(ingameOfferSettings, this, settings.AnimationSettings);
                IngameOffer.Spawn(Controller.transform);

                InitialiseOfferLifetimeTimer(settings.Lifetime);
            }
            else
            {
                InitialiseOfferSpawnTimer(settings.NegativeCooldown);
            }
        }


        void InitialiseOfferSpawnTimer(float delay)
        {
            SimpleTimer timer = new SimpleTimer(delay, () =>
            {
                Pinata.OnPinataDead -= Pinata_OnPinataDead;
                ShooterBody.OnOutOfAmmo -= ShooterBody_OnOutOfAmmo;
                Arena.OnStartLevel -= Arena_OnStartLevel;

                SpawnTimer = null;

                TrySpawnOffer();
            });

            SpawnTimer = timer;
        }


        void InitialiseOfferLifetimeTimer(float lifetime)
        {
            Player.OnResetProgress += Player_OnResetProgress;

            IngameOfferLifetimeTimer = new SimpleTimer(lifetime, () =>
            {
                IngameOfferLifetimeTimer = null;

                RemoveOffer(true, false);

                Player.OnResetProgress -= Player_OnResetProgress;
            });
        }


        IngameOfferSettings GetIngameOffer(IngameOffersSettings offersSettings)
        {
            List<IngameOfferSettings> possibleOffers = offersSettings.IngameOffers.Where((offer) => (offer.Weight > 0f)).ToList();

            IngameOfferSettings offerSettings = null;

            if (possibleOffers.Count > 0)
            {
                float weight = 0f;

                for (int idx = 0; idx < possibleOffers.Count; idx++)
                {
                    weight += possibleOffers[idx].Weight;
                }

                float randomWeight = Random.Range(0f, weight);
                weight = 0f;

                offerSettings = possibleOffers.Last();

                for (int idx = 0; idx < possibleOffers.Count; idx++)
                {
                    weight += possibleOffers[idx].Weight;

                    if (weight > randomWeight)
                    {
                        offerSettings = possibleOffers[idx];
                        break;
                    }
                }
            }

            return offerSettings;
        }

        #endregion



        #region Events handler

        void Player_OnResetProgress()
        {
            if (IngameOffer != null)
            {
                RemoveOffer(false, false);
            }
        }


        void Pinata_OnPinataDead()
        {
            canSpawnOffer = false;
        }


        void ShooterBody_OnOutOfAmmo()
        {
            canSpawnOffer = false;
        }


        void Arena_OnStartLevel()
        {
            canSpawnOffer = true;
        }

        #endregion
    }
}
