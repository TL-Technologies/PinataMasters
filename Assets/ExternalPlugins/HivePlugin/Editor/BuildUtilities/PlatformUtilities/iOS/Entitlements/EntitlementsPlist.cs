using System;
using System.IO;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#else
using Modules.Hive.Editor.BuildUtilities.Ios.Stub;
#endif


namespace Modules.Hive.Editor.BuildUtilities.Ios
{
    public class EntitlementsPlist : PlistDocument, IDisposable
    {
        public const string DefaultFileName = "project.entitlements";

        /// <summary>
        /// Gets a path to save the plist-file.
        /// </summary>
        public string OutputPath { get; private set; }


        /// <summary>
        /// Gets or sets value of aps-environment option.
        /// </summary>
        public ApsEnvironment? ApsEnvironment 
        {
            get
            {
                string value = root.GetString("aps-environment", null);
                if (value == null)
                {
                    return null;
                }

                return new ApsEnvironment(value);
            }
            set 
            {
                if (!value.HasValue)
                {
                    root.Remove("aps-environment");
                    return;
                }

                root.SetString("aps-environment", value.Value.Name);
            }
        }


        #region Instancing

        /// <summary>
        /// Opens a plist-file by path.
        /// </summary>
        /// <param name="path">A path to plist-file.</param>
        public static EntitlementsPlist Open(string path)
        {
            return new EntitlementsPlist(path, false);
        }


        /// <summary>
        /// Opens project.entitlements file located at specified folder.
        /// </summary>
        /// <param name="folderPath">A path to project.entitlements file</param>
        public static EntitlementsPlist OpenInFolder(string folderPath)
        {
            return new EntitlementsPlist(Path.Combine(folderPath, DefaultFileName), false);
        }


        /// <summary>
        /// Creates a new plist-file.
        /// </summary>
        /// <param name="path">Target path to the plist-file.</param>
        public static EntitlementsPlist Create(string path)
        {
            return new EntitlementsPlist(path, true);
        }


        /// <summary>
        /// Creates a new project.entitlements file located at specified folder.
        /// </summary>
        /// <param name="folderPath">Target path to the folder with project.entitlements.</param>
        public static EntitlementsPlist CreateInFolder(string folderPath)
        {
            return new EntitlementsPlist(Path.Combine(folderPath, DefaultFileName), true);
        }


        private EntitlementsPlist(string path, bool createNew) : base()
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



        #region Other public properties and methods

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
