using DG.Tweening;
using Modules.Legacy.Tweens;
using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
    public class Pinata : MonoBehaviour
    {
        #region Variables

        public static event Action<Vector3> NeedHitAnnouncer = delegate { };
        public static event Action<Vector3> NeedCritAnnouncer = delegate { };
        public static event Action NeedPinataDestroyAnnouncer = delegate { };
        public static event Action OnPinataDead;
        public static event Action OnPinataDestroy = delegate { };
        public static event Action OnPinataLeaveTween = delegate { };
        public static event Action OnCollision = delegate { };

        public delegate void PinataBreak(uint finishCoinsAmount);
        public PinataBreak onBreak;

        private const int ANIMATION_INDEX = 0;
        private const uint COINS_CRIT_MULTIPLIER = 2u;

        [Header("Parameters")]
        [SerializeField]
        private Vector3 pinataPlaceOffset = Vector3.zero;
        [SerializeField]
        private Transform terget = null;
        [SerializeField]
        private float coinsSpawnTime = 1f;
        [SerializeField]
        private bool ignoreCoinsSpawnTime = false;
        [SerializeField]
        private float coinsPerSecond = 10f;

        [Header("PhysicsParameters")]
        [SerializeField]
        private float velocityBorder = 7f;
        [SerializeField]
        private float dragCoefficient = 0.2f;
        [SerializeField]
        private Vector2 impulseVector = Vector2.one;
        [SerializeField]
        private float minImpulseMultiplier = 0.5f;
        [SerializeField]
        private float maxImpulseMultiplier = 5.3f;
        [SerializeField]
        private float verticalPinataReactionCooldown = 0.1f;
        [SerializeField]
        private float horizontalPinataReactionCooldown = 0.1f;


        [Header("Tweens")]
        [SerializeField]
        private TweenPosition appearTween = null;
        [SerializeField]
        private TweenPosition leaveTween = null;
        [SerializeField]
        private TweenScale tweenScale = null;
        [SerializeField]
        private List<Transform> partsTransform = null;
        [SerializeField]
        private AnimationCurve partsCurve = null;

        [Header("Effects")]
        [SerializeField]
        private float squashCooldown = 0.1f;
        [SerializeField]
        private Effect hitEffect = null;
        [SerializeField]
        private Effect destroyEffect = null;
        [SerializeField]
        private Effect preDestroyEffect = null;

        [Header("Shake")]
        [SerializeField]
        [Range(0f, float.MaxValue)]
        private float magnitudeShakeDestroy = 0f;
        [SerializeField]
        private AnimationCurve curveShakeBeforeDestroy = null;
        [SerializeField]
        private AnimationCurve curveShakeAfterDestroy = null;
        [SerializeField]
        [Range(0f, float.MaxValue)]
        private float durationShakeAfterDestroy = 0f;

        [Header("Vibration")]
        [SerializeField]
        private HapticTypes hitVibrationType = HapticTypes.None;
        [SerializeField]
        private HapticTypes destroyVibrationType = HapticTypes.None;
        [SerializeField]
        private HapticTypes leaveVibrationType = HapticTypes.None;

        [Header("Sounds")]
        [SerializeField]
        private List<AudioClip> hitClips = null;
        [SerializeField]
        private float hitClipCooldown = 0.1f;
        [SerializeField]
        private AudioClip destroyClip = null;
        [SerializeField]
        private AudioClip preDestroyClip = null;
        [SerializeField]
        private AudioClip leaveClip = null;
        [SerializeField]
        private float leaveClipDelay = 0f;
        [SerializeField]
        private AudioClip appearClip = null;
        [SerializeField]
        private AudioClip appearLoopClip = null;

        [Header("Animation")]
        [SerializeField]
        private PinataEmotions pinataEmotions = null;

        private Transform anchorTransform;
        private IngameCurrencySpawner coinSpawner;
        private Rigidbody2D body;
        private SpringJoint2D spring;
        private float healthMax;
        private float health;

        private uint hitCount;
        private bool isFirstHitHappen;
        private bool isSquashAllow = true;
        private bool isHitSoundAllow = true;
        private bool isPinataReady;
        private bool allowPartsSquash = true;
        private bool allowHitReactionY = true;
        private bool allowHitReactionX = true;
        private Shake shake;

        #endregion



        #region Properties

        public Transform Target
        {
            get { return terget; }
        }


        public Vector3 Place { get; private set; }

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            tweenScale = tweenScale ?? GetComponent<TweenScale>();
            coinSpawner = GetComponent<IngameCurrencySpawner>();
            body = GetComponent<Rigidbody2D>();
            spring = GetComponent<SpringJoint2D>();
            healthMax = SelectorLevels.GetLevels.GetHealthPinata(Player.Level);
            health = healthMax;
        }


        private void Update()
        {
            if (!isPinataReady)
            {
                return;
            }

            if (body.velocity.magnitude > velocityBorder)
            {
                body.drag = body.velocity.magnitude * dragCoefficient;
            }

            Vector3 direction = anchorTransform.position - transform.position;
            float angle = Mathf.Atan2(-direction.x, direction.y) * Mathf.Rad2Deg;
            body.rotation = angle;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            Shell shell = collision.GetComponent<Shell>();
            if (!shell || health < 0f) return;

            OnCollision();

            PlayPartsTween();

            if (!shell.IsShadowShot)
            {
                VibrationManager.Instance.PlayVibration(hitVibrationType);
                EffectPlayer.Play(hitEffect, transform.position);
            }

            if (!isFirstHitHappen)
            {
                AudioManager.Instance.StopAmbient(appearLoopClip);
                isFirstHitHappen = true;
            }
            if (isHitSoundAllow)
            {
                int index = UnityEngine.Random.Range(0, hitClips.Count);
                AudioManager.Instance.Play(hitClips[index], AudioType.Sound, 0.5f);
                isHitSoundAllow = false;
                StartCoroutine(AllowHitSound());
            }

            if (isSquashAllow)
            {
                tweenScale.ResetTween();
                tweenScale.Play(true);

                isSquashAllow = false;
                StartCoroutine(AllowSquash());
            }

            float randomImpulceMultipler = UnityEngine.Random.Range(minImpulseMultiplier, maxImpulseMultiplier) * ShooterUpgradesBooster.asset.Value.BulletsImpulseMultiplier;
            randomImpulceMultipler *= shell.ImpulseMultiplier;

            if (allowHitReactionY)
            {
                body.AddForce(new Vector2(0f, shell.Direction.y) * impulseVector * shell.Force * randomImpulceMultipler, ForceMode2D.Impulse);
                allowHitReactionY = false;
                StartCoroutine(HitReactionYCooldown());
            }

            if (allowHitReactionX)
            {
                body.AddForce(new Vector2(shell.Direction.x, 0f) * impulseVector.x * shell.Force * randomImpulceMultipler, ForceMode2D.Impulse);
                allowHitReactionX = false;
                StartCoroutine(HitReactionXCooldown());
            }

            bool isCrit = UnityEngine.Random.value < Player.GetCritChance();
            float damage = isCrit ? shell.CritDamage : shell.Damage;
            uint coinsToSpawn = Arsenal.GetWeaponCoins(Player.CurrentWeapon) * (isCrit ? COINS_CRIT_MULTIPLIER : 1u);

            if (health > 0f)
            {
                if (ignoreCoinsSpawnTime)
                {
                    coinSpawner.SpawnIngameCurrency(coinsToSpawn, damage, IngameCurrencySpawner.Type.Speed, coinsPerSecond * (isCrit ? COINS_CRIT_MULTIPLIER : 1u));
                }
                else
                {
                    coinSpawner.SpawnIngameCurrency(coinsToSpawn, damage, IngameCurrencySpawner.Type.Time, coinsSpawnTime * (isCrit ? COINS_CRIT_MULTIPLIER : 1u));
                }
            }

            health -= damage;

            UILevel.Prefab.Instance.HealthPinata(health / healthMax);

            hitCount++;
            if (hitCount == 7 && !shell.IsShadowShot)
            {
                NeedHitAnnouncer(transform.position);
                hitCount = 0;
            }

            if (isCrit)
            {
                NeedCritAnnouncer(transform.position);
            }

            if (health < 0f)
            {
                DeathPinata();
            }

            if (!shell.IsShadowShot)
            {
                EffectPlayer.Play(shell.EffectPinata, transform.position, transform);
            }
        }

        #endregion



        #region Public methods

        public void Init(Transform anchor, Vector3 position, PinataBreak initOnBreak)
        {
            Place = position + pinataPlaceOffset;
            anchorTransform = anchor;
            transform.SetParent(anchor);
            transform.position = new Vector3(anchor.position.x, anchor.position.y + 1f, transform.position.z);

            appearTween.AddOnFinishedDelegate(AppearTweenCallback);
            appearTween.endPosition = new Vector3(0f, Place.y - anchor.transform.position.y, Place.z);
            appearTween.ResetTween();
            appearTween.Play(true);

            UILevel.Prefab.Instance.HealthPinata(health / healthMax);

            onBreak = initOnBreak;

            IngameCurrencySystem.Prefab.Instance.IsVibrationAllow = false;
            AudioManager.Instance.Play(appearClip, AudioType.Sound);
        }


        public void Leave(Modules.Legacy.Tweens.TweenCallback pinataLeft)
        {
            AudioManager.Instance.Play(leaveClip, AudioType.Sound, 1f, leaveClipDelay);
            pinataEmotions.PlayFailAnimation(() => LeavePinata(pinataLeft));
        }


        public void ConnectRopeEnd(Rigidbody2D endRB)
        {
            HingeJoint2D joint = gameObject.AddComponent<HingeJoint2D>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedBody = endRB;
            joint.anchor = Vector2.zero;
            joint.connectedAnchor = Vector2.zero;
        }


        public void KillPinata()
        {
            DeathPinata();
        }


        public void SetHealth(float healthToSet)
        {
            health = healthToSet; 
        }

        #endregion



        #region Private methods

        private void LeavePinata(Modules.Legacy.Tweens.TweenCallback pinataLeft)
        {
            OnPinataLeaveTween();

            isPinataReady = false;
            spring.enabled = false;

            leaveTween.SetOnFinishedDelegate(pinataLeft);
            leaveTween.beginPosition = transform.localPosition;
            leaveTween.endPosition = new Vector3(transform.position.x, 1f, transform.position.z);
            leaveTween.ResetTween();
            leaveTween.Play(true);

            tweenScale.ResetTween();
            tweenScale.Play(true);

            VibrationManager.Instance.PlayVibration(leaveVibrationType);
        }


        private void Destroy()
        {
            shake.Stop();
            OnPinataDestroy();

            IngameCurrencySystem.Prefab.Instance.IsVibrationAllow = true;

            VibrationManager.Instance.PlayVibration(destroyVibrationType);
            EffectPlayer.Play(destroyEffect, transform.position);
            AudioManager.Instance.Play(destroyClip, AudioType.PrioritySound, 2.5f);
            Shake.Play(Camera.main.transform, durationShakeAfterDestroy, magnitudeShakeDestroy, curveShakeAfterDestroy);
            NeedPinataDestroyAnnouncer();

            Destroy(gameObject);
        }


        private void AppearTweenCallback(ITweener tw)
        {
            body.velocity = Vector2.zero;
            isPinataReady = true;
            spring.enabled = true;
            spring.connectedAnchor = Place;

            tweenScale.ResetTween();
            tweenScale.Play(true);

            if (health > 0f)
            {
                pinataEmotions.PlayAppearAnimation();
            }

            if (!isFirstHitHappen)
            {
                AudioManager.Instance.Play(appearLoopClip, AudioType.Ambient);
            }
        }


        private IEnumerator AllowSquash()
        {
            yield return new WaitForSeconds(squashCooldown);

            isSquashAllow = true;
        }


        private IEnumerator AllowHitSound()
        {
            yield return new WaitForSeconds(hitClipCooldown);

            isHitSoundAllow = true;
        }


        private IEnumerator HitReactionYCooldown()
        {
            yield return new WaitForSeconds(verticalPinataReactionCooldown);

            allowHitReactionY = true;
        }


        private IEnumerator HitReactionXCooldown()
        {
            yield return new WaitForSeconds(horizontalPinataReactionCooldown);

            allowHitReactionX = true;
        }


        private IEnumerator PartsTweenCooldown()
        {
            yield return new WaitForSeconds(0.4f);

            allowPartsSquash = true;
        }


        private void PlayPartsTween()
        {

            if (!allowPartsSquash)
            {
                return;
            }

            for (int i = 0; i < partsTransform.Count; i++)
            {
                partsTransform[i].DOScaleX(2.2f, 0.4f).SetEase(partsCurve);
            }

            allowPartsSquash = false;
            StartCoroutine(PartsTweenCooldown());
        }


        private void DeathPinata()
        {
            OnPinataDead?.Invoke();

            GetComponent<Collider2D>().enabled = false;
            uint number = SelectorLevels.GetLevels.CoinsBase + SelectorLevels.GetLevels.CoinsUpgrate * Player.Level;
            uint amount = (uint)Mathf.RoundToInt(SelectorLevels.GetLevels.WinCoins(Player.Level));

            float duration = preDestroyEffect.GetComponent<ParticleSystem>().main.duration;
            coinSpawner.SpawnIngameCurrency(number, 0f, IngameCurrencySpawner.Type.Time, duration, callback: Destroy);
            shake = Shake.Play(Camera.main.transform, duration, magnitudeShakeDestroy, curveShakeBeforeDestroy);

            AudioManager.Instance.Play(preDestroyClip, AudioType.Sound, 0.7f);

            onBreak(amount);

            Instantiate(preDestroyEffect, transform.position, Quaternion.identity, transform);

            tweenScale.ResetTween();
            tweenScale.style = Style.PingPong;
            tweenScale.EndScale = new Vector3(1.3f, 0.8f, 1f);
            tweenScale.Play(true);

            pinataEmotions.PlayDestroyAnimation();
        }

        #endregion
    }
}
