using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.HelperClasses
{
    public static class BuildInfo
    {
        #region Fields
        
        public const string BuildNumberKey = "BuildNumber";
        public const string BranchKey = "GitBranch";
        public const string GitCommitHashKey = "GitCommitHash";
        private const string BuildInfoFileResourcePath = "jenkins_build_info";
        
        private static Dictionary<string, string> buildInfoDictionary;
        
        #endregion
        
        
        
        #region Properties
    
        public static bool IsDebugBuild
        {
            get
            {
                #if DEBUG_TARGET
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        
        public static bool IsUaBuild
        {
            get
            {
                #if UA_BUILD
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        
        public static bool IsChinaBuild
        {
            get
            {
                #if CHINA_BUILD
                    return true;
                #else
                    return false;
                #endif
            }
        }
        
        
        private static Dictionary<string, string> BuildInfoDictionary
        {
            get
            {
                if (buildInfoDictionary == null)
                {
                    TextAsset jsonTextFile = Resources.Load<TextAsset>(BuildInfoFileResourcePath);
                    buildInfoDictionary = jsonTextFile != null ?
                        JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonTextFile.text) :
                        new Dictionary<string, string>();
                }
                
                return buildInfoDictionary;
            }
        }
    
        #endregion
        
        
        
        #region Methods
        
        public static string GetInfoByKey(string key)
        {
            if (!BuildInfoDictionary.TryGetValue(key, out string result))
            {
                result = string.Empty;
            }
            
            return result;
        }
        
        #endregion
    }
}
