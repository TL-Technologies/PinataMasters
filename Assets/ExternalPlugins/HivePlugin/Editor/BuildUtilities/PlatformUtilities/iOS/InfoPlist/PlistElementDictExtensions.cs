#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    /// <summary>
    /// Extensions of PlistElementDict class
    /// </summary>
    public static class PlistElementDictExtensions
    {
        public static void SetArray(this PlistElementDict dict, string key, PlistElementArray val)
        {
            dict.values[key] = val;
        }

        
        
        public static void SetDict(this PlistElementDict dict, string key, PlistElementDict val)
        {
            dict.values[key] = val;
        }


        /// <summary>
        /// Returns an array instance by key or null.
        /// </summary>
        /// <param name="dict">target dictionary</param>
        /// <param name="key">key string</param>
        /// <param name="createIfNotExists">create and add to the dictionary a new array if it doesn't exist</param>
        /// <returns></returns>
        public static PlistElementArray GetArray(this PlistElementDict dict, string key, bool createIfNotExists = false)
        {
            if (dict.values.TryGetValue(key, out PlistElement element))
            {
                return element.AsArray();
            }

            return createIfNotExists ? dict.CreateArray(key) : null;
        }


        /// <summary>
        /// Returns a dictionary instance by key or null.
        /// </summary>
        /// <param name="dict">target dictionary</param>
        /// <param name="key">key string</param>
        /// <param name="createIfNotExists">create and add to the dictionary a new sub-dictionary if it doesn't exist</param>
        /// <returns></returns>
        public static PlistElementDict GetDict(this PlistElementDict dict, string key, bool createIfNotExists = false)
        {
            if (dict.values.TryGetValue(key, out PlistElement element))
            {
                return element.AsDict();
            }

            return createIfNotExists ? dict.CreateDict(key) : null;
        }


        public static bool GetBoolean(this PlistElementDict dict, string key, bool defaultValue)
        {
            if (dict.values.TryGetValue(key, out PlistElement element))
            {
                return element.AsBoolean();
            }

            return defaultValue;
        }


        public static string GetString(this PlistElementDict dict, string key, string defaultValue)
        {
            if (dict.values.TryGetValue(key, out PlistElement element))
            {
                return element.AsString();
            }

            return defaultValue;
        }


        public static int GetInteger(this PlistElementDict dict, string key, int defaultValue)
        {
            if (dict.values.TryGetValue(key, out PlistElement element))
            {
                return element.AsInteger();
            }

            return defaultValue;
        }
        
        
        public static bool ContainsKey(this PlistElementDict dict, string key)
        {
            return dict.values.ContainsKey(key);
        }

        public static bool Remove(this PlistElementDict dict, string key)
        {
            return dict.values.Remove(key);
        }

    }
}
