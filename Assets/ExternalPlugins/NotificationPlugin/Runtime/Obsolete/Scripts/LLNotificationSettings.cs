using Modules.General.HelperClasses;
using UnityEngine;


namespace Modules.Notification.Obsolete
{
    [CreateAssetMenu(fileName = "LLNotificationSettings")]
    public class LLNotificationSettings : ScriptableSingleton<LLNotificationSettings>
    {
        #region Fields

        [Header("Notification Texture")] 
        [SerializeField] private Texture notificationIconTexture = null;

        [Header("View Textures")]
        [SerializeField] private Texture viewIconTexture = null;

        [SerializeField] private Texture viewBackgroundTexture = null;

        [Header("Text Colors")] 
        [SerializeField] private Color32 titleColor = Color.black;

        [SerializeField] private Color32 descriptionColor = Color.black;

        #endregion


        
        #region Properties

        public Texture NotificationIconTexture => notificationIconTexture;
        
        public Texture ViewIconTexture => viewIconTexture;

        public Texture ViewBackgroundTexture => viewBackgroundTexture;

        public Color32 TitleColor => titleColor;

        public Color32 DescriptionColor => descriptionColor;

        #endregion
    }
}