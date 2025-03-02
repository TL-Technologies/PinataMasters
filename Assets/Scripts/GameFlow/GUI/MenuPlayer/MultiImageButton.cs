using UnityEngine;
using UnityEngine.UI;


namespace PinataMasters
{
    public class MultiImageButton : MonoBehaviour
    {
        #region Types

        [System.Serializable]
        private class MultiImage
        {
            public Image Image = null;
            public Sprite Normal = null;
            public Color NormalColor = Color.white;
            public Sprite Disabled = null;
            public Color DisabledColor = Color.white;
        }

        #endregion



        #region Variables

        [SerializeField]
        private MultiImage[] multiImage = null;

        #endregion



        #region Public methods
        
        public void Interactable(bool enable)
        {
            foreach (MultiImage image in multiImage)
            {
                image.Image.sprite = enable ? image.Normal : image.Disabled;
                image.Image.color = enable ? image.NormalColor : image.DisabledColor;
            }
        }

        #endregion
    }
}