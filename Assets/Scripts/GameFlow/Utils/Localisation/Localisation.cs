using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class Localisation : ScriptableObject
    {
        [Serializable]
        private struct LanguageAsset
        {
            public SystemLanguage language;
            public List<SystemLanguage> familiarLanguages;
            [ResourceLink]
            public AssetLink textAsset;
            public Font fontAsset;
            public TMP_FontAsset TMPfontAsset;
        }

        #region Variables

        public static event Action OnLanguageChanged = delegate { };

        private const string PATH_RESOURCES = "Localisation/Localisation";

        private const string LANGUAGE = "language";

        [SerializeField]
        private SystemLanguage defaultLanguage = SystemLanguage.Unknown;
        [Space]
        [SerializeField]
        private LanguageAsset[] languagesAssets = null;

        private Dictionary<string, string> internalText;

        private static Localisation instance;

        #endregion



        #region Properties

        private static SystemLanguage CurrentLanguage
        {
            get
            {
                if (!CustomPlayerPrefs.HasKey(LANGUAGE))
                {
                    CustomPlayerPrefs.SetInt(LANGUAGE, (int)GetInitLanguage());
                }

                return (SystemLanguage)CustomPlayerPrefs.GetInt(LANGUAGE, (int)SystemLanguage.Unknown);
            }
            set
            {
                CustomPlayerPrefs.SetInt(LANGUAGE, (int)value);
            }
        }


        private static Localisation Instance
        {
            get
            {
                instance = instance ?? (Localisation)Resources.Load(PATH_RESOURCES);
                return instance;
            }
        }

        #endregion



        #region Public methods

        public static string LocalizedStringOrSource(string source)
        {
            if (Instance.internalText == null)
            {
                ParseCurrentLanguageText();
            }

            string result;
            Instance.internalText.TryGetValue(source, out result);
            return result ?? source;
        }


        public static Font LocalizedFont()
        {
            return Array.Find(Instance.languagesAssets, element => element.language == CurrentLanguage).fontAsset;
        }


        public static TMP_FontAsset LocalizedTMPFont()
        {
            return Array.Find(Instance.languagesAssets, element => element.language == CurrentLanguage).TMPfontAsset;
        }


        public static void ChangeLanguage(SystemLanguage languageToChange)
        {
            if (languageToChange != CurrentLanguage)
            {
                if (Array.Exists(Instance.languagesAssets, element => element.language == languageToChange))
                {
                    CurrentLanguage = languageToChange;
                    ParseCurrentLanguageText();
                    OnLanguageChanged();
                }
                else
                {
                    CurrentLanguage = Instance.defaultLanguage;
                    ParseCurrentLanguageText();
                    OnLanguageChanged();
                    CustomDebug.LogWarning("No language asset for language: " + languageToChange);
                }
            }
        }


        public static void SetNextLanguage()
        {
            int index = Array.FindIndex(Instance.languagesAssets, element => element.language == CurrentLanguage) + 1;
            index = (index == Instance.languagesAssets.Length) ? 0 : index;
            ChangeLanguage(Instance.languagesAssets[index].language);
        }

        #endregion



        #region Private methods

        private static void ParseCurrentLanguageText()
        {
            Instance.internalText = new Dictionary<string, string>();

            TextAsset asset = (TextAsset)Array.Find(Instance.languagesAssets, element => element.language == CurrentLanguage).textAsset.GetAsset();
            string textToParse = asset.text;

            string[,] loadedText = CSVReader.SplitCsvGrid(textToParse);

            for (int y = 0; y < loadedText.GetUpperBound(1); y++)
            {
                if (!string.IsNullOrEmpty(loadedText[0, y]))
                {
                    if (Instance.internalText.ContainsKey(loadedText[0, y]))
                    {
                        CustomDebug.LogError("KEY ALLREADY EXISTS = " + loadedText[0, y]);
                    }
                    else
                    {
                        string value = loadedText[1, y];

                        if (!string.IsNullOrEmpty(value))
                        {
                            value = value.Replace(Constants.LocalizationTags.LINE, "\n");
                            value = value.Replace(Constants.LocalizationTags.COMMA, ",");

                            Instance.internalText.Add(loadedText[0, y], value);
                        }
                        else
                        {
                            CustomDebug.LogWarning("Null ref in key : " + loadedText[0, y]);
                        }
                    }
                }
            }
        }


        private static SystemLanguage GetInitLanguage()
        {
            int index = Array.FindIndex(Instance.languagesAssets, element => element.familiarLanguages.Contains(Application.systemLanguage) || element.language == Application.systemLanguage);

            if (index != -1)
            {
                return Instance.languagesAssets[index].language;
            }
            else
            {
                return Instance.defaultLanguage;
            }
        }

        #endregion
    }
}
