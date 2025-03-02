using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public class GradleScript : IReadOnlyGradleScript, IDisposable
    {
        private const string SemanticPrefix = "HIVE_";
        private const string MultidexEnabledKey = "HIVE_MULTIDEX_ENABLED";

        private Dictionary<string, Tuple<GradleType, List<IGradleScriptElement>>> gradleElements = 
            new Dictionary<string, Tuple<GradleType, List<IGradleScriptElement>>>();

        
        public string TemplatesDirectoryPath { get; }
        public string OutputDirectoryPath { get; }
        public ExtendedVersion GradlePackageVersion { get; }
        public bool IsMultiDexEnabled { get; }
        public bool IsDisposed { get; private set; }


        public GradleScript(string templatesDirectoryPath, string outputDirectoryPath)
        {
            if (string.IsNullOrWhiteSpace(templatesDirectoryPath))
            {
                throw new ArgumentNullException(nameof(templatesDirectoryPath));
            }

            IsMultiDexEnabled = true;
            TemplatesDirectoryPath = templatesDirectoryPath;
            OutputDirectoryPath = outputDirectoryPath;

            // Add dependency to gradle package
            GradleDependency gradlePackageDependency = 
                GradleScriptUtilities.GetGradlePackageDependency(templatesDirectoryPath) ??
                GradleScriptUtilities.GetGradlePackageDependency(GradleScriptUtilities.GradleTemplatesUnityDirectory);
            GradlePackageVersion = gradlePackageDependency.Version;
            this.AddDependency(GradleDependencyPlacement.BuildScript, gradlePackageDependency);
        }


        public void Dispose()
        {
            if (IsDisposed)
            {
                return;
            }

            Save();
            IsDisposed = true;
        }


        #region Elements management

        public void AddElement(string semantic, GradleType gradleType, IGradleScriptElement element)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(GradleScript));
            }
            if (string.IsNullOrEmpty(semantic))
            {
                Debug.LogError("Failed to add gradle element with empty semantic");
                return;
            }

            element.Validate();

            if (!gradleElements.TryGetValue(semantic, out Tuple<GradleType, List<IGradleScriptElement>> semanticTuple))
            {
                semanticTuple = new Tuple<GradleType, List<IGradleScriptElement>>(gradleType, new List<IGradleScriptElement>(1));
                gradleElements.Add(semantic, semanticTuple);
            }
            semanticTuple.Item2.Add(element);
        }
        

        public IEnumerable<IGradleScriptElement> GetElementsEnumerable(params string[] semantics)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(GradleScript));
            }

            return gradleElements
                .Where(p => semantics.Contains(p.Key))
                .SelectMany(p => p.Value.Item2);
        }


        public IEnumerable<T> GetElementsEnumerable<T>(params string[] semantics) where T : class, IGradleScriptElement
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(GradleScript));
            }

            return gradleElements
                .Where(p => semantics.Contains(p.Key))
                .SelectMany(p => p.Value.Item2)
                .Select(p => p as T)
                .Where(p => p != null);
        }
        
        
        private IEnumerable<IGradleScriptElement> GetElementsByGradleType(GradleType gradleType, params string[] semantics)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(GradleScript));
            }

            return gradleElements
                .Where(p => semantics.Contains(p.Key) && p.Value.Item1 == gradleType)
                .SelectMany(p => p.Value.Item2);
        }

        #endregion



        #region Serialization

        public void Save(string directoryPath = null)
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(GradleScript));
            }

            string destinationDirectoryPath = !string.IsNullOrEmpty(directoryPath) ? directoryPath : OutputDirectoryPath;
            foreach (GradleType gradleType in GradleScriptUtilities.EnumerateGradleTemplateTypes())
            {
                string templateName = GradleScriptUtilities.GetGradleTemplateName(gradleType);
                string sourcePath = UnityPath.Combine(TemplatesDirectoryPath, templateName);
                if (File.Exists(sourcePath))
                {
                    string destinationPath = UnityPath.Combine(destinationDirectoryPath, templateName);
                    // In case if destinationPath equals sourcePath
                    string temporaryPath = FileUtil.GetUniqueTempPathInProject();
                    
                    using (StreamWriter writer = File.CreateText(temporaryPath))
                    using (StreamReader reader = File.OpenText(sourcePath))
                    {
                        while (!reader.EndOfStream)
                        {
                            string token = MoveToNextToken(reader, writer);
                            switch (token)
                            {
                                // Empty token. Do nothing
                                case null:
                                    break;
                                // Multidex
                                case MultidexEnabledKey:
                                    writer.Write(IsMultiDexEnabled.ToString().ToLower());
                                    break;
                                // Unknown token. Save it as is
                                default:
                                    foreach (IGradleScriptElement element in GetElementsByGradleType(gradleType, token))
                                    {
                                        writer.WriteLine(element.ToString(this));
                                    }
        
                                    if (!token.StartsWith(SemanticPrefix))
                                    {
                                        writer.Write("**");
                                        writer.Write(token);
                                        writer.Write("**");
                                    }
                                    break;
                            }
                        }
                    }
                    
                    File.Copy(temporaryPath, destinationPath, true);
                    File.Delete(temporaryPath);
                }
            }
        }


        private string MoveToNextToken(StreamReader reader, TextWriter writer)
        {
            bool isFound = false;

            // Write from istream to ostream until '**' has found;
            while (!reader.EndOfStream)
            {
                char c1 = (char)reader.Read();
                if (c1 != '*')
                {
                    writer.Write(c1);
                    continue;
                }

                if (reader.EndOfStream)
                {
                    writer.Write(c1);
                    return null;
                }

                char c2 = (char)reader.Read();
                if (c2 != '*')
                {
                    writer.Write(c1);
                    writer.Write(c2);
                    continue;
                }

                isFound = true;
                break;
            }

            if (!isFound)
            {
                return null;
            }

            // Parse token
            StringBuilder token = new StringBuilder();
            while (!reader.EndOfStream)
            {
                char c1 = (char)reader.Read();
                if (c1 != '*')
                {
                    token.Append(c1);
                    continue;
                }

                if (reader.EndOfStream)
                {
                    token.Append(c1);
                    break;
                }

                char c2 = (char)reader.Read();
                if (c2 != '*')
                {
                    token.Append(c1);
                    token.Append(c2);
                    continue;
                }

                break;
            }

            return token.ToString();
        }

        #endregion
    }
}
