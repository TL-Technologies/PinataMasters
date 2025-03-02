using Modules.General.HelperClasses;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class Arsenal : ScriptableObject
    {
        #region Fields

        private static readonly ResourceAsset<Arsenal> asset = new ResourceAsset<Arsenal>("Game/Arsenal");
        
        [SerializeField][ResourceLink] List<AssetLink> weaponsAssetLinks = null;

        static Dictionary<int, WeaponConfig> loadedWeapons = new Dictionary<int, WeaponConfig>();

        #endregion



        #region Properties

        public List<AssetLink> WeaponsAssetLinks => weaponsAssetLinks;
        

        public static int Count
        {
            get { return asset.Value.weaponsAssetLinks.Count; }
        }

        #endregion



        #region Public methods

        public static string GetWeaponDesc(int weapon)
        {
            return GetWeaponConfig(weapon).Desc;
        }


        public static float GetWeaponPrice(int weapon)
        {
            return GetWeaponConfig(weapon).Price;
        }


        public static float GetNextAvailableWeaponPrice()
        {
            int nexAvailableIndex = Count - 1;
            for (int i = nexAvailableIndex; i >= 0; i--)
            {
                if (Player.IsWeaponBought(i))
                {
                    break;
                }

                nexAvailableIndex = i;
            }

            return GetWeaponPrice(nexAvailableIndex);
        }


        public static Sprite GetWeaponSprite(int weapon)
        {
            return GetWeaponConfig(weapon).Sprite;
        }


        public static Sprite GetWeaponMask(int weapon)
        {
            return GetWeaponConfig(weapon).Mask;
        }


        public static ParticleSystem GetWeaponUnlockEffect(int weapon)
        {
            return GetWeaponConfig(weapon).UnlockEffect;
        }


        public static uint GetWeaponMaxAmmo(int weapon)
        {
            WeaponConfig config = GetWeaponConfig(weapon);
            return config.AmmoBase + Player.GetAmmoLevel(weapon) * config.AmmoUpgrade;
        }


        public static uint GetWeaponBaseAmmo(int weapon)
        {
            return GetWeaponConfig(weapon).AmmoBase;
        }


        public static float GetAmmoUpgradePrice(int weapon)
        {
            return GetWeaponConfig(weapon).AmmoUpgradePrice * Mathf.Pow(1.27f, Player.GetAmmoLevel(weapon)) * Player.GetPercentReducedWeaponUpgrade();
        }


        public static float GetAmmoUpgradePriceForLevel(int weapon, int level)
        {
            return GetWeaponConfig(weapon).AmmoUpgradePrice * Mathf.Pow(1.27f, level) * Player.GetPercentReducedWeaponUpgrade();
        }


        public static float GetWeaponDamage(int weapon)
        {
            WeaponConfig config = GetWeaponConfig(weapon);
            return config.DamageBase + Player.GetDamageLevel(weapon) * config.DamageUpgrade;
        }


        public static float GetWeaponBaseDamage(int weapon)
        {
            return GetWeaponConfig(weapon).DamageBase;
        }


        public static float GetWeaponPower(int weapon)
        {
            return GetWeaponDamage(weapon) * GetWeaponMaxAmmo(weapon);
        }


        public static float GetDamageUpgradePrice(int weapon)
        {
            return GetWeaponConfig(weapon).DamageUpgradePrice * Mathf.Pow(1.27f, Player.GetDamageLevel(weapon)) * Player.GetPercentReducedWeaponUpgrade();
        }


        public static float GetDamageUpgradePriceForLevel(int weapon, int level)
        {
            return GetWeaponConfig(weapon).DamageUpgradePrice * Mathf.Pow(1.27f, level) * Player.GetPercentReducedWeaponUpgrade();
        }


        public static uint GetWeaponCoins(int weapon)
        {
            WeaponConfig config = GetWeaponConfig(weapon);
            return (uint)Mathf.Max((config.CoinsBase + Player.GetDamageLevel(weapon) * config.CoinsUpgrate) / config.Parameters.SizeMagazine, 1);
        }


        public static WeaponParameters GetWeaponParameters(int weapon)
        {
            WeaponConfig config = GetWeaponConfig(weapon);
            return config.Parameters;
        }


        public static bool CanWeaponAutoShoot(int weapon)
        {
            return ABTest.AutotapTrue || GetWeaponConfig(weapon).CanAutoShoot;
        }


        public static float GetWeaponAutoShootСooldown(int weapon)
        {
            return GetWeaponConfig(weapon).AutoShootCooldown;
        }
        
        
        public static WeaponConfig GetWeaponConfig(int index)
        {
            if (!loadedWeapons.ContainsKey(index))
            {
                loadedWeapons.Add(index, asset.Value.weaponsAssetLinks[index].GetAsset() as WeaponConfig);
            }
            
            return loadedWeapons[index];
        }

        #endregion
    }
}
