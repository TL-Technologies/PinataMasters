using MoreMountains.NiceVibrations;
using System;
using UnityEngine;


namespace PinataMasters
{
    public enum AimWeapon
    {
        Throw,
        Hands,
        Hip,
        RocketLauncher,
    }

    public static class AimWeaponExtension
    {
        public static bool IsThrow(this AimWeapon aimWeapon)
        {
            return aimWeapon == AimWeapon.Throw;
        }
    }

    [Serializable]
    public struct WeaponParameters
    {
        public Sprite View;
        public AimWeapon Aim;

        [Header("Characteristics")]
        public Shell PrefabShell;
        public float TimeReload;
        public uint SizeMagazine;
        public float RateOfFire;
        public float DoubleShootDelay;
        public bool HasSpray;
        [ConditionalHide("HasSpray")]
        public float Spray;

        [Header("Effects")]
        public Effect ShootEffect; 

        [Header("Vibration")]
        public HapticTypes MagazineVibrationType;
        public HapticTypes ShellVibrationType;

        [Header("Sounds")]
        public AudioClip ShotClip;
        public float ShotClipVolume;
        public AudioClip MagazineClip;
        public AudioClip ReloadClip;
        public AudioClip PassiveClip;
        public float PassiveClipVolume;
    }
    
    [Serializable]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField]
        public string Desc;

        public float Price;

        [Header("Sprites")]
        public Sprite Sprite;
        public Sprite Mask;
        public ParticleSystem UnlockEffect;

        [Header("Ammo")]
        public uint AmmoBase;
        public uint AmmoUpgrade;
        public float AmmoUpgradePrice;

        [Header("Damage")]
        public float DamageBase;
        public float DamageUpgrade;
        public float DamageUpgradePrice;

        [Header("Coins")]
        public uint CoinsBase;
        public uint CoinsUpgrate;

        [Header("AutoTap")]
        public bool CanAutoShoot;
        public float AutoShootCooldown;

        [Header("ShadowOptions")]
        public float shotsPercents;
        public float impulseMultiplier;

        public WeaponParameters Parameters;
    }
}
