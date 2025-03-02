using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    [Serializable]
    public class SkinsProgress
    {
        public int Type;
        public uint Level;
    }


    [Serializable]
    public class WeaponProgress
    {
        public int Type;
        public uint DamageLevel;
        public uint AmmoLevel;
    }


    public class ShadowInfo
    {
        public int skin;
        public int weapon;
    }


    public static class Player
    {
        #region Types

        [Serializable]
        public class Data
        {
            public float AllGems;
            public uint KillsPinate;

            public uint Level;
            public uint TotalLevel = 1;
            public uint PresigeLevel;
            public float Coins;
            public float Gems;
            public uint SpeedLevel;
            public uint OfflineRewardLevel;
            public uint BonusCoinsLevel;
            public int CurrentWeapon;
            public int CurrentSkin;
            public readonly List<WeaponProgress> WeaponProgress = new List<WeaponProgress>();
            public readonly List<SkinsProgress> SkinsProgress = new List<SkinsProgress>();

            public void Reset()
            {
                Level = 0;
                PresigeLevel++;
                Coins = 0f;
                SpeedLevel = 0;
                OfflineRewardLevel = 0;
                BonusCoinsLevel = 0;
                CurrentWeapon = 0;
                WeaponProgress.Clear();
                WeaponProgress.Add(new WeaponProgress { Type = 0 });

                Refresh();
            }

            public void Refresh()
            {
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

                OnResetProgress();
                OnChangeGems();
                OnChangeCoins();
                OnChangeWeapon();
                OnChangeShooterSpeed();
            }
        }

        #endregion



        #region Variables

        private const string DATA_KEY = "PlayerData";

        private static Data data;

        public static event Action OnLevelUp = delegate { };
        public static event Action OnChangeCoins = delegate { };
        public static event Action OnChangeGems = delegate { };
        public static event Action OnIncreaseGems = delegate { };
        public static event Action OnChangeWeapon = delegate { };
        public static event Action OnChangeSkin = delegate { };
        public static event Action OnChangeShooterSpeed = delegate { };
        public static event Action OnResetProgress = delegate { };
        public static event Action OnPrefsUpdated = delegate { };

        #endregion



        #region Properties


        public static int CurrentWeapon
        {
            get
            {
                return data.CurrentWeapon;
            }
            set
            {
                if (data.CurrentWeapon != value)
                {
                    data.CurrentWeapon = value;
                    CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
                    OnChangeWeapon();
                }
            }
        }


        public static int CurrentSkin
        {
            get
            {
                return data.CurrentSkin;
            }
            set
            {
                if (data.CurrentSkin != value)
                {
                    data.CurrentSkin = value;
                    CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
                    OnChangeSkin();
                    GameAnalytics.ChooseSkin(Skins.GetKeyName(CurrentSkin), GetSkinProgress(CurrentSkin).Level + 1);
                }
            }
        }


        public static uint KillsPinate
        {
            get
            {
                return data.KillsPinate;
            }
            private set
            {
                data.KillsPinate = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }


        public static float AllGems
        {
            get
            {
                return data.AllGems;
            }
            private set
            {
                data.AllGems = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }


        public static uint Level
        {
            get
            {
                return data.Level;
            }
            private set
            {
                data.Level = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }

        
        public static uint TotalLevel
        {
            get
            {
                return data.TotalLevel;
            }
            private set
            {
                data.TotalLevel = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }
        

        public static bool IsBossLevel => (Level + 1) % 5 == 0;


        public static uint PresigeLevel => data.PresigeLevel;


        public static float Coins
        {
            get
            {
                return data.Coins;
            }
            private set
            {
                data.Coins = Mathf.Round(value);
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
                OnChangeCoins();
            }
        }


        public static float Gems
        {
            get
            {
                return data.Gems;
            }
            private set
            {
                data.Gems = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
                OnChangeGems.Invoke();
            }
        }


        public static uint SpeedLevel
        {
            get
            {
                return data.SpeedLevel;
            }
            private set
            {
                data.SpeedLevel = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
                OnChangeShooterSpeed();
            }
        }


        public static uint BonusCoinsLevel
        {
            get
            {
                return data.BonusCoinsLevel;
            }
            private set
            {
                data.BonusCoinsLevel = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }

        public static uint OfflineRewardLevel
        {
            get
            {
                return data.OfflineRewardLevel;
            }
            private set
            {
                data.OfflineRewardLevel = value;
                CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
            }
        }


        public static void ResetWeapons()
        {
            OnChangeWeapon();
        }

        #endregion


        static Player()
        {
            data = CustomPlayerPrefs.GetObjectValue<Data>(DATA_KEY);

            if (data == null)
            {
                data = new Data();
            }

            if (!data.SkinsProgress.Exists(p => p.Type == 0))
            {
                data.SkinsProgress.Add(new SkinsProgress { Type = 0 });
                data.CurrentSkin = 0;
            }

            if (!data.WeaponProgress.Exists(p => p.Type == 0))
            {
                data.WeaponProgress.Add(new WeaponProgress { Type = 0 });
                data.CurrentWeapon = 0;
            }

            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);
        }


        #region Public methods

        public static Data GetPrefs()
        {
            return data;
        }


        public static void UpdatePrefs(Data remoteData)
        {
            data = remoteData;
            data.Refresh();

            ShooterShadowsConfig.SetShadowsInfo();

            OnChangeSkin();
            OnChangeWeapon();
            OnPrefsUpdated();
        }


        public static bool IsBetterCurrent(Data other)
        {

            return other.PresigeLevel > data.PresigeLevel ||
                   ((other.PresigeLevel == data.PresigeLevel) && other.KillsPinate > data.KillsPinate);
        }


        public static void UpLevel()
        {
            Level++;
            TotalLevel++;
            data.KillsPinate++;
            SelectorLevels.GetLevels.CheckLevelsOnOutOfRange();
            OnLevelUp();
        }


        public static void SetLevel(uint level)
        {
            Level = level;
            OnLevelUp();
        }


        public static void ResetProgress()
        {
            data.Reset();
        }


        public static void AddCoins(float count)
        {
            Coins += count;
        }


        public static void AddGems(float gems)
        {
            Gems += gems;
            data.AllGems += gems;

            GameAnalytics.SetGemsEarnedUserProperty(gems);

            OnIncreaseGems.Invoke();
        }


        public static bool TryRemoveCoins(float count)
        {
            if (Coins < count)
            {
                return false;
            }

            Coins -= count;
            GameAnalytics.SetCoinsSpentUserProperty(count);

            return true;
        }


        public static bool TryRemoveGems(float count)
        {
            if (Gems < count)
            {
                return false;
            }

            Gems -= count;
            return true;
        }


        public static void BuySkin(int type)
        {
            data.SkinsProgress.Add(new SkinsProgress { Type = type });
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.SendUnlockContentEvent(ContentClass.SKIN, Skins.GetKeyName(type).ToLower(), UnlockType.INGAME_PURCHASE);
            GameAnalytics.SendSpendIngameCurrencyEvent(Skins.GetSkinPrice(type).ToString(), ContentClass.SKIN, Skins.GetKeyName(type).ToLower(), CurrencyType.GEMS);
            GameAnalytics.BuySkin(Skins.GetKeyName(type), Skins.GetSkinPrice(type));
        }


        public static void UpgradeSkin(int type)
        {
            float price = Skins.GetUpgradePrice(type);

            SkinsProgress progress = GetSkinProgress(type);
            progress.Level++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.UpgradeSkin(Skins.GetKeyName(type), price, progress.Level + 1);
            GameAnalytics.SendSpendIngameCurrencyEvent(price.ToString(), ContentClass.SKIN, Skins.GetKeyName(type).ToLower(), CurrencyType.GEMS);
        }


        public static void BuyWeapon(int type)
        {
            WeaponProgress progress = new WeaponProgress { Type = type };

            data.WeaponProgress.Add(progress);
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            TutorialManager.Instance.IsBuyWeaponTutorialPassed = true;

            GameAnalytics.SendUnlockContentEvent(ContentClass.WEAPON, Arsenal.GetWeaponDesc(type).ToLower(), UnlockType.INGAME_PURCHASE);
            GameAnalytics.SendSpendIngameCurrencyEvent(Arsenal.GetWeaponPrice(type).ToString(), ContentClass.WEAPON, Arsenal.GetWeaponDesc(type).ToLower(), CurrencyType.COINS);
        }


        public static void BuyAmmoUpgrade(int type)
        {
            WeaponProgress progress = GetWeaponProgress(type);

            progress.AmmoLevel++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.TrySendUpgradeContentEvent(Arsenal.GetWeaponDesc(type).ToLower(), ContentType.AMMO, progress.AmmoLevel);

            if (progress.AmmoLevel % GameAnalytics.EventUpgradeStep == 0)
            {
                float priceForUpgrade = 0f;

                for (int i = (int)progress.AmmoLevel; i > (progress.AmmoLevel - GameAnalytics.EventUpgradeStep); i--)
                {
                    priceForUpgrade += Arsenal.GetAmmoUpgradePriceForLevel(type, i - 1);
                }

                GameAnalytics.SendSpendIngameCurrencyEvent(priceForUpgrade.ToString(), ContentClass.WEAPON, ContentType.AMMO, CurrencyType.COINS, progress.AmmoLevel);
            }
        }


        public static void BuyDamageUpgrade(int type)
        {
            WeaponProgress progress = GetWeaponProgress(type);


            progress.DamageLevel++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.TrySendUpgradeContentEvent(Arsenal.GetWeaponDesc(type).ToLower(), ContentType.DAMAGE, progress.DamageLevel);

            if (progress.DamageLevel % GameAnalytics.EventUpgradeStep == 0)
            {
                float priceForUpgrade = 0f;

                for (int i = (int)progress.DamageLevel; i > (progress.DamageLevel - GameAnalytics.EventUpgradeStep); i--)
                {
                    priceForUpgrade += Arsenal.GetDamageUpgradePriceForLevel(type, i - 1);
                }

                GameAnalytics.SendSpendIngameCurrencyEvent(priceForUpgrade.ToString(), ContentClass.WEAPON, ContentType.DAMAGE, CurrencyType.COINS, progress.DamageLevel);
            }
        }


        public static void BuySpeedUpgrade()
        {
            SpeedLevel++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.TrySendUpgradeContentEvent(ContentClass.CHARACTER, ContentType.SPEED, SpeedLevel);

            if (SpeedLevel % GameAnalytics.EventUpgradeStep == 0)
            {
                float priceForUpgrade = 0f;

                for (uint i = SpeedLevel; i > (SpeedLevel - GameAnalytics.EventUpgradeStep); i--)
                {
                    priceForUpgrade += PlayerConfig.GetSpeedUpgradePrice(i - 1);
                }

                GameAnalytics.SendSpendIngameCurrencyEvent(priceForUpgrade.ToString(), ContentClass.CHARACTER, ContentType.SPEED, CurrencyType.COINS, SpeedLevel);
            }
        }


        public static void BuyOfflineRewardUpgrade()
        {
            OfflineRewardLevel++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.TrySendUpgradeContentEvent(ContentClass.CHARACTER, ContentType.OFFLINE_REWARD, OfflineRewardLevel);

            if (OfflineRewardLevel % GameAnalytics.EventUpgradeStep == 0)
            {
                float priceForUpgrade = 0f;

                for (uint i = OfflineRewardLevel; i > (OfflineRewardLevel - GameAnalytics.EventUpgradeStep); i--)
                {
                    priceForUpgrade += PlayerConfig.GetOfflineRewardUpgradePrice(i - 1);
                }

                GameAnalytics.SendSpendIngameCurrencyEvent(priceForUpgrade.ToString(), ContentClass.CHARACTER, ContentType.OFFLINE_REWARD, CurrencyType.COINS, OfflineRewardLevel);
            }
        }


        public static void BuyBonusCoinsUpgrade()
        {
            BonusCoinsLevel++;
            CustomPlayerPrefs.SetObjectValue(DATA_KEY, data);

            GameAnalytics.TrySendUpgradeContentEvent(ContentClass.CHARACTER, ContentType.BONUS_COINS, BonusCoinsLevel);

            if (BonusCoinsLevel % GameAnalytics.EventUpgradeStep == 0)
            {
                float priceForUpgrade = 0f;

                for (uint i = BonusCoinsLevel; i > (BonusCoinsLevel - GameAnalytics.EventUpgradeStep); i--)
                {
                    priceForUpgrade += PlayerConfig.GetBonusCoinsUpgradePrice(i - 1);
                }

                GameAnalytics.SendSpendIngameCurrencyEvent(priceForUpgrade.ToString(), ContentClass.CHARACTER, ContentType.BONUS_COINS, CurrencyType.COINS, BonusCoinsLevel);
            }
        }


        public static uint GetDamageLevel(int weapon)
        {
            return GetWeaponProgress(weapon).DamageLevel;
        }


        public static uint GetAmmoLevel(int weapon)
        {
            return GetWeaponProgress(weapon).AmmoLevel;
        }


        public static bool IsWeaponBought(int weapon)
        {
            return GetWeaponProgress(weapon) != null;
        }


        public static bool IsSkinBought(int skin)
        {
            return GetSkinProgress(skin) != null;
        }


        public static bool IsSkinMaxLevelReached(int skin)
        {
            return GetSkinProgress(skin).Level == Skins.IsMaxUpgradeLevel(skin);
        }


        public static int GetWeaponsCount()
        {
            return data.WeaponProgress.Count;
        }


        public static int GetSkinsCount()
        {
            return data.SkinsProgress.Count;
        }


        public static uint GetSkinLevel(int skin)
        {
            return GetSkinProgress(skin) == null ? 0 : GetSkinProgress(skin).Level;
        }


        public static float GetBonusCoinsSkill()
        {
            return Skins.GetPassiveSkillBonus(PassiveSkillType.MoreCoins);
        }


        public static float GetAdditionalObstacleDamage()
        {
            return Skins.GetPassiveSkillBonus(PassiveSkillType.ObstaclesDamage);
        }


        public static float GetCritChance()
        {
            return Skins.GetPassiveSkillBonus(PassiveSkillType.CritChance);
        }


        public static float GetObstacleCoins()
        {
            return Skins.GetPassiveSkillBonus(PassiveSkillType.ObstaclesCoins);
        }

        public static int GetDoubleShotRate()
        {
            float rate = Skins.GetPassiveSkillBonus(PassiveSkillType.DoubleShoot);
            return float.IsInfinity(rate) ? int.MaxValue : Mathf.RoundToInt(rate);
        }


        public static float GetPercentReducedWeaponUpgrade()
        {
            return 1f - Skins.GetPassiveSkillBonus(PassiveSkillType.WeaponUpgradePrice);
        }

        #endregion



        #region Private methods

        private static WeaponProgress GetWeaponProgress(int weapon)
        {
            return  data.WeaponProgress.Find(p => p.Type == weapon);
        }


        private static SkinsProgress GetSkinProgress(int skin)
        {
            return data.SkinsProgress.Find(p => p.Type == skin);
        }

        #endregion
    }
}
