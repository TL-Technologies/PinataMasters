using Modules.Analytics;
using Modules.General;
using Modules.Legacy.Tweens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace PinataMasters
{
    public class Arena : Unit<Arena.Result>
    {

        #region Types

        public class Result : UnitResult
        {
            public bool Win;
            public float Coins;
            public float MissedShells;
            public float ObstaclesHitShells;
            public bool IsForcedRestart;
        }


        public enum State
        {
            Playing,
            OutOfAmmo,
            Win,
            Lose
        }

        #endregion



        #region Variables

        public static event Action OnStartLevel = delegate { };
        public static event Action<State> OnStateChange = delegate { };

        [SerializeField]
        private Rope rope = null;
        [SerializeField]
        private Transform anchor = null;
        [SerializeField]
        private Transform placePinata = null;
        [SerializeField]
        private float finishDelay = 0.0f;
        [SerializeField]
        Shooter shooterShadowPrefab = null;
        [SerializeField]
        private Transform shadowsTransformParent;

        private int shellCount;

        private Pinata pinata;
        private Shooter shooter;
        private readonly Dictionary<GameObject, GameObject> backs = new Dictionary<GameObject, GameObject>();
        private GameObject levelObstacles;
        private GameObject pinataObstacles;

        private float missedShells;
        private float obstaclesHitShells;

        private State state = State.Playing;

        IngameCurrencySystem ingameCurrencySystemInstance;

        Dictionary<Shooter, ShadowInfo> shadowsInfo = new Dictionary<Shooter, ShadowInfo>();

        #endregion



        #region Properties

        private State CurrentState
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                OnStateChange(state);
            }
        }


        private IngameCurrencySystem IngameCurrencySystemInstance => ingameCurrencySystemInstance ?? (ingameCurrencySystemInstance = IngameCurrencySystem.Prefab.Instance);

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            shooter = GetComponentInChildren<Shooter>();
            Shell.OnShellSpawn += OnShellSpawn;
            Shell.OnShellDestroy += OnShellDestroy;
            Player.OnResetProgress += () => { HideBack(); ShowBack(SelectorLevels.GetLevels.GetPrefabBack(Player.Level)); };
            ShooterShadowsConfig.OnShadowsChanged += InitShadows;
            Player.OnPrefsUpdated += RestartGame;
        }


        private void Start()
        {
            ShowBack(SelectorLevels.GetLevels.GetPrefabBack(Player.Level));
        }


        private void OnDestroy()
        {
            Shell.OnShellSpawn -= OnShellSpawn;
            Shell.OnShellDestroy -= OnShellDestroy;
            ShooterShadowsConfig.OnShadowsChanged -= InitShadows;
            Player.OnPrefsUpdated -= RestartGame;
        }

        #endregion



        #region Public methods

        public void Init()
        {
            shooter.Init(OnOutOfAmmo);

            ShooterShadowsConfig.SetShadowsInfo();

            InitShadows();
        }


        public override void Show(Action<Result> onHided, Action onShowed = null)
        {
            base.Show(onHided, onShowed);

            if (pinata)
            {
                Destroy(pinata.gameObject);
                Destroy(pinataObstacles);
            }
            HideBack();

            ShowBack(SelectorLevels.GetLevels.GetPrefabBack(Player.Level));

            pinata = Instantiate(SelectorLevels.GetLevels.GetPrefabPinata(Player.Level));
            pinata.Init(anchor, placePinata.position, OnBreakPinata);
            rope.GenerateRope(pinata);

            if (SelectorLevels.GetLevels.GetPrefabPinataObstacles(Player.Level) != null)
            {
                pinataObstacles = Instantiate(SelectorLevels.GetLevels.GetPrefabPinataObstacles(Player.Level), transform);
                PinataObstacle[] obstacles = pinataObstacles.GetComponentsInChildren<PinataObstacle>();

                for (int i = 0; i < obstacles.Length; i++)
                {
                    obstacles[i].Init(pinata.transform);
                }
            }

            if (SelectorLevels.GetLevels.GetPrefabLevelObstacles(Player.Level) != null)
            {
                levelObstacles = Instantiate(SelectorLevels.GetLevels.GetPrefabLevelObstacles(Player.Level), transform);
            }

            shooter.StartLevel(pinata.Target);
            foreach (Shooter shadow in shadowsInfo.Keys)
            {
                shadow.StartLevel(pinata.Target);
            }
            missedShells = 0;
            obstaclesHitShells = 0;
            OnStartLevel();
        }


        public void Transition()
        {
            HideBack();
            ShowBack(SelectorLevels.GetLevels.GetPrefabBack(Player.Level));
        }


        public void ChangePinata(Pinata newPinata)
        {
            Destroy(pinata.gameObject);
            rope.DestroyRope();

            newPinata.transform.parent = transform;
            pinata = newPinata;
            pinata.Init(anchor, placePinata.position, OnBreakPinata);
            rope.GenerateRope(pinata);

            shooter.FinishLevel();
            shooter.StartLevel(pinata.Target);

            foreach (Shooter shadow in shadowsInfo.Keys)
            {
                shadow.FinishLevel();
                shadow.StartLevel(pinata.Target);
            }
        }

        #endregion



        #region Private methods

        protected override void Hided(Result result)
        {
            base.Hided(result);

            Destroy(levelObstacles);
            Destroy(pinataObstacles);

            CurrentState = State.Playing;
        }


        private void OnOutOfAmmo()
        {
            if (CurrentState != State.Win)
            {
                CurrentState = State.OutOfAmmo;
            }
        }


        private void OnBreakPinata(uint coinsAmount)
        {
            CurrentState = State.Win;
            shooter.Coins += coinsAmount;
            shooter.FinishLevel();

            Destroy(pinataObstacles);

            UILevel.Prefab.Instance.Hide();
            StartCoroutine(FinishLevel(finishDelay));
        }


        private IEnumerator LoseCheck()
        {
            shooter.FinishLevel();

            yield return null;

            if (CurrentState == State.OutOfAmmo)
            {
                CurrentState = State.Lose;
                pinata.Leave(PinataLeft);
            }
        }


        private void PinataLeft(ITweener tw)
        {
            pinata.gameObject.SetActive(false);
            UILevel.Prefab.Instance.Hide();
          
            float coinsPrice = IngameCurrencySystemInstance.GetIngameCurrencyPrice(IngameCurrencyType.Coin);
            shooter.Coins += coinsPrice;

            float gemsPrice = IngameCurrencySystemInstance.GetIngameCurrencyPrice(IngameCurrencyType.Gem);
            Player.AddGems(gemsPrice);

            Hided(new Result
            {
                Win = false,
                Coins = shooter.Coins,
                MissedShells = missedShells,
                ObstaclesHitShells = obstaclesHitShells,
                IsForcedRestart = false
            });

            IngameCurrencySystemInstance.ResetIngameCurrenciesPrice();

            rope.DestroyRope();
        }


        private IEnumerator FinishLevel(float delay)
        {
            yield return new WaitForSeconds(delay);

            Hided(new Result
            {
                Win = true,
                Coins = shooter.Coins,
                MissedShells = missedShells,
                ObstaclesHitShells = obstaclesHitShells,
                IsForcedRestart = false
            });
        }

        private void OnShellSpawn()
        {
            shellCount++;
        }


        private void OnShellDestroy(Collider2D collision)
        {
            if (!collision.GetComponent<Pinata>())
            {
                missedShells++;
            }

            if (collision.GetComponent<Obstacle>())
            {
                obstaclesHitShells++;
            }

            shellCount--;

            if (CurrentState != State.OutOfAmmo || shellCount != 0) return;

            StartCoroutine(LoseCheck());
        }


        private void ShowBack(GameObject prefab)
        {
            if (!backs.ContainsKey(prefab))
            {
                backs[prefab] = Instantiate(prefab, transform);
            }

            GameObject back = backs[prefab];
            back.SetActive(true);
        }

        private void HideBack()
        {
            foreach (GameObject back in backs.Values)
            {
                back.SetActive(false);
            }
        }


        private void ChangeBack(GameObject back)
        {
            HideBack();
            ShowBack(back);
        }


        private void InitShadows()
        {
            List<ShadowInfo> shadows = ShooterShadowsConfig.GetShadowsInfo();

            if (shadows.Count == 0 && shadowsInfo.Count > 0)
            {
                for (int i = shadowsInfo.Count - 1; i >= 0; i--)
                {
                    Shooter shadowForRemove = shadowsInfo.ElementAt(i).Key;
                    shadowsInfo.Remove(shadowForRemove);
                    shadowForRemove.FinishLevel();
                    Destroy(shadowForRemove.gameObject);
                }
            }

            int shadowsCount = shadowsInfo.Count;

            for (int i = 0; i < shadows.Count; i++)
            {
                Shooter shadow = null;

                if (shadowsCount <= i)
                {
                    shadow = Instantiate(shooterShadowPrefab, shadowsTransformParent);
                    shadow.transform.localPosition = shooter.transform.localPosition;
                    shadowsInfo.Add(shadow, shadows[i]);
                    shadow.IsMovingDisabled = true;
                    EnableShadowMoving(shadow, ShooterShadowsConfig.GetShadowDelay(i));
                }
                else
                {
                    shadow = shadowsInfo.ElementAt(i).Key;
                    shadowsInfo[shadow] = shadows[i];
                }

                shadow.Init(null, shadows[i], i + 1);
                shadow.StartLevel(pinata.Target);
            }
        }


        private void EnableShadowMoving(Shooter shooterShadow, float delay)
        {
            Scheduler.Instance.CallMethodWithDelay(this, () =>
            {
                shooterShadow.IsMovingDisabled = false;
            }, delay);
        }


        private void RestartGame()
        {
            CurrentState = State.Lose;
            shooter.FinishLevel();
            Destroy(pinataObstacles);
            UILevel.Prefab.Instance.Hide();
            
            if (pinata != null)
            {
                rope.DestroyRope();
                pinata.Leave(PinataLeft);
            }
            Hided(new Result
            {
                Win = false,
                Coins = shooter.Coins,
                MissedShells = missedShells,
                ObstaclesHitShells = obstaclesHitShells,
                IsForcedRestart = true
            });
        }

        #endregion
    }
}

