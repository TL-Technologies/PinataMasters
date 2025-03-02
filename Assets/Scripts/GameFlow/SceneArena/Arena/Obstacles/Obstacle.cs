using UnityEngine;
using Modules.Legacy.Tweens;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;


namespace PinataMasters
{
    public class Obstacle : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private AnimationCurve appearCurve = null;
        [SerializeField]
        private float appearDuration = 0f;
        [SerializeField]
        private float appearDelay = 0f;

        [SerializeField]
        private AudioClip hitClip = null;
        [SerializeField]
        private float hitClipCooldown = 0.1f;

        [SerializeField]
        private List<SpriteMask> damageMasks = null;
        [SerializeField]
        private ObstacleEffect obstacleEffect = null;

        private TweenScale tween;
        private TrailRenderer trail;
        private bool isHitSoundAllow = true;

        private int activeDamageMasksCount;
        private float healthDamageSegment;
        private float maxHealth;

        private IngameCurrencySpawner coinSpawner;

        #endregion



        #region Properies

        protected float Health { get; set; }

        private TweenScale Tween
        {
            get
            {
                if (tween == null)
                {
                    tween = GetComponent<TweenScale>();
                }
                return tween;
            }
        }

        #endregion



        #region Unity lifecycle

        protected virtual void Start()
        {
            coinSpawner = GetComponent<IngameCurrencySpawner>();
            trail = GetComponent<TrailRenderer>();
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            transform.localScale = Vector3.zero;
            transform.DOScale(Vector3.one, appearDuration).SetDelay(appearDelay).SetEase(appearCurve).OnComplete(TweenScaleCallback);

            if (trail != null)
            {
                float maxTrailWidth = trail.widthMultiplier;
                trail.widthMultiplier = 0f;
                StartCoroutine(AppearTrail(maxTrailWidth));
            }

            maxHealth = Health;

            if (damageMasks.Count != 0)
            {
                healthDamageSegment = maxHealth / damageMasks.Count;
            }
        }

        #endregion



        #region Private methods

        public void OnCollision(Shell shell)
        {
            float damage = shell.Damage * Player.GetAdditionalObstacleDamage();

            float damagePartHealth = Mathf.Clamp(damage / maxHealth, 0f, 1f);
            obstacleEffect.SetMaxParticlesPartEffect(damagePartHealth);

            if (!shell.IsShadowShot)
            {
                EffectPlayer.Play(obstacleEffect, shell.transform.position);
            }

            float priceToSpawn = Health - damage > 0f ? damage * Player.GetObstacleCoins() : Health * Player.GetObstacleCoins();

            Health -= damage;

            if (!Mathf.Approximately(priceToSpawn, 0f))
            {
                coinSpawner.SpawnIngameCurrency(Arsenal.GetWeaponCoins(Player.CurrentWeapon), priceToSpawn, IngameCurrencySpawner.Type.Time, 0.1f, callback: Health < 0f ? () => Destroy(gameObject) : (System.Action)null);
            }
            else if (Health < 0f)
            {
                Destroy(gameObject);
            }

            if (damageMasks.Count != 0 && Health < maxHealth - activeDamageMasksCount * healthDamageSegment)
            {
                for (int i = 0; i <= (maxHealth - Health - activeDamageMasksCount * healthDamageSegment) / healthDamageSegment + 1; i++)
                {
                    if (damageMasks.Count > 0)
                    {
                        activeDamageMasksCount++;
                        SpriteMask mask = FindNearestToShellMask(shell.transform.position);
                        mask.enabled = true;
                        damageMasks.Remove(mask);
                    }
                }
            }

            if (Tween != null)
            {
                Tween.ResetTween();
                Tween.Play(true);
            }

            if (isHitSoundAllow)
            {
                AudioManager.Instance.Play(hitClip, AudioType.Sound);
                isHitSoundAllow = false;
                StartCoroutine(AllowHitSound());
            }
        }

        #endregion



        #region Private methods

        protected virtual void TweenScaleCallback() { }


        private IEnumerator AllowHitSound()
        {
            yield return new WaitForSeconds(hitClipCooldown);

            isHitSoundAllow = true;
        }


        private IEnumerator AppearTrail(float maxWidth)
        {
            yield return new WaitForSeconds(appearDelay);

            float time = 0f;

            while (time < appearDuration)
            {
                trail.widthMultiplier = Mathf.Lerp(0f, maxWidth, time);

                time += Time.deltaTime;

                yield return null;
            }

            trail.widthMultiplier = maxWidth;
        }


        SpriteMask FindNearestToShellMask(Vector3 shellPosition)
        {
            SpriteMask spriteMask = null;
            float minSqrMagnitude = float.MaxValue;

            for (int i = 0, count = damageMasks.Count; i < count; i++)
            {
                float currentSqrMagnitude = (damageMasks[i].transform.position - shellPosition).sqrMagnitude;

                if (currentSqrMagnitude < minSqrMagnitude)
                {
                    minSqrMagnitude = currentSqrMagnitude;
                    spriteMask = damageMasks[i];
                }
            }

            return spriteMask;
        }

        #endregion
    }
}
