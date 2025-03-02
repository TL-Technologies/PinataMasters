using UnityEngine;
using System;


namespace PinataMasters
{
    public class IngameOfferGoldBusterReward : IngameOfferReward
    {
        #region Fields

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
                    callback?.Invoke(new ClaimResult { animated = false, claimed = result.IsSuccessfully });
                });

                CoinsBooster.asset.Value.ActivateBooster();
            }
            else
            {
                callback?.Invoke(new ClaimResult { animated = true, claimed = result.IsSuccessfully });
            }
        }

        #endregion
    }
}
