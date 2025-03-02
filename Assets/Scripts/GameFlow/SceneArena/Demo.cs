using System.Collections;
using UnityEngine;
using DG.Tweening;


namespace PinataMasters
{
    public class Demo : MonoBehaviour
    {

        #region Variables

        [Header("Shooter")]
        [SerializeField]
        private Transform shooterTransform = null;
        [SerializeField]
        private Shooter shooter = null;
        [SerializeField]
        private ShooterBody shooterBody = null;
        [SerializeField]
        private ShooterLegs shooterLegs = null;
        [SerializeField]
        private float shootingRate = 0.2f;
        [SerializeField]
        private int weapon = 1;
        [SerializeField]
        private int skin = 1;
        [SerializeField]
        private float shooterAppearDelay = 1f;
        [SerializeField]
        private float scaleDuration = 1f;
        [SerializeField]
        private AnimationCurve scaleCurve = null;

        [Header("PinataRope")]
        [SerializeField]
        private Rope rope = null;
        [SerializeField]
        private Pinata pinataPrefab = null;
        [SerializeField]
        private Transform anchor = null;
        [SerializeField]
        private Transform placePinata = null;

        private Pinata pinata;
        private Coroutine shootCoroutine;

        #endregion



        #region Public methods

        public void Init()
        {
            pinata = Instantiate(pinataPrefab);
            pinata.Init(anchor, placePinata.position, delegate { });
            pinata.SetHealth(float.MaxValue);
            rope.GenerateRope(pinata);

            shooter.StartDemoLevel(pinata.Target);

            IngameCurrencySystem.Prefab.Instance.Configure(new IngameCurrencySystem.InitializationConfig()
            {
                collector = shooter,
                gravity = new Vector3(0f, -25f, 0f),
                borders = new Vector3(transform.position.x - 6.21f,
                                      transform.position.y - 6.51f, 
                                      transform.position.x + 6.21f)
            });

            shootCoroutine = StartCoroutine(Shot());

            StartCoroutine(SetDemoShooter());
        }


        public void DestroyPinata()
        {
            pinata.KillPinata();
            StartCoroutine(DestroyDemo());
            StopCoroutine(shootCoroutine);
        }

        #endregion



        #region Private methods

        private IEnumerator DestroyDemo()
        {
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }


        private IEnumerator Shot()
        {
            yield return new WaitForSeconds(shooterAppearDelay + scaleDuration);

            while (true)
            {
                shooterBody.DemoFire();

                yield return new WaitForSeconds(shootingRate);
            }
        }


        private IEnumerator SetDemoShooter()
        {
            yield return null;

            shooterBody.SetDemoWeapon(weapon);
            shooterBody.SetDemoBodySkin(skin);
            shooterLegs.SetDemoLegsSkin(skin);

            StartCoroutine(ShowShooter());
        }


        private IEnumerator ShowShooter()
        {
            yield return new WaitForSeconds(shooterAppearDelay);

            shooterTransform.localScale = Vector3.zero;
            shooterTransform.DOScale(Vector3.one, scaleDuration).SetEase(scaleCurve);
        }

        #endregion
    }
}
