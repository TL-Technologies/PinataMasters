using Modules.General.Obsolete;
using MoreMountains.NiceVibrations;
using System;
using UnityEngine;

namespace PinataMasters
{
    public class Shell : MonoBehaviour
    {
        #region Variables

        public static event Action OnShellSpawn = delegate { };
        public static event Action<Collider2D> OnShellDestroy = delegate { };


        private const float REDUCE_SHELL_ANGLE = 55f;
        private const uint CRIT_DAMAGE_MULTIPLIER = 2u;

        [Header("Parameters")]
        [SerializeField]
        private float speed = 1f;
        [SerializeField]
        private float force = 1f;
        [SerializeField]
        private float rotationsPerSecond = 0f;
        [SerializeField]
        private float acceleration = 0f;

        [Header("Sprite")]
        [SerializeField]
        private GameObject sprite = null;

        [Header("Effects")]
        [SerializeField]
        private TrailRenderer trail = null;
        [SerializeField] 
        private ArrayGradients trailGradients = null;

        [SerializeField]
        private TrailEffect trailEffect = null;

        [SerializeField]
        private Effect destroyEffect = null;
        [SerializeField]
        private Effect pinataEffect = null;
        [SerializeField]
        private HapticTypes destroyVibrationType = HapticTypes.None;

        [Header("Sounds")]
        [SerializeField]
        private AudioClip hit = null;

        private float angle;
        private float currentSpeed;
        private bool isCollisionHappened;

        private TrailEffect currentTrailEffect;
        private ObjectPool poolForTrailEffect;

        private Transform pinata;

        private bool shouldSetPerfectDirection;

        #endregion



        #region Properties

        public float Damage { get; private set; }

        public float CritDamage { get; private set; }

        public float Force { get { return force; } }

        public Vector3 Direction { get; private set; }

        public Effect EffectPinata => pinataEffect;

        public TrailEffect TrailEffectPrefab { get { return trailEffect; } }

        public float ImpulseMultiplier { get; private set; }

        public bool IsShadowShot { get; set; }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            poolForTrailEffect = trailEffect ? PoolManager.Instance.PoolForObject(trailEffect.gameObject) : null;
        }


        private void Update()
        {
            if (pinata && transform.position.y < pinata.position.y)
            {
                Vector3 perfectDirection = (pinata.position - transform.position).normalized;

                if (shouldSetPerfectDirection)
                {
                    Direction = perfectDirection;
                }
                else
                {
                    float directionsAngle = Vector3.Angle(perfectDirection, Direction);
                    Vector3 cross = Vector3.Cross(perfectDirection, Direction);

                    float angleToRotate = directionsAngle > REDUCE_SHELL_ANGLE ? REDUCE_SHELL_ANGLE : directionsAngle;
                    angleToRotate = cross.z > 0 ? -angleToRotate : angleToRotate;
                    Direction = Quaternion.Euler(0f, 0f, angleToRotate * Time.deltaTime) * Direction;
                }
            }

            transform.Translate(Direction * Time.deltaTime * currentSpeed);

            currentSpeed += acceleration * Time.deltaTime;

            if (!Mathf.Approximately(rotationsPerSecond, 0))
            {
                angle += 360f * Time.deltaTime * rotationsPerSecond;
                sprite.transform.localEulerAngles = new Vector3(0f, 0f, angle);
            }
            else
            {
                sprite.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.Angle(Direction, Vector2.up) * -Mathf.Sign(Direction.x));
            }

            if (currentTrailEffect != null)
            {
                currentTrailEffect.transform.position = transform.position;
            }
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isCollisionHappened)
            {
                return;
            }

            isCollisionHappened = true;

            OnShellDestroy(collision);

            VibrationManager.Instance.PlayVibration(destroyVibrationType);

            EffectPlayer.Play(destroyEffect, transform.position);

            if (collision.GetComponent<Pinata>())
            {
                AudioManager.Instance.Play(hit, AudioType.Sound);
                DestroyShell();
                return;
            }


            Obstacle obstacle = collision.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.OnCollision(this);
                AudioManager.Instance.Play(hit, AudioType.Sound);
            }

            DestroyShell();
        }

        #endregion



        #region Public methods

        public void Init(Vector3 position, Vector3 initDirection, float damage, float impulseMultiplier, uint level, Transform target, bool canWeaponSpray, bool isShadowShot)
        {
            OnShellSpawn();

            isCollisionHappened = false;

            currentSpeed = speed;
            sprite.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.Angle(initDirection, Vector2.up) * -Mathf.Sign(initDirection.x));

            transform.position = position;
            Direction = initDirection;
            Damage = damage;
            IsShadowShot = isShadowShot;
            ImpulseMultiplier = impulseMultiplier;
            CritDamage = damage * CRIT_DAMAGE_MULTIPLIER;
            pinata = target;
            shouldSetPerfectDirection = !canWeaponSpray;

            if (trailEffect != null)
            {
                currentTrailEffect = poolForTrailEffect.Pop().GetComponent<TrailEffect>();
                currentTrailEffect.transform.position = position;
                currentTrailEffect.Init(trailGradients.Gradients[level % trailGradients.Gradients.Length]);
            }

            if (trail != null)
            {
                trail.Clear();
                trail.colorGradient = trailGradients.Gradients[level % trailGradients.Gradients.Length];
            }
        }

        #endregion



        #region Private methods

        private void DestroyShell()
        {
            if (currentTrailEffect != null)
            {
                currentTrailEffect.DisableAfterDelay();
            }

            gameObject.ReturnToPool();
        }

        #endregion
    }
}
