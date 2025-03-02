#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace PinataMasters
{
    public class MissingAssetsWindow : EditorWindow
    {
        private static readonly string[] skipableOnShow = {
                    "Animations", "TargetSettings", "Resources", "Mockup", ".unity", ".spriteatlas"
        };

        private static Vector2 scrollPos = new Vector2(100, 100);
        private static List<AssetInfo> info;

        public static void ShowResult(List<AssetInfo> list = null)
        {
            info = list;
            MissingAssetsWindow window = (MissingAssetsWindow)GetWindow(typeof(MissingAssetsWindow));

            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.LabelField("FOUND MISSED ASSET:");

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

                        EditorGUILayout.PrefixLabel("IN ASSET WITH PATH: ");
                        EditorGUILayout.TextField(asset.Path);
                        GUILayout.Space(position.width / 4f);
                        EditorGUILayout.PrefixLabel("MISSED GUID:");
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