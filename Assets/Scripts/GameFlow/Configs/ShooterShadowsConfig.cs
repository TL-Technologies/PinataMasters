using UnityEngine;
using System;
using System.Collections.Generic;
using Spine.Unity;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class ShooterShadowsConfig : ScriptableObject
    {
        #region Variables

        private static readonly ResourceAsset<ShooterShadowsConfig> asset = new ResourceAsset<ShooterShadowsConfig>("Game/ShooterShadowsConfig");

        public static event Action OnShadowsChanged = delegate { };

        private const string SHADER_COLOR_LIGHT_PROPERTY = "_ShadowColorLight";
        private const string SHADER_COLOR_MIDDLE_PROPERTY = "_ShadowColorMiddle";
        private const string SHADER_COLOR_DARK_PROPERTY = "_ShadowColorDark";
        private const string SPRITE_DEFAULT_SHADER = "Sprites/Default";

        [SerializeField]
        private float firstShadowMoveDelay;
        [SerializeField]
        private float shadowsMoveDelay;
        [SerializeField]
        private float minShadowShotDelay;
        [SerializeField]
        private float maxShadowShotDelay;
        [SerializeField]
        private Shader shadowAnimationShader;
        [SerializeField]
        private Shader shadowSpriteShader;
        [SerializeField]
        private Shader shooterAnimationShader;
        [SerializeField]
        private Color shadowColorLight = Color.black;
        [SerializeField]
        private Color shadowColorMiddle = Color.black;
        [SerializeField]
        private Color shadowColorDark = Color.black;

        Dictionary<int, List<int>> legalShots = new Dictionary<int, List<int>>();

        private ShadowInfo shooterInfo = new ShadowInfo();
        private List<ShadowInfo> shadowsInfo = new List<ShadowInfo>();

        #endregion



        #region Public methods

        public static bool IsNeedreleaseShot(int weapon, int playerLeftShots, int totalShotsCount)
        {
            List<int> shots = new List<int>();
            if (asset.Value.legalShots.TryGetValue(weapon, out shots))
            {
                return shots.Contains(totalShotsCount - playerLeftShots + 1);
            }

            return false;
        }


        public static float GetShadowDelay(int shadowNumber)
        {
            return asset.Value.firstShadowMoveDelay + asset.Value.shadowsMoveDelay * shadowNumber;
        }


        public static float GetShadowsBulletsImpulseDenominationValue(int weapon)
        {
            return Arsenal.GetWeaponConfig(weapon).impulseMultiplier;
        }


        public static List<ShadowInfo> GetShadowsInfo()
        {
            return asset.Value.shadowsInfo;
        }


        public static ShadowInfo GetShooterInfo()
        {
            return asset.Value.shooterInfo;
        }


        public static void SetShadowsInfo(int playerSkin = -1, int playerWeapon = -1)
        {
            asset.Value.shooterInfo.skin = (playerSkin < 0) ? (Player.CurrentSkin) : (playerSkin);
            asset.Value.shooterInfo.weapon = (playerWeapon < 0) ? (Player.CurrentWeapon) : (playerWeapon);

            asset.Value.shadowsInfo.Clear();
            asset.Value.legalShots.Clear();
            List<int> skins = new List<int>();
            List<int> weapons = new List<int>();

            for (int i = 0; i < Skins.Count; i++)
            {
                if (Player.IsSkinBought(i) && asset.Value.shooterInfo.skin != i)
                {
                    skins.Add(i);
                }
            }

            for (int i = 0; i < Arsenal.Count; i++)
            {
                if (Player.IsWeaponBought(i) && asset.Value.shooterInfo.weapon != i)
                {
                    weapons.Insert(0, i);
                }
            }

            int shadowsCount = skins.Count;

            for (int i = 0; i < shadowsCount; i++)
            {
                int skin = skins.RandomObject();
                ShadowInfo shadowInfo = new ShadowInfo();
                shadowInfo.skin = skin;
                if ((weapons.Count > 0))
                {
                    shadowInfo.weapon = weapons.FirstObject();
                    weapons.RemoveAt(0);
                    asset.Value.legalShots.Add(shadowInfo.weapon, RandomizeShots(shadowInfo.weapon));
                }
                else
                {
                    shadowInfo.weapon = -1;
                }
                asset.Value.shadowsInfo.Add(shadowInfo);
                skins.Remove(skin);
            }

            OnShadowsChanged();
        }


        public static Sprite GetShadowSprite(int weapon)
        {
            Sprite resultSprite = null;

            for (int i = 0; i < asset.Value.shadowsInfo.Count; i++)
            {
                if (asset.Value.shadowsInfo[i].weapon == weapon)
                {
                    resultSprite = Skins.GetShadowSprite(asset.Value.shadowsInfo[i].skin);
                }
            }

            return resultSprite;
        }


        public static float GetShadowShotDelay()
        {
            return UnityEngine.Random.Range(asset.Value.minShadowShotDelay, asset.Value.maxShadowShotDelay);
        }


        public static float GetAllSkinsWeaponsPower()
        {
            float result = Arsenal.GetWeaponPower(Player.CurrentWeapon);

            foreach (ShadowInfo shadowInfo in asset.Value.shadowsInfo)
            {
                if (shadowInfo.weapon >= 0)
                {
                    result += Arsenal.GetWeaponPower(shadowInfo.weapon);
                }
            }

            return result;
        }


        public static Material SetSpriteShader(Material material, bool isShadow)
        {
            material.shader = (isShadow) ? (asset.Value.shadowSpriteShader) : (Shader.Find(SPRITE_DEFAULT_SHADER));

            if (isShadow)
            {
                material.SetColor(SHADER_COLOR_LIGHT_PROPERTY, asset.Value.shadowColorLight);
                material.SetColor(SHADER_COLOR_MIDDLE_PROPERTY, asset.Value.shadowColorMiddle);
                material.SetColor(SHADER_COLOR_DARK_PROPERTY, asset.Value.shadowColorDark);
            }

            return material;
        }


        public static void SetAnimationShader(SkeletonAnimation animation, bool isShadow)
        {
            Shader newShader = (isShadow) ? (asset.Value.shadowAnimationShader) : (asset.Value.shooterAnimationShader);

            List<Material> oldMaterials = new List<Material>();

            animation.CustomMaterialOverride.Clear();
            
            List<SpineAtlasAsset> atlases = new List<SpineAtlasAsset>();
            
            foreach (AtlasAssetBase atlasAssetBase in animation.SkeletonDataAsset.atlasAssets)
            {
                SpineAtlasAsset atlasAsset = atlasAssetBase as SpineAtlasAsset;
                if (atlasAsset != null)
                {
                    atlases.Add(atlasAsset);
                }
            }

            for (int i = 0; i < atlases.Count; i++)
            {
                for (int j = 0; j < atlases[i].materials.Length; j++)
                {
                    animation.CustomMaterialOverride.Add(atlases[i].materials[j], atlases[i].materials[j]);
                    oldMaterials.Add(atlases[i].materials[j]);
                }
            }

            for (int i = 0; i < oldMaterials.Count; i++)
            {
                Material newMaterial = new Material(oldMaterials[i]) { shader = newShader };
                if (isShadow)
                {
                    newMaterial.SetColor(SHADER_COLOR_LIGHT_PROPERTY, asset.Value.shadowColorLight);
                    newMaterial.SetColor(SHADER_COLOR_MIDDLE_PROPERTY, asset.Value.shadowColorMiddle);
                    newMaterial.SetColor(SHADER_COLOR_DARK_PROPERTY, asset.Value.shadowColorDark);
                }
                animation.CustomMaterialOverride[oldMaterials[i]] = newMaterial;
            }
        }

        #endregion



        #region Private methods

        private static List<int> RandomizeShots(int weapon)
        {
            int ammo = (int)Arsenal.GetWeaponMaxAmmo(Player.CurrentWeapon);
            List<int> result = new List<int>();
            List<int> shots = new List<int>();
            int shotsCount = Mathf.CeilToInt(((float)ammo * Arsenal.GetWeaponConfig(weapon).shotsPercents) + 0.5f);

            for (int i = 0; i < ammo; i++)
            {
                shots.Add(i + 1);
            }

            for (int i = 0; i < shotsCount; i++)
            {
                int randomShot = shots.RandomObject();
                shots.Remove(randomShot);
                result.Add(randomShot);
            }

            return result;
        }

        #endregion
    }
}