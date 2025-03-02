using UnityEngine;
using System;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

namespace PinataMasters
{
    [ExecuteInEditMode]
    [CreateAssetMenu]
    public class Levels : ScriptableObject
    {
        #region Variables

        [SerializeField]
        private LevelConfig[] configs = null;

        [Header("Number Coins")]
        [SerializeField]
        private uint coinsBase = 0;
        [SerializeField]
        private uint coinsUpgrate = 0;

        [Header("Obstacle")]
        [SerializeField]
        private float factorAround = 0f;
        [SerializeField]
        private float factorSlower = 1f;
        [SerializeField]
        private float partPinataHealth = 0f;
        
        [Header("Out Of range configs")]
        [SerializeField] float healthMultiplier = 1.2f;
        [SerializeField] int minRandomLevelsIndex;
        [SerializeField] int maxRandomLevelsIndex;

        LevelConfig currentLevel;

        #endregion



        #region Properties

        public uint CoinsBase
        {
            get { return coinsBase; }
        }


        public uint CoinsUpgrate
        {
            get { return coinsUpgrate; }
        }


        public float FactorAround
        {
            get { return factorAround; }
        }


        public float FactorSlower
        {
            get { return factorSlower; }
        }


        public static int OutOfRangeLevelIndex
        {
            get
            {
                return PlayerPrefs.GetInt("out_of_range_level_index", -1);
            }
            set
            {
                PlayerPrefs.SetInt("out_of_range_level_index", value);
            }
        }
        

        LevelConfig CurrentLevel
        {
            get
            {
                if (currentLevel.Equals(default(LevelConfig)))
                {
                    RandomOutOfRangeLevel();
                }

                return currentLevel;
            }
        }

        #endregion



        #region Public methods

        public GameObject GetPrefabBack(uint level)
        {
            return (level >= configs.Length ? CurrentLevel : configs[level]).AssetBack.GetAsset() as GameObject;
        }


        public GameObject GetPrefabBack(string backName)
        {
            foreach (LevelConfig config in configs)
            {
                if (config.AssetBack.GetAsset().name.Equals(backName))
                {
                    return config.AssetBack.GetAsset() as GameObject;
                }
            }

            throw new ArgumentException("No back with name " + backName);
        }


        public string[] GetBackgrounds()
        {
            string[] backgrounds = new string[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                backgrounds[i] = configs[i].AssetBack.GetAsset().name;
            }

            return backgrounds;
        }


        public string[] GetPinatas()
        {
            string[] pinatas = new string[configs.Length];

            for (int i = 0; i < configs.Length; i++)
            {
                pinatas[i] = configs[i].AssetPinata.GetAsset().name;
            }

            return pinatas;
        }


        public Pinata GetPrefabPinata(uint level)
        {
            return ((level >= configs.Length ? CurrentLevel : configs[level]).
                AssetPinata.GetAsset() as GameObject).GetComponent<Pinata>();
        }


        public Pinata GetPrefabPinata(string pinataName)
        {
            foreach (LevelConfig config in configs)
            {
                if (config.AssetPinata.GetAsset().name.Equals(pinataName))
                {
                    return (config.AssetPinata.GetAsset() as GameObject).GetComponent<Pinata>();
                }
            }

            throw new ArgumentException("No pinata with name " + pinataName);
        }


        public GameObject GetPrefabPinataObstacles(uint level)
        {
            if (string.IsNullOrEmpty(
                (level >= configs.Length ? CurrentLevel : configs[level]).AssetPinataObstacles.FullPath))
            {
                return null;
            }

            return
                (level >= configs.Length ? CurrentLevel : configs[level]).AssetPinataObstacles.GetAsset() as GameObject;
        }


        public GameObject GetPrefabLevelObstacles(uint level)
        {
            if (string.IsNullOrEmpty((level >= configs.Length ? CurrentLevel : configs[level]).AssetLevelObstacles.FullPath))
            {
                return null;
            }

            return (level >= configs.Length ? CurrentLevel : configs[level]).AssetLevelObstacles.GetAsset() as GameObject;
        }


        public float WinCoins(uint level)
        {
            return GetHealthPinata(level) *
                   (level >= configs.Length ? CurrentLevel : configs[level]).WinCoinsPersentHealthPinata;
        }


        public float GetHealthPinata(uint level)
        {
            return (level >= configs.Length ? CurrentLevel : configs[level]).HealthPinata;
        }


        public float GetHealthArenaObstacle(uint level)
        {
            return (level >= configs.Length ? CurrentLevel : configs[level]).LevelObstacleHealth *
                   GetHealthPinata(level);
        }


        public float GetHealthPinataObstacle(uint level)
        {
            return (level >= configs.Length ? CurrentLevel : configs[level]).PinataObstacleHealth *
                   GetHealthPinata(level);
        }


        public float GetGemsForReset(uint level)
        {
            return (level >= configs.Length ? CurrentLevel : configs[level]).GemsForReset;
        }


        public void TryLoadOutOfRangeLevel()
        {
            if (OutOfRangeLevelIndex > 0)
            {
                FillCurrentOutOfRangeLevel(OutOfRangeLevelIndex);
            }
        }


        public void CheckLevelsOnOutOfRange()
        {
            if (Player.Level >= configs.Length)
            {
                RandomOutOfRangeLevel();
            }
        }

        #endregion



        #region Private methods
            
        void RandomOutOfRangeLevel()
        {
            FillCurrentOutOfRangeLevel(Random.Range(minRandomLevelsIndex, maxRandomLevelsIndex));
        }

        
        void FillCurrentOutOfRangeLevel(int index)
        {
            long differFromBorder = Player.Level - configs.Length + 1;
            
            currentLevel = configs[index];
            currentLevel.HealthPinata = configs.LastObject().HealthPinata * Mathf.Pow(healthMultiplier, differFromBorder);
            currentLevel.WinCoinsPersentHealthPinata = Player.IsBossLevel ? 2f : 1.2f;
            currentLevel.GemsForReset = configs.LastObject().GemsForReset + differFromBorder;
            
            OutOfRangeLevelIndex = index;
        }

        #endregion
        

#if UNITY_EDITOR
        const string separator = ",";

        [Space]
        [Header("Export/Import")]
        [Button]
        public string ButtonExport = "ExportToFile";
        private void ExportToFile()
        {
            string path = EditorUtility.SaveFilePanel("Export to file", string.Empty, "Levels.csv", string.Empty);

            if (string.IsNullOrEmpty(path)) return;

            using (FileStream fs = File.OpenWrite(path))
            using (StreamWriter tw = new StreamWriter(fs))
            {
                LevelConfig[] levels = configs;
                for (int i = 0; i < levels.Length; i++)
                {
                    tw.Write(i);
                    tw.Write(separator);

                    tw.Write(levels[i].HealthPinata);
                    tw.Write(separator);

                    tw.Write(levels[i].WinCoinsPersentHealthPinata);
                    tw.Write(separator);

                    tw.Write(levels[i].GemsForReset);
                    tw.WriteLine();
                }
            }

            Debug.Log("Done export levels to file: " + path);
        }

        [Button]
        public string ButtonImport = "ImportFromFile";
        private void ImportFromFile()
        {
            string path = EditorUtility.OpenFilePanel("Import from file", string.Empty, string.Empty);

            if (string.IsNullOrEmpty(path)) return;

            using (FileStream fs = File.OpenRead(path))
            using (StreamReader tr = new StreamReader(fs))
            {
                LevelConfig[] levels = configs;

                for (int i = 0; i < levels.Length; i++)
                {
                    string line = tr.ReadLine();
                    string[] values = line.Split(new string[] { separator }, StringSplitOptions.RemoveEmptyEntries);

                    levels[i].HealthPinata = float.Parse(values[1]);
                    levels[i].WinCoinsPersentHealthPinata = float.Parse(values[2]);
                    levels[i].GemsForReset = float.Parse(values[3]);
                }
            }

            Debug.Log("Done import levels from file: " + path);
        }

        [Space]
        [Header("CloneLevels")]
        [SerializeField]
        private int startLevel = 0;
        [SerializeField]
        private int count = 0;
        [Button]
        public string ButtonCloneLevels = "Clone";
        private void Clone()
        {
            if (startLevel < 0 || configs.Length <= startLevel)
            {
                EditorUtility.DisplayDialog("Error", "Invalid start level", "Ok");
                return;
            }

            if (startLevel + count < 0 || startLevel + configs.Length <= count)
            {
                EditorUtility.DisplayDialog("Error", "Invalid count", "Ok");
                return;
            }

            int oldSize = configs.Length;
            Array.Resize(ref configs, configs.Length + count);
            for (int i = 0; i < count; i++)
            {
                configs[oldSize + i] = configs[startLevel + i];
            }
        }


        private void OnValidate()
        {
            partPinataHealth = partPinataHealth > 0 ? partPinataHealth : 0;

            for (int i = 0; i < configs.Length; i++)
            {
                if (!configs[i].UseCustomHealth)
                {
                    configs[i].PinataObstacleHealth = partPinataHealth;
                    configs[i].LevelObstacleHealth = partPinataHealth;
                }
            }
        }
#endif

    }
}
