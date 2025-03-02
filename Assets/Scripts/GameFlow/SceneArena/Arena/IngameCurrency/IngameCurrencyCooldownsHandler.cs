using System;


namespace PinataMasters
{
    using HelperTypes;

    public class IngameCurrencyCooldownsHandler
    {
        #region Nested types

        [Serializable]
        public struct Settings
        {
            public IngameCurrencyType currencyType;
            public float textCooldown;
            public float soundsCooldown;
            public float vibrationCooldown;
        }

        #endregion



        #region Fields

        Settings settings;

        bool isTextAvailable = true;
        bool isSoundsAvailable = true;
        bool isVibrationAvailable = true;

        SimpleTimer textCooldownTimer;
        SimpleTimer soundsCooldownTimer;
        SimpleTimer vibrationCooldownTimer;

        #endregion



        #region Class lifecycle

        public IngameCurrencyCooldownsHandler(Settings settings)
        {
            this.settings = settings;
        }

        #endregion



        #region Public methods

        public void CustomUpdate(float deltaTime)
        {
            textCooldownTimer?.CustomUpdate(deltaTime);
            soundsCooldownTimer?.CustomUpdate(deltaTime);
            vibrationCooldownTimer?.CustomUpdate(deltaTime);
        }


        public void TrySpawnIngameOfferText(Action callback)
        {
            if (isTextAvailable)
            {
                isTextAvailable = false;

                callback?.Invoke();

                textCooldownTimer = new SimpleTimer(settings.textCooldown, () =>
                {
                    textCooldownTimer = null;
                    isTextAvailable = true;
                });
            }
        }


        public void TryPlayIngameOfferSound(Action callback)
        {
            if (isSoundsAvailable)
            {
                isSoundsAvailable = false;

                callback?.Invoke();

                soundsCooldownTimer = new SimpleTimer(settings.soundsCooldown, () =>
                {
                    soundsCooldownTimer = null;
                    isSoundsAvailable = true;
                });
            }
        }


        public void TryPlayIngameOfferVibration(Action callback)
        {
            if (isVibrationAvailable)
            {
                isVibrationAvailable = false;

                callback?.Invoke();

                vibrationCooldownTimer = new SimpleTimer(settings.soundsCooldown, () =>
                {
                    vibrationCooldownTimer = null;
                    isVibrationAvailable = true;
                });
            }
        }

        #endregion
    }
}
