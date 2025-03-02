using System;
using System.Collections.Generic;
using System.IO;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    /// <summary>
    /// A simple class to edit content of gradle properties file.
    /// </summary>
    public class GradleProperties : IDisposable
    {
        public const string DefaultFileName = "gradle.properties";


        public Dictionary<string, string> Data { get; private set; }


        /// <summary>
        /// Gets a path to save the gradle properties file.
        /// </summary>
        public string OutputPath { get; private set; }


        #region Instancing

        /// <summary>
        /// Opens a gradle properties file by path.
        /// </summary>
        /// <param name="path">A path to manifest.</param>
        public static GradleProperties Open(string path)
        {
            Dictionary<string, string> content = new Dictionary<string, string>();

            using (StreamReader reader = File.OpenText(path))
            {
                while (reader.Peek() >= 0)
                {
                    string line = reader.ReadLine().Trim();

                    // Skip comments
                    if (line.StartsWith("//") || line.StartsWith("#"))
                    {
                        continue;
                    }

                    // Parse line
                    string[] pair = line.Split('=');
                    if (pair.Length != 2)
                    {
                        throw new FormatException($"Failed to parse line '{line}'");
                    }

                    content[pair[0].Trim()] = pair[1].Trim();
                }
            }

            return new GradleProperties(path, content);
        }


        /// <summary>
        /// Opens gradle.properties file located at specified folder.
        /// </summary>
        /// <param name="folderPath">A path to gradle.properties file.</param>
        public static GradleProperties OpenInFolder(string folderPath)
        {
            return Open(UnityPath.Combine(folderPath, DefaultFileName));
        }


        /// <summary>
        /// Creates a new gradle properties file.
        /// </summary>
        /// <param name="path">Target path to gradle properties, include file name.</param>
        public static GradleProperties Create(string path)
        {
           return new GradleProperties(path, null);
        }


        /// <summary>
        /// Creates a new gradle properties file in specified folder.
        /// </summary>
        /// <param name="folderPath">A path to folder where the gradle properties file will be created.</param>
        public static GradleProperties CreateInFolder(string folderPath)
        {
            return Create(UnityPath.Combine(folderPath, DefaultFileName));
        }


        private GradleProperties(string path, Dictionary<string, string> content)
        {
            OutputPath = path;
            Data = content ?? new Dictionary<string, string>();
        }


        public void Dispose()
        {
            if (Data == null)
            {
                return;
            }

            Save();
            Data = null;
            OutputPath = null;
        }

        #endregion



        #region Data management

        public bool ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }


        public bool RemoveKey(string key)
        {
            return Data.Remove(key);
        }


        public bool? GetBoolean(string key)
        {
            if (!Data.TryGetValue(key, out string value))
            {
                return null;
            }

            return bool.Parse(value);
        }


        public void SetBoolean(string key, bool value)
        {
            Data[key] = value ? "true" : "false";
        }


        public int? GetInt(string key)
        {
            if (!Data.TryGetValue(key, out string value))
            {
                return null;
            }

            return int.Parse(value);
        }


        public void SetInt(string key, int value)
        {
            Data[key] = value.ToString();
        }


        public string GetString(string key)
        {
            if (!Data.TryGetValue(key, out string value))
            {
                return null;
            }

            return value;
        }


        public void SetString(string key, string value)
        {
            Data[key] = value;
        }

        #endregion



        #region AndroidX and Jetifier

        public bool IsAndroidXEnabled
        {
            // Gradle considers the value of android.useAndroidX is false by default
            get => GetBoolean("android.useAndroidX") ?? false;
            set => SetBoolean("android.useAndroidX", value);
        }


        public bool IsJetifierEnabled
        {
            // Gradle considers the value of android.enableJetifier is false by default
            get => GetBoolean("android.enableJetifier") ?? false;
            set => SetBoolean("android.enableJetifier", value);
        }

        #endregion



        #region Other properties and methods

        /// <summary>
        /// Saves manifest.
        /// </summary>
        public void Save()
        {
            using (StreamWriter writer = File.CreateText(OutputPath))
            {
                foreach (var property in Data)
                {
                    writer.Write(property.Key);
                    writer.Write('=');
                    writer.WriteLine(property.Value);
                }
            }
        }

        #endregion
    }
}
