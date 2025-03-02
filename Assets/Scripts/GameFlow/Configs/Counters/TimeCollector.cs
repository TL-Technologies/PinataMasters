using Modules.General;
using UnityEngine;

namespace PinataMasters
{
    public static class TimeCollector
    {
        public static float Value { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Start()
        {
            OnEnteredBackground(false);
            LLApplicationStateRegister.OnApplicationEnteredBackground += OnEnteredBackground;
        }


        private static void OnEnteredBackground(bool isEnteredBackground)
        {
            if (isEnteredBackground)
            {
                Sheduler.OnUpdate -= Update;
            }
            else
            {
                Sheduler.OnUpdate += Update;
            }
        }


        private static void Update()
        {
            Value += Time.deltaTime;
        }


        public static void Reset()
        {
            Value = 0f;
        }
    }
}