using Modules.General.Obsolete;
using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace PinataMasters
{
    public class IngameCurrency : MonoBehaviour
    {
        #region Variables

        const float OffsetYForGroundDetection = 0.1f;

        [Header("Parameters")]
        [SerializeField] IngameCurrencyType currencyType;
        [SerializeField] float rotationsPerSecond = 1f;
        [SerializeField] float lifeTime = 3f;
        [SerializeField] Vector2 size = Vector2.zero;
        [SerializeField] float minScale = 0f;
        [SerializeField] float maxScale = 0f;

        [Header("Motion Settings")]
        [SerializeField] float bounciness = 0.4f;
        [SerializeField] float friction = 0f;

        [Header("Effects")]
        [SerializeField] Effect destroyEffect = null;

        [Header("Text")]
        [SerializeField] IngameCurrencyCollectText currencyTextPrefab;

        [Header("Sounds")]
        [SerializeField] AudioClip collectClip = null;
        [SerializeField] float collectVolume = 0f;

        [Header("Vibration")]
        [SerializeField] HapticTypes destroyVibrationType = HapticTypes.None;


        IngameCurrencySystem ingameCurrencySystem;


        Vector3 velocity;

        float angle;
        Vector3 baseScale;

        #endregion



        #region Properties

        public IngameCurrencyType CurrencyType => currencyType;

        public HapticTypes DestroyVibrationType => destroyVibrationType;

        public float Price { get; set; }

        public Rect Rect { get; private set; }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private void Update()
        {
            angle += 360f * Time.deltaTime * rotationsPerSecond;
            transform.localEulerAngles = new Vector3(0f, angle, 0f);
        }

        #endregion



        #region Public methods

        public void Init(IngameCurrencySystem system, float price, Vector3 impulse, bool isAutodestroyEnabled)
        {
            ingameCurrencySystem = system;

            Price = price;
            float scale = Random.Range(minScale, maxScale);
            transform.localScale = new Vector3(baseScale.x * scale, baseScale.y * scale, baseScale.z);
            Rect = new Rect(Vector2.zero, size * scale);

            velocity = impulse;

            ingameCurrencySystem.AddIngameCurrency(this);

            if (isAutodestroyEnabled)
            {
                StartCoroutine(Destroy());
            }
        }


        public void UpdateTransform(float deltaTime, Vector3 borders, Vector3 gravity)
        {
            velocity += gravity * deltaTime;

            Rect newRect = Rect;
            newRect.center = transform.position + velocity * Time.fixedDeltaTime;

            velocity.x = (newRect.xMin < borders.x && velocity.x < 0f) || (newRect.xMax > borders.z && velocity.x > 0f) ? -velocity.x : velocity.x;
            velocity.y = newRect.yMin < borders.y && velocity.y < 0f ? -velocity.y * bounciness : velocity.y;

            if (newRect.yMin < borders.y + OffsetYForGroundDetection)
            {
                velocity.x *= (1f - friction);
            }



            transform.position += velocity * Time.fixedDeltaTime;
            newRect.center = transform.position;

            Rect = newRect;
        }


        public void Collision()
        {
            EffectPlayer.Play(destroyEffect, transform.position);

            Release();

            ingameCurrencySystem.TryPlaySound(this);
            ingameCurrencySystem.TryPlayVibrationEffect(this);
        }


        public void ForceStartAutodestroy()
        {
            StartCoroutine(Destroy());
        }


        public void SpawnCollectText(float price, Vector3 position)
        {
            PoolManager.Instance.PoolForObject(currencyTextPrefab.gameObject).Pop((text) =>
            {
                IngameCurrencyCollectText ingameCurrencyCollectText = text.GetComponent<IngameCurrencyCollectText>();
                ingameCurrencyCollectText.Init(position, price);
            });
        }


        public void PlayCollectSound()
        {
            AudioManager.Instance.Play(collectClip, AudioType.Sound, collectVolume);
        }


        public void PlayVibrationEffect()
        {
            VibrationManager.Instance.PlayVibration(destroyVibrationType);
        }

        #endregion



        #region Private methods

        private IEnumerator Destroy()
        {
            yield return new WaitForSeconds(lifeTime);

            Release();
        }

        private void Release()
        {
            ingameCurrencySystem.RemoveIngameCurrency(this);
            gameObject.ReturnToPool();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, size);
        }
        #endregion
    }
}
