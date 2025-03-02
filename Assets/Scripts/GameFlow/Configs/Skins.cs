using Spine.Unity;
using UnityEngine;
using System;

namespace PinataMasters
{
    [Serializable]
    public struct SkinConfig
    {
        public Sprite Sprite;
        public Sprite ShadowSprite;
        public string NameKey;
        public SkeletonDataAsset body;
        public SkeletonDataAsset legs;
        public Skill skill;
    }


    [CreateAssetMenu]
    public class Skins : ScriptableObject
    {
        #region Variables

        private static readonly ResourceAsset<Skins> asset = new ResourceAsset<Skins>("Game/Skins");

        [SerializeField]
        private SkinConfig[] skins = null;

        #endregion



        #region Properties

        public static int Count
        {
            get { return asset.Value.skins.Length; }
        }

        #endregion



        #region Public methods

        public static SkeletonDataAsset GetBodySkeletonAsset(int index)
        {
            return GetConfig(index).body;
        }


        public static SkeletonDataAsset GetLegsSkeletonAsset(int index)
        {
            return GetConfig(index).legs;
        }


        public static string GetName(int index)
        {
            return Localisation.LocalizedStringOrSource(GetConfig(index).NameKey);
        }


        public static string GetKeyName(int index)
        {
            return GetConfig(index).NameKey;
        }


        public static string GetDesc(int index)
        {
            return Localisation.LocalizedStringOrSource(GetSkillConfig(index).SkillDescKey);
        }


        public static string GetFormat(int index)
        {
            return GetSkillConfig(index).SkillDescFormat;
        }


        public static Sprite GetSprite(int index)
        {
            return GetConfig(index).Sprite;
        }


        public static Sprite GetShadowSprite(int index)
        {
            return GetConfig(index).ShadowSprite;
        }


        public static float GetSkinPrice(int index)
        {
            return GetSkillConfig(index).BuyPrice;
        }


        public static float GetUpgradePrice(int index)
        {
            return GetSkillConfig(index).UpgradePrice[Player.GetSkinLevel(index)];
        }


        public static float GetPassiveSkillBonus(PassiveSkillType type)
        {
            for (int i = 0; i < Count; i++)
            {
                if (GetSkillConfig(i).Type == type)
                {
                    if (Player.IsSkinBought(i))
                    {
                        return GetSkillConfig(i).StartValue + GetSkillConfig(i).Increment * Player.GetSkinLevel(i);
                    }
                    else
                    {
                        return GetSkillConfig(i).DefaultValue;
                    }
                }
            }

            throw new ArgumentException("Not found skill with type " + type);
        }


        public static float GetPassiveSkillBonusForText(int index)
        {
            for (int i = 0; i < Count; i++)
            {
                if (i == index)
                {
                    if (Player.IsSkinBought(i))
                    {
                        return GetSkillConfig(i).Type.ToText(GetSkillConfig(i).StartValue + GetSkillConfig(i).Increment * Player.GetSkinLevel(i));
                    }
                    else
                    {
                        return GetSkillConfig(i).Type.ToText(GetSkillConfig(i).StartValue);
                    }
                }
            }

            throw new ArgumentException();
        }


        public static uint IsMaxUpgradeLevel(int index)
        {
           return GetSkillConfig(index).MaxUpgradeLevel;
        }


        public static int GetSkinByKey(string key)
        {
            for (int i = 0; i < asset.Value.skins.Length; i++)
            {
                if (asset.Value.skins[i].NameKey.Equals(key))
                {
                    return i;
                }
            }

            throw new ArgumentException("No skin with key " + key);
        }

        #endregion



        #region Private methods

        private static SkinConfig GetConfig(int index)
        {
            return asset.Value.skins[index];
        }


        private static Skill GetSkillConfig(int index)
        {
            return asset.Value.skins[index].skill;
        }

        #endregion
    }
}
