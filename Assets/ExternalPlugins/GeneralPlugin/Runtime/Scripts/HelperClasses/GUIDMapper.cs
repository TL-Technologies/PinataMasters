using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Modules.General.HelperClasses
{
    public static class GUIDMapper 
    {
        #region Fields
    
        #if !UNITY_EDITOR
        static Dictionary<string, string> guidMap = new Dictionary<string, string>();
        static bool isParse;
        #endif
    
        #endregion
    
    
        #region Public methods
    
        public static string GUIDToAssetPath(string guid)
        {
            string path = string.Empty;
            #if UNITY_EDITOR
                path = AssetDatabase.GUIDToAssetPath(guid);
            #else
                if (!isParse)
                {
                    Parse();
                }
                if (!guidMap.TryGetValue(guid, out path))
                {
                    CustomDebug.Log("GUIDMapper.Log can't find path for Guid : " + guid);
                }
            #endif
            
            return path;
        }
    
        #endregion
    
    
        #region Private methods
    
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void RuntimeInitialize()
        {
            #if !UNITY_EDITOR
                if (!isParse)
                {
                    Parse();
                }
            #endif
        }
        
    
        #if !UNITY_EDITOR
        static void Parse()
        {
            if (!isParse)
            {
                string filePath = Application.streamingAssetsPath.AppendPathComponent("guids");
                string result = string.Empty;
                bool isCorrect = false;
    
                float time = Time.realtimeSinceStartup;
    
                if (filePath.Contains(PathUtils.WWW_FILE_PREFIX))
                {
                    using (WWW wwwFile = new WWW(filePath))
                    {
                        while (!wwwFile.isDone) { System.Threading.Thread.Sleep(1); }
                        isCorrect = string.IsNullOrEmpty(wwwFile.error);
                        if (isCorrect)
                        {
                            result = wwwFile.text;
                        }
                    }
                }
                else
                {
                    isCorrect = File.Exists(filePath);
                    if (isCorrect)
                    {
                        result = File.ReadAllText(filePath);
                    }
                }
    
                if (isCorrect)
                {
                    guidMap = MiniJSON.Json.Deserialize<Dictionary<string, string>>(result);
    
                    time = (Time.realtimeSinceStartup - time) * 1000.0f;
                    CustomDebug.Log("GUIDMapper.Parse by : " + time.ToString("f10") + " milliseconds for " + guidMap.Keys.Count);
                }
                else
                {
                    CustomDebug.LogError("GUIDMapper.Error can't find file : " + filePath);
                }
                isParse = true;
            }
        }
        #endif
    
        #endregion
    }
}

