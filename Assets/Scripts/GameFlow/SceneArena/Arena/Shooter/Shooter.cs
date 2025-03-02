using UnityEngine;
using System;


namespace PinataMasters
{
    public class Shooter : MonoBehaviour
    {
        #region Variables

        [Header("Parts")]
        [SerializeField]
        private ShooterBody shooterBody = null;
        [SerializeField]
        private ShooterLegs shooterLegs = null;
        [SerializeField]
        private Transform collectIngameCurrencyTransform = null;
        [SerializeField]
        private Vector2 size = Vector2.zero;

        public Vector3 Offset = Vector3.zero;

        [SerializeField]
        private ParticleEffect dustEffect;

        [Header("VisualParts")]
        [SerializeField]
        private MeshRenderer[] bodyRenderers;
        [SerializeField]
        private SpriteRenderer weaponRenderer;
        [SerializeField]
        private MeshRenderer handsRenderer;

        private ShadowInfo shadowInfo = null;

        #endregion



        #region Properties

        public float Coins { get; set; }

        [HideInInspector]
        public Rect Rect;

        public bool IsShadow { get { return shadowInfo != null; } }

        public ShadowInfo ShadowInfo { get { return shadowInfo; } }

        public bool IsMovingDisabled { get; set; }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            Rect.size = size;
        }


        private void OnDestroy()
        {
            Player.OnChangeSkin -= OnChangeSkin;
        }

        #endregion



        #region Public methods

        public void Init(Action InitOnOutOfAmmo = null, ShadowInfo info = null, int shadowNumber = 0)
        {
            shooterBody.Init(InitOnOutOfAmmo);
            shadowInfo = info;
            if (shadowInfo != null && dustEffect != null)
            {
                dustEffect.StopEffect();
            }
            SetShooterPartsOrders(shadowNumber);

            Player.OnChangeSkin += OnChangeSkin;
        }


        public void StartLevel(Transform initTarget)
        {
            shooterLegs.Target = initTarget;

            if (!IsShadow)
            {
                IngameCurrencySystem.Prefab.Instance.Configure(new IngameCurrencySystem.InitializationConfig()
                {
                    collector = this,
                    gravity = new Vector3(0f, -25f, 0f),
                    borders = new Vector3(-6.21f, -6.51f, 6.21f)
                });
            }

            Coins = 0;
            UILevel.Prefab.Instance.Coins(Coins);
            UILevel.Prefab.Instance.Level(Player.Level + 1);
            shooterBody.StartLevel(initTarget);
        }


        public void StartDemoLevel(Transform initTarget)
        {
            shooterLegs.Target = initTarget;
            Coins = 0;
            shooterBody.StartDemoLevel(initTarget);
        }


        public void FinishLevel()
        {
            shooterBody.FinishLevel();
        }


        public void Collision(IngameCurrencySystem system, IngameCurrency ingameCurrency)
        {
            switch (ingameCurrency.CurrencyType)
            {
                case IngameCurrencyType.Coin:
                    Coins += ingameCurrency.Price;
                    UILevel.Prefab.Instance.Coins(Coins);
                    break;

                case IngameCurrencyType.Gem:
                    if (!Mathf.Approximately(ingameCurrency.Price, 0f))
                    {
                        Player.AddGems(ingameCurrency.Price);
                    }
                    break;
            }

            system.TrySpawnCollectText(ingameCurrency, collectIngameCurrencyTransform.position);
        }

        #endregion



        #region Private methods

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + Vector3.Scale(Offset, transform.localScale), size);
        }


        private void SetShooterPartsOrders(int shooterIndex = 0)
        {
            int deltaSortingOrder = -shooterIndex * 3;
            for (int i = 0; i < bodyRenderers.Length; i++)
            {
                bodyRenderers[i].sortingOrder = deltaSortingOrder;
            }
            weaponRenderer.sortingOrder = deltaSortingOrder + 1;
            handsRenderer.sortingOrder = deltaSortingOrder + 2;
        }


        private void OnChangeSkin()
        {
            shooterBody.SetBodySkin();
            shooterLegs.SetLegsSkin();
        }

        #endregion
    }
}
