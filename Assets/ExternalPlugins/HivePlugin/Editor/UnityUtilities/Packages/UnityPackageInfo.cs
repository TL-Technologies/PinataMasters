using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;


namespace Modules.Hive.Editor
{
    // Class for package.json file serialization and deserialization purposes
    [Serializable]
    public class UnityPackageInfo
    {
        public string name = null;
        public string version = null;
        public string displayName = null;
        public string description = null;
        public string unity = null;
        public string type = null;
        public Dictionary<string, string> dependencies = null;
        
        
        public static UnityPackageInfo Open(string path)
        {
            return !File.Exists(path) ?
                null :
                JsonConvert.DeserializeObject<UnityPackageInfo>(File.ReadAllText(path));
        }
    }
}
