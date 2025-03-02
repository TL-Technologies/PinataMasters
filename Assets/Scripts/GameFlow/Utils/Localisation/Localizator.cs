using UnityEngine;


namespace PinataMasters
{
    public abstract class Localizator : MonoBehaviour
    {
        #region Types

        private enum Style
        {
            None,
            ToUpper,
            ToLower
        }

        #endregion



        #region Variables

        [SerializeField]
        protected string key = null;
        [SerializeField]
        private Style style = Style.None;

        protected object[] stringParams;
        #endregion



        #region Unity lifecycle

        protected virtual void Start()
        {
            Localisation.OnLanguageChanged += LanguageChanged;
            LanguageChanged();
        }


        void OnDestroy()
        {
            Localisation.OnLanguageChanged -= LanguageChanged;
        }

        #endregion



        #region Public methods

        public void SetKey(string key)
        {
            if (this.key == key) return;

            this.key = key;
            UpdateText();
        }

        public void SetParams(params object[] stringParams)
        {
            this.stringParams = stringParams;
            UpdateText();
        }

        #endregion



        #region private methods


        private void LanguageChanged()
        {
            OnLanguageChanged();
            UpdateText();
        }

        protected abstract void OnLanguageChanged();

        protected virtual void UpdateText()
        {
            string text;

            if (stringParams != null && stringParams.Length != 0)
            {
                text = string.Format(Localisation.LocalizedStringOrSource(key), stringParams);
            }
            else
            {
                text = Localisation.LocalizedStringOrSource(key);
            }

            text = SetStyle(text);
            SetText(text);
        }

        protected abstract void SetText(string value);


        private string SetStyle(string text)
        {
            switch (style)
            {
                case Style.ToUpper:
                    return text.ToUpper();

                case Style.ToLower:
                    return text.ToLower();

                default:
                    return text;
            }
        }

        #endregion
    }
}
