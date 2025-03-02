using Modules.General.Obsolete;
using MoreMountains.NiceVibrations;
using System.Collections;
using UnityEngine;

namespace PinataMasters
{
    public class Weapon : MonoBehaviour
    {
        #region Variables

        private const float DOUBLE_SHOT_FIRE_RATE_MULTIPLIER = 0.5f;

        private PoolManager poolManager;
        private ObjectPool poolForObject;

        private WaitForSeconds fireRate;
        private WaitForSeconds fireRateDoubleShot;
        private WaitForSeconds reloadTime;
        private static WaitForSeconds waitVibration = new WaitForSeconds(0.1f);

        private bool isVibrationAllow = true;
        private float reloadMultiplier = 1.0f;

        #endregion



        #region Properties

        public bool IsReady { get; private set; } = true;

        public bool IsAutoShootReady { get; private set; } = true;
        
        public WeaponParameters Parameters { get; private set; }

        private int WeaponNumber { get; set; } = 0;

        private float ImpulseMultiplier { get; set; } = 1.0f;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            SetParameters(Player.CurrentWeapon);

            BaseBooster.OnBoosterStateChanged += BaseBooster_OnBoosterStateChanged;
        }


        private void Start()
        {
            AudioManager.Instance.Play(Parameters.PassiveClip, AudioType.Ambient, Parameters.PassiveClipVolume);
        }


        private void OnDestroy()
        {
            if (AudioManager.Instance)
            {
                AudioManager.Instance.StopAmbient(Parameters.PassiveClip);
            }

            BaseBooster.OnBoosterStateChanged -= BaseBooster_OnBoosterStateChanged;
        }


        #if UNITY_EDITOR
        private void Update()
        {
            Debug.DrawRay(transform.position, (transform.up * transform.localScale.x * Mathf.Cos(Mathf.Deg2Rad * Parameters.Spray) + transform.right * Mathf.Sin(Mathf.Deg2Rad * Parameters.Spray)) * 20f);
            Debug.DrawRay(transform.position, (transform.up * transform.localScale.x * Mathf.Cos(Mathf.Deg2Rad * -Parameters.Spray) + transform.right * Mathf.Sin(Mathf.Deg2Rad * -Parameters.Spray)) * 20f);
        }
        #endif

        #endregion



        #region Public methods

        public void Fire(Transform target, bool doubleShot)
        {
            StartCoroutine(ProduceShot(target, doubleShot));
        }


        public void SetParameters(int weapon, float impulseMultiplier = 1.0f, bool isDemo = false)
        {
            poolManager = PoolManager.Instance;
            Parameters = Arsenal.GetWeaponParameters(weapon);
            poolForObject = poolManager.PoolForObject(Parameters.PrefabShell.gameObject);
            WeaponNumber = (isDemo) ? (Player.CurrentWeapon) : (weapon);
            ImpulseMultiplier = impulseMultiplier;

            reloadMultiplier = ShooterUpgradesBooster.asset.Value.TimeReloadMultiplier; 

            fireRate = new WaitForSeconds(Parameters.RateOfFire * reloadMultiplier);
            fireRateDoubleShot = new WaitForSeconds(Parameters.RateOfFire * DOUBLE_SHOT_FIRE_RATE_MULTIPLIER * reloadMultiplier);
            reloadTime = new WaitForSeconds(Parameters.TimeReload * ShooterUpgradesBooster.asset.Value.TimeReloadMultiplier * reloadMultiplier);
        }

        #endregion



        #region Privater methods

        private IEnumerator ProduceShot(Transform target,bool doubleShot)
        {
            IsReady = false;
            IsAutoShootReady = false;

            var e = doubleShot ? fireRateDoubleShot : fireRate;
            yield return Shot(target, e);

            if (doubleShot)
            {
                yield return new WaitForSeconds(Parameters.DoubleShootDelay * reloadMultiplier);
                yield return Shot(target, e);
            }

            IsAutoShootReady = true;
            yield return reloadTime;

            IsReady = true;
        }

        private IEnumerator Shot(Transform target, WaitForSeconds rate)
        {
            yield return null;

            int i = (int)Parameters.SizeMagazine;
            InitShell(target);
            i--;

            EffectPlayer.Play(Parameters.ShootEffect, transform.position, transform);
            PlayVibration(Parameters.MagazineVibrationType);

            AudioManager.Instance.Play(Parameters.MagazineClip, AudioType.Sound);

            while (i > 0)
            {
                yield return rate;

                int shellPerFrame = Mathf.CeilToInt(Time.deltaTime / Parameters.RateOfFire);
                for (int j = 0; j < shellPerFrame && i > 0; j++, i--)
                {
                    InitShell(target);
                }
            }

            AudioManager.Instance.Play(Parameters.ReloadClip, AudioType.Sound);
        }


        private void InitShell(Transform target)
        {
            poolForObject.Pop((shell) =>
            {
                float damage = Arsenal.GetWeaponDamage(WeaponNumber) / Parameters.SizeMagazine;
                Shooter shooter = GetComponentInParent<Shooter>();
                if (shooter != null)
                {
                    float shadowDamageCoef = Arsenal.GetWeaponConfig(WeaponNumber).shotsPercents;
                    if (shooter.IsShadow && shadowDamageCoef > float.Epsilon)
                    {
                        damage /= shadowDamageCoef;
                    }
                }
                float sprayAngle = Parameters.HasSpray ? Mathf.Deg2Rad * Random.Range(-Parameters.Spray, Parameters.Spray) : 0f;
                Vector3 direction = (transform.up * transform.localScale.x * Mathf.Cos(sprayAngle) + transform.right * Mathf.Sin(sprayAngle)).normalized;
                shell.GetComponent<Shell>().Init(transform.position, direction, damage, ImpulseMultiplier, Player.GetDamageLevel(WeaponNumber), target, Parameters.HasSpray, shooter.IsShadow);
            });

            EffectPlayer.Play(Parameters.ShootEffect, transform.position, transform);
            PlayVibration(Parameters.ShellVibrationType);
            AudioManager.Instance.Play(Parameters.ShotClip, AudioType.Sound, Parameters.ShotClipVolume);
        }


        private void PlayVibration(HapticTypes type)
        {
            if (isVibrationAllow)
            {
                isVibrationAllow = false;
                VibrationManager.Instance.PlayVibration(type);
                StartCoroutine(VibrationCooldown());
            }
        }


        private IEnumerator VibrationCooldown()
        {
            yield return waitVibration;

            isVibrationAllow = true;
        }


        void BaseBooster_OnBoosterStateChanged(BoosterType boosterType, BoosterState state)
        {
            if (boosterType == BoosterType.ShooterUpgrades)
            {
                SetParameters(WeaponNumber, ImpulseMultiplier);
            }
        }

        #endregion
    }
}
