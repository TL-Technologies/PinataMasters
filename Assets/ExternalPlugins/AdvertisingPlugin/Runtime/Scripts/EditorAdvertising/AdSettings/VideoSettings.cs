using System;
using UnityEngine;


namespace Modules.Advertising
{
    [Serializable]
    public class VideoSettings
    {
        #region Fields

        [SerializeField] private float fullScreenAdDuration = 15.0f;
        [SerializeField] private float interstitialCloseButtonDelay = 5.0f;
        [SerializeField] private bool shouldUseUnscaledDeltaTime = false;

        #endregion



        #region Properties

        public float FullScreenAdDuration => fullScreenAdDuration;


        public float InterstitialCloseButtonDelay => interstitialCloseButtonDelay;


        public bool ShouldUseUnscaledDeltaTime => shouldUseUnscaledDeltaTime;

        #endregion
    }
}