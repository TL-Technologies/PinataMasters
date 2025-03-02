using UnityEngine;

namespace UnityEditor.Purchasing
{
    internal class RichEditorWindow : EditorWindow
    {
        private const string kLightLinkIconPath = "Icons/LinkWhite.png";
        private const string kDarkLinkIconPath = "Icons/LinkBlack.png";
        
        private GUIStyle m_LinkStyle;
        private Texture m_LinkIcon;
        private string m_iconPath;
        
        internal RichEditorWindow()
        {
        }

        internal void GUILink(string linkText, string url)
        {
            m_LinkStyle = m_LinkStyle ?? new GUIStyle();
            m_LinkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.cyan : Color.blue;
            m_LinkStyle.contentOffset = new Vector2(6, 0); // Indent like other labels

            if (m_LinkIcon == null)
            {
                string iconName = EditorGUIUtility.isProSkin ? kLightLinkIconPath : kDarkLinkIconPath;
                m_iconPath = UnityIapPluginHierarchy.Instance.GetPathWithRoot(iconName);
                m_LinkIcon = AssetDatabase.LoadAssetAtPath<Texture>(m_iconPath);
            }

            var linkSize = m_LinkStyle.CalcSize(new GUIContent(linkText));
            GUILayout.Label(linkText, m_LinkStyle);
            var linkRect = GUILayoutUtility.GetLastRect();

            if (m_LinkIcon != null)
                GUI.Label(new Rect(linkSize.x, linkRect.y, linkRect.height, linkRect.height), m_LinkIcon);
            else
            {
                Debug.LogWarning("Cannot get icon: " + m_iconPath);
            }

            if (Event.current.type == EventType.MouseUp && linkRect.Contains(Event.current.mousePosition))
                Application.OpenURL(url);
        }

    }
}
