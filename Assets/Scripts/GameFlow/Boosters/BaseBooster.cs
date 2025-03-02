using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    public enum BoosterType
    {
        None                = 0,
        ShooterUpgrades     = 1,
        Coins               = 2
    }


    public enum BoosterState
    {
        Active              = 1,
        Disabled            = 2
    }


    public abstract class BaseBooster : ScriptableObject 
    {
        #region Fields

        public static event Action<BoosterType, BoosterState> OnBoosterStateChanged;

        const string BOOSTER_TIMER_KEY = "{0}booster_timer_key";
        const string BOOSTER_STATE_KEY = "{0}booster_state_key";
        const string BOOSTER_TIME_KEY = "{0}booster_time_key";

        [SerializeField] BoosterType boosterType = BoosterType.None;
        [SerializeField] float defaultBoosterDuration = 0.0f;

        string boosterTimerKey;
        string boosterTimeKey;
        string boosterStateKey;

        Timer boosterTimer;

        BoosterState currentBoosterState = BoosterState.Disabled;

        #endregion



        #region Properties

        #region Keys

        string BoosterTimerKey  
        {
            get
            {
                if (string.IsNullOrEmpty(boosterTimerKey))
                {
                    boosterTimerKey = string.Format(BOOSTER_TIMER_KEY, boosterType);
                }

                return boosterTimerKey;
            }
        }


        string BoosterTimeKey  
        {
            get
            {
                if (string.IsNullOrEmpty(boosterTimeKey))
                {
                    boosterTimeKey = string.Format(BOOSTER_TIME_KEY, boosterType);
                }

                return boosterTimeKey;
            }
        }


        string BoosterStateKey  
        {
            get
            {
                if (string.IsNullOrEmpty(boosterStateKey))
                {
                    boosterStateKey = string.Format(BOOSTER_STATE_KEY, boosterType);
                }

                return boosterStateKey;
            }
        }

        #endregion

        public float PercentOfBoosterDurationLeft => BoosterDurationLeft / defaultBoosterDuration;

        public float BoosterDurationLeft => (CurrentBoosterState == BoosterState.Active) ? ((float)BoosterTimer.GetTimeLeft().TotalSeconds) : (BoosterDuration);

        public BoosterState CurrentBoosterState
        {
            get => currentBoosterState;
            private set
            {
                if (currentBoosterState != value)
                {
                    currentBoosterState = value;
                    
                    switch (value)
                    {
                        case BoosterState.Active:
                            BoosterTimer.Start(BoosterDuration, OnBoosterTimeOver);
                            break;
                        case BoosterState.Disabled:
                            BoosterDuration = (float)BoosterTimer.GetTimeLeft().TotalSeconds;
                            BoosterTimer.Stop();
                            break;
                    }

                    OnBoosterStateChanged?.Invoke(boosterType, value);
                }
            }
        }


        float BoosterDuration
        {
            get => CustomPlayerPrefs.GetFloat(BoosterTimeKey, 0.0f);
            set
            {
                CustomPlayerPrefs.SetFloat(BoosterTimeKey, value);
            }
        }


        Timer BoosterTimer
        {
            get
            {
                if (boosterTimer == null)
                {
                    boosterTimer = new Timer(BoosterTimerKey);
                }

                return boosterTimer;
            }
        } 

        #endregion



        #region Public Methods

        public void ActivateBooster()
        {
            BoosterDuration = defaultBoosterDuration;
            CurrentBoosterState = BoosterState.Active;
        }


        public void DisableBooster()
        {
            CurrentBoosterState = BoosterState.Disabled;
        }


        public void TryToResumeBooster()
        {
            if (BoosterDuration > 0.0f)
            {
                CurrentBoosterState = BoosterState.Active;
            }
        }

        #endregion



        #region Private Methods

        void OnBoosterTimeOver()
        {
            CurrentBoosterState = BoosterState.Disabled;
        }

        #endregion
    }
}