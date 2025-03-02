using System;
using UnityEngine;


namespace PinataMasters
{
    [Serializable]
    public enum PassiveSkillType
    {
        MoreCoins,
        ObstaclesDamage,
        ObstaclesCoins,
        CritChance,
        WeaponUpgradePrice,
        DoubleShoot
    }

    public static class PassiveSkillTypeExtension
    {
        public static float ToText(this PassiveSkillType type, float value)
        {
            switch(type)
            {
                case PassiveSkillType.MoreCoins:
                case PassiveSkillType.ObstaclesDamage:
                case PassiveSkillType.DoubleShoot:
                    return value;
                case PassiveSkillType.ObstaclesCoins:
                case PassiveSkillType.WeaponUpgradePrice:
                case PassiveSkillType.CritChance:
                    return value * 100f;
                default:
                    throw new ArgumentException("No behavior for type: " + type);
            }
        }
    }

    [CreateAssetMenu]
    public class Skill : ScriptableObject
    {
        #region Variables

        public PassiveSkillType Type = PassiveSkillType.MoreCoins;
        public float BuyPrice = 0f;
        public float[] UpgradePrice = null;
        public float StartValue = 0f;
        public float Increment = 0f;
        public float DefaultValue = 0f;
        public string SkillDescKey = null;
        public string SkillDescFormat = null;
        public uint MaxUpgradeLevel = 0u;

        #endregion


#if UNITY_EDITOR
        #region Private methods

        private void OnValidate()
        {
            if (Type == PassiveSkillType.DoubleShoot && (StartValue < 0f || Increment > 0f))
            {
                Debug.LogWarning("Probably you need to check StartValue and Increment for Skill " + Type);
            }

            if (Type == PassiveSkillType.WeaponUpgradePrice && (StartValue < 0f || StartValue > 1f || Increment < 0f || Increment > 1f))
            { 
                Debug.LogWarning("StartValue and Increment is calculated in parts (not percent) for Weapon Upgrade Price");
            }

            if (MaxUpgradeLevel < 0)
            {
                Debug.LogWarning("MaxUpgradeLevel must be grater or equal than 0");
            }

            if (UpgradePrice.Length != MaxUpgradeLevel)
            {
                Debug.LogWarning("Upgrade price aren't set for all levels of skill");
            }
        }

        #endregion
#endif
    }
}
