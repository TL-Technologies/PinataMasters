using Modules.General;
using System;
using UnityEngine;

namespace PinataMasters
{
    public class IngameOfferFreeGoldReward : IngameOfferReward
    {
        #region Fields

        const float SpawnCoinsDuration = 0.1f;

        [SerializeField] AudioClip claimedOfferSound = null;
        [SerializeField] uint spawnedCoinsCount = 0;
        [SerializeField] IngameCurrencySpawner coinsSpawner = null;
        [SerializeField] IngameOfferVFX explodeVFX = null;

        #endregion



        #region Public methods

        public override void TryClaimReward(Action<ClaimResult> callback)
        {
            AudioManager.Instance.Play(claimedOfferSound, AudioType.Sound);

            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                coinsSpawner.SpawnIngameCurrency(spawnedCoinsCount, Arsenal.GetWeaponPower(Player.CurrentWeapon), IngameCurrencySpawner.Type.Time, SpawnCoinsDuration, false, null);
            }, rewardDelay);

            explodeVFX.Spawn(() =>
            {
                callback?.Invoke(new ClaimResult() { animated = false, claimed = true });
            });
        }

        #endregion
    }
}
