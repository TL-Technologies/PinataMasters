using Modules.General;
using System;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PinataMasters
{
    public class ShooterBody : MonoBehaviour
    {
        #region Variables

        public static event Action<int> OnShotRelease;

        public static event Action OnOutOfAmmo;

        private const int WALK_INDEX = 0;
        private const int AIM_INDEX = 1;
        private const int FIREARMSHOT_INDEX = 2;
        private const int BODY_SEPARATOR_SLOTS_COUNT = 1;
        private const string COSMONAUT_SKULL_BONE_NAME = "skull";

        [SerializeField]
        private Shooter shooter = null;

        [Header("Animation")]
        [SerializeField]
        private SkeletonAnimation body = null;

        [SerializeField][SpineAnimation(dataField: "body")]
        private string walkBody = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string aimShoulder = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string aimArm = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string aimHead = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string aimRoket = null;


        [SerializeField][SpineAnimation(dataField: "body")]
        private string shotThrowing = null;
        [SerializeField][SpineAnimation(dataField: "body")]
        private string shotFirearms = null;

        [Header("Weapons")]
        [SerializeField]
        private Transform hip = null;
        [SerializeField]
        private Transform hands = null;
        [SerializeField]
        private Transform throwing = null;

        [SerializeField]
        private SpriteRenderer viewWeapon = null;

        private string currentAimAnimation;

        private TrackEntry tracker;

        private Weapon currentWeapon;
        private uint ammo;
        private Transform target;

        private float autoShootCooldown;
        private float timeForAutoShoot;

        private Coroutine autoShootCorotine;
        private int counterForDoubleShot;
        private int lastShadowShotNumber = -1;

        #endregion



        #region Properties

        private int currentWeaponNumber { get { return (shooter.IsShadow) ? (shooter.ShadowInfo.weapon) : (Player.CurrentWeapon); } }

        #endregion



        #region Unity lifecycle

        private void Start()
        {
            SetWeapon();
            SetBodySkin(true);

            Player.OnChangeWeapon += SetWeapon;
        }


        private void OnDestroy()
        {
            Player.OnChangeWeapon -= SetWeapon;
        }


        private void Update()
        {
            if (currentWeapon != null)
            {
                Vector3 aimDirection = target ? target.position - GetAimTransform(currentWeapon.Parameters.Aim).position : Vector3.up;
                float angle = Mathf.Atan2(-aimDirection.x, aimDirection.y) * Mathf.Rad2Deg;
                currentWeapon.transform.eulerAngles = new Vector3(0f, 0f, angle);
                tracker.TrackTime = Mathf.Abs(angle) / 90f * body.Skeleton.Data.FindAnimation(currentAimAnimation).Duration;
            }
        }

        #endregion



        #region Public methods

        public void Init(Action initOnOutOfAmmo = null)
        {
            OnOutOfAmmo += initOnOutOfAmmo;
        }


        public void StartLevel(Transform initTarget)
        {
            target = initTarget;
            ammo = (Arsenal.GetWeaponMaxAmmo(Player.CurrentWeapon));
            if (!shooter.IsShadow)
            {
                UILevel.Prefab.Instance.Ammo(ammo);
            }

            if (shooter.IsShadow)
            {
                ShooterBody.OnShotRelease += TryShadowFire;
            }
            else
            {
                TapZone.OnTap += TryFire;

                if (Arsenal.CanWeaponAutoShoot(currentWeaponNumber))
                {
                    autoShootCooldown = Arsenal.GetWeaponAutoShootСooldown(currentWeaponNumber);
                    TapZone.OnTap += StartAutoShoot;
                    TapZone.OnUp += FinishAutoShoot;
                }
            }
        }


        public void StartDemoLevel(Transform initTarget)
        {
            target = initTarget;
            ammo = uint.MaxValue;
        }


        public void FinishLevel()
        {
            if (shooter.IsShadow)
            {
                ShooterBody.OnShotRelease -= TryShadowFire;
            }
            else
            {
                TapZone.OnTap -= TryFire;

                if (Arsenal.CanWeaponAutoShoot(currentWeaponNumber))
                {
                    FinishAutoShoot();
                    TapZone.OnTap -= StartAutoShoot;
                    TapZone.OnUp -= FinishAutoShoot;
                }
            }
        }


        public void DemoFire()
        {
            ammo = 999;
            TryFire();
        }


        public void SetDemoWeapon(int weapon)
        {
            Destroy(hip.GetComponent<Weapon>());
            Destroy(throwing.GetComponent<Weapon>());
            Destroy(hands.GetComponent<Weapon>());

            currentWeapon = GetAimTransform(Arsenal.GetWeaponParameters(weapon).Aim).gameObject.AddComponent<Weapon>();
            currentWeapon.SetParameters(weapon, isDemo: true);

            viewWeapon.sprite = currentWeapon.Parameters.View;

            SetAimAnimation();
        }


        public void SetDemoBodySkin(int skin)
        {
            body.skeletonDataAsset = Skins.GetBodySkeletonAsset(skin);
            body.Initialize(true);

            body.AnimationState.SetAnimation(WALK_INDEX, walkBody, true);
            SetAimAnimation();
        }


        public void SetBodySkin(bool isForced = false)
        {
            SkeletonDataAsset newSkeletonDataAsset = Skins.GetBodySkeletonAsset((shooter.IsShadow) ? (shooter.ShadowInfo.skin) : (Player.CurrentSkin));

            if (newSkeletonDataAsset && (newSkeletonDataAsset != body.skeletonDataAsset || isForced))
            {
                body.skeletonDataAsset = newSkeletonDataAsset;
                body.Initialize(true);

                //cosmonaut crutch
                SkeletonRenderSeparator skeletonRenderSeparator = body.GetComponent<SkeletonRenderSeparator>();

                if (body.Skeleton.FindBone(COSMONAUT_SKULL_BONE_NAME) != null)
                {
                    if (body.separatorSlots.Count == BODY_SEPARATOR_SLOTS_COUNT)
                    {
                        List<string> slotsNames = new List<string>();
                        slotsNames.Add(body.SeparatorSlotNames.FirstObject());
                        slotsNames.Add(COSMONAUT_SKULL_BONE_NAME);
                        body.SeparatorSlotNames = slotsNames.ToArray();
                        skeletonRenderSeparator.AddPartsRenderer();
                        body.Initialize(true);
                        SkeletonPartsRenderer addedPart = skeletonRenderSeparator.partsRenderers.LastObject();
                        addedPart.MeshRenderer.sortingOrder = skeletonRenderSeparator.partsRenderers.FirstObject().MeshRenderer.sortingOrder;
                        addedPart.gameObject.transform.localPosition = Vector3.back;
                    }
                }
                else if (body.separatorSlots.Count > BODY_SEPARATOR_SLOTS_COUNT)
                {
                    List<string> slotsNames = new List<string>();
                    slotsNames.Add(body.SeparatorSlotNames.FirstObject());
                    body.SeparatorSlotNames = slotsNames.ToArray();
                    body.separatorSlots.RemoveAt(body.separatorSlots.Count - 1);
                    SkeletonPartsRenderer removedPart = skeletonRenderSeparator.partsRenderers.LastObject();
                    skeletonRenderSeparator.partsRenderers.RemoveAt(skeletonRenderSeparator.partsRenderers.Count - 1);
                    Destroy(removedPart.gameObject);
                    body.Initialize(true);
                }
            }
            ShooterShadowsConfig.SetAnimationShader(body, shooter.IsShadow);
            body.AnimationState.SetAnimation(WALK_INDEX, walkBody, true);
            SetAimAnimation();
        }

        #endregion



        #region Private methods

        private IEnumerator CheckAutoShoot()
        {
            while (true)
            {
                if (timeForAutoShoot > autoShootCooldown && currentWeapon.IsAutoShootReady && currentWeapon.IsReady)
                {
                    TryFire();
                    timeForAutoShoot = 0f;
                }
                else
                {
                    timeForAutoShoot += Time.deltaTime;
                }

                yield return null;
            }
        }


        private void FinishAutoShoot()
        {
            if (autoShootCorotine != null)
            {
                StopCoroutine(autoShootCorotine);
                autoShootCorotine = null;
            }
        }


        private void StartAutoShoot()
        {
            timeForAutoShoot = 0f;
            if (!shooter.IsShadow && autoShootCorotine == null)
            {
                autoShootCorotine = StartCoroutine(CheckAutoShoot());
            }
        }


        private void TryFire()
        {
            if (!shooter.IsShadow && currentWeapon.IsReady && ammo > 0u)
            {
                SetFireAnimation();
                counterForDoubleShot = ammo == 1u ? counterForDoubleShot : counterForDoubleShot + 1;
                bool isDoubleShot = (counterForDoubleShot % Player.GetDoubleShotRate() == 0) && (ammo > 1u) && !shooter.IsShadow;
                currentWeapon.Fire(target, isDoubleShot);

                ammo = isDoubleShot ? ammo - 2u : ammo - 1u;
                UILevel.Prefab.Instance.Ammo(ammo);

                if (ammo == 0 && OnOutOfAmmo != null)
                {
                    OnOutOfAmmo();
                }
                if (OnShotRelease != null)
                {
                    OnShotRelease((int)ammo);
                }
            }
        }


        private void TryShadowFire(int shooterAmmo)
        {
            if (currentWeapon != null && currentWeapon.IsReady && lastShadowShotNumber != shooterAmmo && ShooterShadowsConfig.IsNeedreleaseShot(shooter.ShadowInfo.weapon, (int)shooterAmmo, (int)Arsenal.GetWeaponMaxAmmo(Player.CurrentWeapon)))
            {
                lastShadowShotNumber = shooterAmmo;

                Scheduler.Instance.CallMethodWithDelay(this, () =>
                {
                    if (currentWeapon != null)
                    {
                        SetFireAnimation();
                        currentWeapon.Fire(target, false);
                    }
                }, ShooterShadowsConfig.GetShadowShotDelay());
            }
        }


        private void SetWeapon()
        {
            Destroy(hip.GetComponent<Weapon>());
            Destroy(throwing.GetComponent<Weapon>());
            Destroy(hands.GetComponent<Weapon>());

            if (currentWeaponNumber >= 0)
            {
                currentWeapon = GetAimTransform(Arsenal.GetWeaponParameters(currentWeaponNumber).Aim).gameObject.AddComponent<Weapon>();

                if (shooter.IsShadow)
                {
                    currentWeapon.SetParameters(currentWeaponNumber, ShooterShadowsConfig.GetShadowsBulletsImpulseDenominationValue(currentWeaponNumber));
                }

                viewWeapon.sprite = currentWeapon.Parameters.View;
                viewWeapon.material = ShooterShadowsConfig.SetSpriteShader(viewWeapon.material, shooter.IsShadow);

                SetAimAnimation();
            }
            else
            {
                viewWeapon.sprite = null;
            }
        }


        private Transform GetAimTransform(AimWeapon aim)
        {
            switch (aim)
            {
                case AimWeapon.Hip:
                case AimWeapon.RocketLauncher:
                    return hip;

                case AimWeapon.Throw:
                    return throwing;

                case AimWeapon.Hands:
                    return hands;

                default:
                    throw new Exception("Unknow weapon type");
            }
        }


        private void SetAimAnimation()
        {
            if (currentWeapon == null)
            {
                return;
            }

            body.AnimationState.ClearTrack(AIM_INDEX);
            body.AnimationState.ClearTrack(FIREARMSHOT_INDEX);

            switch (currentWeapon.Parameters.Aim)
            {
                case AimWeapon.Hip:
                    currentAimAnimation = aimShoulder;
                    break;

                case AimWeapon.Throw:
                    currentAimAnimation = aimHead;
                    break;

                case AimWeapon.Hands:
                    currentAimAnimation = aimArm;
                    break;
                
                case AimWeapon.RocketLauncher:
                    currentAimAnimation = aimRoket;
                    break;
            }

            tracker = body.AnimationState.SetAnimation(AIM_INDEX, currentAimAnimation, false);
        }


        private void SetFireAnimation()
        {
            string shotAnimation = currentWeapon.Parameters.Aim.IsThrow() ? shotThrowing : shotFirearms;
            int index = currentWeapon.Parameters.Aim.IsThrow() ? AIM_INDEX : FIREARMSHOT_INDEX;
            TrackEntry shotTracker = body.AnimationState.SetAnimation(index, shotAnimation, false);
            shotTracker.Complete += EndFireAnimation;
        }


        private void EndFireAnimation(TrackEntry shotTracker)
        {
            SetAimAnimation();
            shotTracker.Complete -= EndFireAnimation;
        }

        #endregion
    }
}
