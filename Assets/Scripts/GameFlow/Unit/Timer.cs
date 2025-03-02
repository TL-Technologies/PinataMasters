using Modules.General.HelperClasses;
using System;


namespace PinataMasters
{
    public class Timer
    {
    
        #region Variables

        const string STARTTIME = "_start_time";
        const string FINISHTIME = "_finish_time";

        private string key;
        private Action timeOverCallback;

        #endregion



        #region Public methods

        public Timer(string timerKey)
        {
            key = timerKey;
        }


        public void Start(float duration, Action callback)
        {
            SetStartTime();
            SetFinishTime(duration);

            timeOverCallback = callback;

            Sheduler.OnUpdate += OnUpdate;
        }


        public void Init(Action callback)
        {
            if (CustomPlayerPrefs.HasKey(key + STARTTIME) && CustomPlayerPrefs.HasKey(key + FINISHTIME))
            {
                timeOverCallback = callback;
                Sheduler.OnUpdate += OnUpdate;
            }
        }


        public void Stop()
        {
            Sheduler.OnUpdate -= OnUpdate;

            CustomPlayerPrefs.DeleteKey(key + STARTTIME);
            CustomPlayerPrefs.DeleteKey(key + FINISHTIME);
        }


        public TimeSpan GetTimeLeft()
        {
            return GetFinishTime().Subtract(DateTime.UtcNow);
        }


        public DateTime GetStartTime()
        {
            return CustomPlayerPrefs.GetDateTime(key + STARTTIME, DateTime.UtcNow);
        }


        public DateTime GetFinishTime()
        {
            return CustomPlayerPrefs.GetDateTime(key + FINISHTIME, DateTime.UtcNow);
        }


        public bool IsTimeOver()
        {
            return GetTimeLeft().TotalSeconds < 0d;
        }

        #endregion



        #region Private methods

        private void SetStartTime()
        {
            CustomPlayerPrefs.SetDateTime(key + STARTTIME, DateTime.UtcNow);
        }


        private void SetFinishTime(float duration)
        {
            CustomPlayerPrefs.SetDateTime(key + FINISHTIME, DateTime.UtcNow.AddSeconds(duration));
        }


        private void OnUpdate()
        {
            if (IsTimeOver())
            {
                timeOverCallback();
                Stop();
            }
        }

        #endregion
    }
}
