using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public class InfoPlist : PlistDocument, IDisposable
    {
        public const string DefaultFileName = "Info.plist";

        /// <summary>
        /// Gets a path to save the plist-file.
        /// </summary>
        public string OutputPath { get; private set; }

        /// <summary>
        /// Gets settings of App Transport Security.
        /// </summary>
        public AppTransportSecurity AppTransportSecurity { get; }


        #region Instancing

        /// <summary>
        /// Opens a plist-file by path.
        /// </summary>
        /// <param name="path">A path to plist-file.</param>
        public static InfoPlist Open(string path)
        {
            return new InfoPlist(path, false);
        }


        /// <summary>
        /// Opens Info.plist file located at specified folder.
        /// </summary>
        /// <param name="folderPath">A path to Info.plist file</param>
        public static InfoPlist OpenInFolder(string folderPath)
        {
            return new InfoPlist(Path.Combine(folderPath, DefaultFileName), false);
        }


        /// <summary>
        /// Creates a new plist-file.
        /// </summary>
        /// <param name="path">Target path to the plist-file.</param>
        public static InfoPlist Create(string path)
        {
            return new InfoPlist(path, true);
        }


        /// <summary>
        /// Creates a new Info.plist file located at specified folder.
        /// </summary>
        /// <param name="path">Target path to the Info.plist.</param>
        public static InfoPlist CreateInFolder(string folderPath)
        {
            return new InfoPlist(Path.Combine(folderPath, DefaultFileName), true);
        }


        private InfoPlist(string path, bool createNew) : base()
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            OutputPath = path;

            bool fileExists = File.Exists(path);

            if (createNew)
            {
                if (fileExists)
                {
                    File.Delete(path);
                }
            }
            else
            {
                if (!fileExists)
                {
                    throw new FileNotFoundException("File not found.", path);
                }

                ReadFromFile(OutputPath);
            }

            // create wrappers
            AppTransportSecurity = new AppTransportSecurity(root);
        }


        public void Dispose()
        {
            if (OutputPath == null)
            {
                return;
            }

            Save();
            OutputPath = null;
        }

        #endregion



        #region General data management methods

        /// <summary>
        /// Returns an array instance by key or null.
        /// </summary>
        /// <param name="key">key string</param>
        /// <param name="createIfNotExists">create and add to the dictionary a new array if it doesn't exist</param>
        /// <returns></returns>
        public PlistElementArray GetArray(string key, bool createIfNotExists = false)
        {
            return root.GetArray(key, createIfNotExists);
        }


        /// <summary>
        /// Returns a dictionary instance by key or null.
        /// </summary>
        /// <param name="key">key string</param>
        /// <param name="createIfNotExists">create and add to the dictionary a new sub-dictionary if it doesn't exist</param>
        /// <returns></returns>
        public PlistElementDict GetDict(string key, bool createIfNotExists = false)
        {
            return root.GetDict(key, createIfNotExists);
        }


        public bool GetBoolean(string key, bool defaultValue)
        {
            return root.GetBoolean(key, defaultValue);
        }


        public string GetString(string key, string defaultValue)
        {
            return root.GetString(key, defaultValue);
        }


        public int GetInteger(string key, int defaultValue)
        {
            return root.GetInteger(key, defaultValue);
        }
        
        
        public void SetArray(string key, PlistElementArray value)
        {
            root.SetArray(key, value);
        }
        
        
        public void SetDict(string key, PlistElementDict value)
        {
            root.SetDict(key, value);
        }


        public void SetBoolean(string key, bool value)
        {
            root.SetBoolean(key, value);
        }


        public void SetString(string key, string value)
        {
            root.SetString(key, value);
        }


        public void SetInteger(string key, int value)
        {
            root.SetInteger(key, value);
        }
        

        public bool ContainsKey(string key)
        {
            return root.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return root.Remove(key);
        }

        #endregion



        #region Url Schemes

        // https://developer.apple.com/documentation/uikit/inter-process_communication/allowing_apps_and_websites_to_link_to_your_content/defining_a_custom_url_scheme_for_your_app?language=objc
        // <key>CFBundleURLTypes</key>
        // <array>
        //     <dict>
        //         <key>CFBundleTypeRole</key>
        //         <string>Editor</string>
        //         <key>CFBundleURLName</key>
        //         <string>id</string>
        //         <key>CFBundleURLSchemes</key>
        //         <array>
        //             <string>scheme1</string>
        //             <string>scheme2</string>
        //         </array>
        //     </dict>
        // </array>


        /// <summary>
        /// Adds new url schemes to Bundle Url Type with specified identifier.
        /// </summary>
        /// <param name="identifier">The identifier of the Bundle Url Type.</param>
        /// <param name="urlSchemes">The collection of new url schemes.</param>
        public void AddUrlSchemes(string identifier, params string[] urlSchemes)
        {
            PlistElementDict bundleUrlType = GetBundleUrlType(identifier, true);

            // Add role if not specified (viewer by default)
            string roleString = bundleUrlType.GetString("CFBundleTypeRole", null);
            if (string.IsNullOrEmpty(roleString))
            {
                bundleUrlType.SetString("CFBundleTypeRole", "Viewer");
            }

            // Load existed url schemes
            var bundleUrlSchemes = bundleUrlType.GetArray("CFBundleURLSchemes", true);
            HashSet<string> existedSchemes = new HashSet<string>();
            foreach (var element in bundleUrlSchemes.values)
            {
                string existedScheme = element.AsString();
                if (!string.IsNullOrEmpty(existedScheme))
                {
                    existedSchemes.Add(existedScheme);
                }
            }

            // Add new url schemes
            foreach (var scheme in urlSchemes)
            {
                if (!existedSchemes.Contains(scheme))
                {
                    bundleUrlSchemes.AddString(scheme);
                    existedSchemes.Add(scheme);
                }
            }
        }


        /// <summary>
        /// Adds new url schemes to Bundle Url Type with identifier that equals to PlayerSettings.applicationIdentifier.
        /// </summary>
        /// <param name="urlSchemes">The collection of new url schemes.</param>
        public void AddUrlSchemes(params string[] urlSchemes)
        {
            AddUrlSchemes(PlayerSettings.applicationIdentifier, urlSchemes);
        }


        private PlistElementDict GetBundleUrlType(string identifier, bool createIfNotExtsts = false)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            PlistElementArray bundleUrlTypes = GetArray("CFBundleURLTypes", true);

            // Get dictionary with CFBundleURLName == identifier
            foreach (var item in bundleUrlTypes.values)
            {
                PlistElementDict bundleUrlType = item.AsDict();
                if (bundleUrlType != null && bundleUrlType.GetString("CFBundleURLName", null) == identifier)
                {
                    return bundleUrlType;
                }
            }
            if (createIfNotExtsts)
            {
                PlistElementDict bundleUrlType = bundleUrlTypes.AddDict();
                bundleUrlType.SetString("CFBundleURLName", identifier);
                return bundleUrlType;
            }

            return null;
        }

        #endregion



        #region Application Queries Schemes

        /// <summary>
        /// Adds new application queries schemes to the plist file.
        /// </summary>
        /// <param name="queriesSchemes">The collection of new quiries schemes.</param>
        public void AddApplicationQueriesSchemes(params string[] queriesSchemes)
        {
            if (queriesSchemes == null || queriesSchemes.Length == 0)
            {
                return;
            }

            PlistElementArray queriesSchemesElement = GetArray("LSApplicationQueriesSchemes", true);

            // Load existed url schemes
            HashSet<string> existedSchemes = new HashSet<string>();
            foreach (var element in queriesSchemesElement.values)
            {
                string existedScheme = element.AsString();
                if (!string.IsNullOrEmpty(existedScheme))
                {
                    existedSchemes.Add(existedScheme);
                }
            }

            // Add new schemes
            foreach (var scheme in queriesSchemes)
            {
                if (!existedSchemes.Contains(scheme))
                {
                    queriesSchemesElement.AddString(scheme);
                    existedSchemes.Add(scheme);
                }
            }
        }

        #endregion



        #region Privacy Data Usage Description

        /// <summary>
        /// Sets a privacy-sensitive data usage description.
        /// </summary>
        /// <param name="privacyDataUsage">The privacy data usage enum (see <see cref="PrivacyDataUsage"/>).</param>
        /// <param name="description">The description.</param>
        /// <param name="overwrite">Should overwrite.</param>
        public void SetPrivacyDataUsageDescription(PrivacyDataUsage privacyDataUsage, string description, bool overwrite = false)
        {
            if (overwrite || string.IsNullOrEmpty(GetString(privacyDataUsage.Key, null)))
            {
                SetString(privacyDataUsage.Key, description);
            }
        }

        #endregion



        #region Other public properties and methods

        public bool AppUsesNonExemptEncryption
        {
            get => GetBoolean("ITSAppUsesNonExemptEncryption", false);
            set => SetBoolean("ITSAppUsesNonExemptEncryption", value);
        }


        public void Save()
        {
            WriteToFile(OutputPath);
        }


        public void Reload()
        {
            ReadFromFile(OutputPath);
        }

        #endregion
    }
}
