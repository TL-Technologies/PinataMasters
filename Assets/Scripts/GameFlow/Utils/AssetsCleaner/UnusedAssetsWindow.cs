#if UNITY_EDITOR
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    public class UnusedAssetsWindow : EditorWindow
    {
        private static readonly string[] skipableOnShow = {
                    "Animations", "TargetSettings", "Resources", "Mockup", ".unity", ".spriteatlas"
        };

        private static Vector2 scrollPos = new Vector2(100,100);
        private static List<AssetInfo> info;

        public static void ShowResult(List<AssetInfo> list = null)
        {
            info = list;
            UnusedAssetsWindow window = (UnusedAssetsWindow)GetWindow(typeof(UnusedAssetsWindow));

            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.LabelField("FOUND UNUSED ASSET:");

            foreach (var asset in info)
            {
                if (asset.usingCounter == 0)
                {
                    bool shouldShow = true;
                    foreach (var i in skipableOnShow)
                    {
                        if (asset.Path.Contains(i))
                        {
                            shouldShow = false;
                        }
                    }

                    if (shouldShow)
                    {
                        EditorGUILayout.BeginHorizontal();

                        EditorGUILayout.PrefixLabel("PATH: ");
                        EditorGUILayout.TextField(asset.Path);
                        GUILayout.Space(position.width / 4f);
                        EditorGUILayout.PrefixLabel("GUID:");
                        EditorGUILayout.TextField(asset.Guid);

                        EditorGUILayout.EndHorizontal();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }
    }
}
#endif
