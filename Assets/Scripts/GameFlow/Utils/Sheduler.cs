using System.Collections;
using UnityEngine;
using System;

namespace PinataMasters
{
    public class Sheduler : MonoBehaviour
    {
        #region Variables

        public static event Action OnGameStart = delegate { };
        public static event Action OnUpdate = delegate { };
        public static event Action OnFixedUpdate = delegate { };
        public static event Action OnDestred;


        public static bool IsGameStarted;

        private static Sheduler instance;

        #endregion



        #region Init

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            GameObject go = new GameObject("MonobehaviorSheduler");
            instance = go.AddComponent<Sheduler>();
            DontDestroyOnLoad(go);
        }

        public static Coroutine PlayCoroutine(IEnumerator routine)
        {
            return instance.StartCoroutine(routine);
        }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            IsGameStarted = true;
            OnGameStart();
        }


        private void Update()
        {
            OnUpdate();
        }


        private void FixedUpdate()
        {
            OnFixedUpdate();
        }


        private void OnDestroy()
        {
            OnDestred?.Invoke();
        }

        #endregion
    }
}
