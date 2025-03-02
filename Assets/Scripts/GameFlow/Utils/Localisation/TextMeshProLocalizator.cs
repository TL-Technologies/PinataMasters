using System.Collections;
using TMPro;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(TMP_Text))]
    public class TextMeshProLocalizator : Localizator
    {
        #region Variables

        private TMP_Text text;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
        }

        #endregion



        #region Private methods

        protected override void OnLanguageChanged()
        {
            text.font = Localisation.LocalizedTMPFont();
        }


        protected override void SetText(string value)
        {
            text = text ?? GetComponent<TMP_Text>();
            text.text = value;
            Sheduler.PlayCoroutine(Refresh());
        }


        private IEnumerator Refresh()
        {
            yield return new WaitForSeconds(0.2f);
            text.SetAllDirty();
        }

        #endregion
    }
}
