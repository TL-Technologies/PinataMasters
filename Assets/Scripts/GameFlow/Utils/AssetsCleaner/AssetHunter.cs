// TODO: Use at your own risk

#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace PinataMasters
{
    public class AssetInfo
    {
        public string Guid;
        public string Path;
        public int usingCounter;
        public List<string> dependences;
        public bool isPrefab;
    }

    public class AssetHunter
    {
        #region Variables

        private static readonly string[] skipableFolders = { "AppsFlyerPlugin", "FacebookPlugin",
            "FirebasePlugin", "GeneralPlugin", "IronSourcePlugin", "PrivacyPlugin",
            "PromofetchPlugin", "TapdaqPlugin", "VibrationPlugin", "SharedScripts",
            "SharedsPlugins", "Plugins"};

        private static List<AssetInfo> assets = null;

        #endregion



        #region Private methods

        [MenuItem("AssetCleaner/Find Unused Assets")]
        private static void FindUnusedAssets()
        {
            AssetDatabase.Refresh();
            FillAssetsInfo();

            foreach (var asset in assets)
            {
                asset.dependences = new List<string>();
                List<string> dependencies = new List<string>(AssetDatabase.GetDependencies(asset.Path));

                if (asset.isPrefab)
                {
                    Regex rx = new Regex("assetGUID: ([0-9a-fA-F]{32})");

                    using (StreamReader sr = File.OpenText(asset.Path))
                    {
                        while (true)
                        {
                            string prefabText = sr.ReadLine();
                            if (string.IsNullOrEmpty(prefabText)) break;

                            if (rx.IsMatch(prefabText))
                            {
                                string assetLinkPath = AssetDatabase.GUIDToAssetPath(rx.Match(prefabText).Groups[1].ToString());

                                if (!string.IsNullOrEmpty(assetLinkPath) && !assetLinkPath.Equals(asset.Path))
                                {
                                    dependencies.Add(assetLinkPath);
                                }
                            }
                        }
                    }
                }

                foreach (var i in dependencies)
                {
                    if (!i.Equals(asset.Path))
                    {
                        asset.dependences.Add(i);
                    }
                }
            }

            foreach (var asset in assets)
            {
                foreach (var asset2 in assets)
                {
                    if (asset2.dependences.Contains(asset.Path) && !asset2.Guid.Equals(asset.Guid))
                    {
                        asset.usingCounter++;
                    }
                }
            }

            var distinctAssets = assets.GroupBy(item => item.Path).Select(item => item.First()).ToList();

            UnusedAssetsWindow.ShowResult(distinctAssets);
        }



        private static void FillAssetsInfo()
        {
            assets = new List<AssetInfo>();

            string[] guids = AssetDatabase.FindAssets(string.Empty, null);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);

                bool shouldAdd = true;

                foreach (var name in skipableFolders)
                {
                    if (path.Contains(name))
                    {
                        shouldAdd = false;
                    }
                }

                if (shouldAdd && path.Contains(".") && !path.Contains(".cs"))
                {
                    AssetInfo info = new AssetInfo { Guid = guid, Path = path };

                    info.isPrefab = path.Contains(".prefab") || path.Contains(".asset");
                    assets.Add(info);
                }
            }
        }


        [MenuItem("AssetCleaner/Find Missing Asset")]
        private static void FindMissingAssets()
        {
            List<AssetInfo> missedAssets = new List<AssetInfo>(); 

            AssetDatabase.Refresh();
            FillAssetsInfo();
            Regex guidRegex = new Regex("(guid|assetGUID): ([0-9a-fA-F]{32})");

            foreach (var asset in assets)
            {
                using (StreamReader sr = File.OpenText(asset.Path))
                {
                    while (true)
                    {
                        string prefabText = sr.ReadLine();
                        if (string.IsNullOrEmpty(prefabText)) break;

                        if (guidRegex.IsMatch(prefabText))
                        {
                            string assetLinkPath = AssetDatabase.GUIDToAssetPath(guidRegex.Match(prefabText).Groups[2].ToString());
                            if (string.IsNullOrEmpty(assetLinkPath))
                            {
                                missedAssets.Add(new AssetInfo { Path = asset.Path, Guid = guidRegex.Match(prefabText).Groups[2].ToString() });
                            }
                        }
                    }
                }
            }

            var distinctAssets = missedAssets.GroupBy(item => item.Path).Select(item => item.First()).ToList();

            MissingAssetsWindow.ShowResult(distinctAssets);
        }
    }

    #endregion
}
#endif