using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    [RequireComponent(typeof(Text))]
    public class TextLocalizator : Localizator
    {
        #region Fields

        Text text;

        #endregion



        #region Unity lifecycle

        void Awake()
        {
            text = GetComponent<Text>();
        }

        #endregion



        #region Private methods

        protected override void OnLanguageChanged()
        {
            text.font = Localisation.LocalizedFont();
        }


        protected override void SetText(string value)
        {
            text.text = value;
        }

        #endregion
    }
}
