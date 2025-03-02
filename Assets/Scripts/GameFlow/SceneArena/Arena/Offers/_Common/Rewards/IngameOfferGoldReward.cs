using Modules.General;
using System;
using UnityEngine;

namespace PinataMasters
{
    public class IngameOfferGoldReward : IngameOfferReward
    {
        #region Fields

        const float SpawnCoinsDuration = 0.5f;

        [SerializeField] uint spawnedCoinsCount = 0;
        [SerializeField] IngameCurrencySpawner coinsSpawner = null;
        [SerializeField] IngameOfferVFX explodeVFX = null;

        Action<ClaimResult> callback;

        #endregion



        #region Public methods

        public override void TryClaimReward(Action<ClaimResult> callback)
        {
            this.callback = callback;

            UIOfferDialog.Prefab.Instance.Show(RewardSettings, OnPopupHide);
            AudioManager.Instance.MuffleAudio();

            TapZone.Lock(true);
        }

        #endregion



        #region Private methods

        void OnPopupHide(UIOfferDialog.Result result)
        {
            TapZone.Lock(false);

            if (result.IsSuccessfully)
            {
                explodeVFX.Spawn(() =>
                {
                    callback?.Invoke(new ClaimResult() { animated = false, claimed = result.IsSuccessfully });
                });

                Scheduler.Instance.CallMethodWithDelay(this, () =>
                {
                    coinsSpawner.SpawnIngameCurrency(spawnedCoinsCount, RewardSettings.Value, IngameCurrencySpawner.Type.Time, SpawnCoinsDuration, false, null);
                }, rewardDelay);
            }
            else
            {
                callback?.Invoke(new ClaimResult() { animated = true, claimed = result.IsSuccessfully });
            }
        }

        #endregion
    }
}
