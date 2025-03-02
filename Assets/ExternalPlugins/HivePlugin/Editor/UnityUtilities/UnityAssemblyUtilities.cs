using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Assembly = System.Reflection.Assembly;


namespace Modules.Hive.Editor
{
    public static class UnityAssemblyUtilities
    {
        #region Fields
        
        private const string AsmdefSearchPattern = "t:asmdef";
        private const string ObsoleteOptionalReferenceForTests = "TestAssemblies";
        private const string TestsDefineConstraint = "UNITY_INCLUDE_TESTS";
        private const string UnityEditorAssembliesNamespace = "UnityEditor";
        
        private static readonly PreprocessorDirectivesCollection Directives = new PreprocessorDirectivesCollection(
            PlayerSettings.GetScriptingDefineSymbolsForGroup(
                BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget)));
        private static readonly HashSet<string> ReferencedAssemblies = new HashSet<string>();
        private static readonly HashSet<string> PredefinedUnityAssemblies = new HashSet<string> 
            { "Assembly-CSharp", "Assembly-CSharp-firstpass" };
        
        private static List<string> appDomainAssemblies = new List<string>();

        #endregion



        #region Methods
        
        public static bool IsAssemblyIncludedInBuild(string assemblyName)
        {
            if (string.IsNullOrEmpty(assemblyName))
            {
                return false;
            }
            if (PredefinedUnityAssemblies.Contains(assemblyName))
            {
                return true;
            }
            
            if (ReferencedAssemblies.Count == 0)
            {
                appDomainAssemblies = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Select(assembly => assembly.GetName().Name)
                    .ToList();
                foreach (string predefinedAssemblyName in PredefinedUnityAssemblies)
                {
                    if (appDomainAssemblies.Contains(predefinedAssemblyName))
                    {
                        ReferencedAssemblies.Add(predefinedAssemblyName);
                        LoadAssembliesRecursively(predefinedAssemblyName);
                    }
                }
                
                // Implementation note: we can't only recursively search from predefined Unity assemblies to their references
                // and so on, because assemblies with AutoReferenced option will not be referenced during Editor time.
                // So we force adding AutoReferenced assemblies (and their transitive dependencies) to result.
                IEnumerable<UnityAssemblyDefinition> projectAsmdefs = AssetDatabase
                    .FindAssets(AsmdefSearchPattern)
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(UnityAssemblyDefinition.OpenAtPath);
            
                foreach (UnityAssemblyDefinition assembly in projectAsmdefs)
                {
                    if (assembly.AutoReferenced && IsAssemblyApplicableForBuild(assembly))
                    {
                        if (ReferencedAssemblies.Add(assembly.Name))
                        {
                            LoadAssembliesRecursively(assembly.Name);
                        }
                    }
                }
                
                // If any assembly was reloaded, we should actualize references list
                AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
                AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
            }
            
            return ReferencedAssemblies.Contains(assemblyName);
            
            
            void LoadAssembliesRecursively(string name)
            {
                foreach (string referencedAssemblyName in GetUnityReferencesAssemblies(name))
                {
                    if (!referencedAssemblyName.StartsWith(UnityEditorAssembliesNamespace))
                    {
                        UnityAssemblyDefinition asmdef = UnityAssemblyDefinition.OpenForAssemblyName(referencedAssemblyName);
                        if (asmdef == null || IsAssemblyApplicableForBuild(asmdef))
                        {
                            if (ReferencedAssemblies.Add(referencedAssemblyName))
                            {
                                LoadAssembliesRecursively(referencedAssemblyName);
                            }
                        }
                    }
                }
            }
            
            
            bool IsAssemblyApplicableForBuild(UnityAssemblyDefinition assemblyDefinition)
            {
                bool result = true;
                if (assemblyDefinition.IsEditorOnly)
                {
                    result = false;
                }
                else
                {
                    List<string> defineConstraints = assemblyDefinition.DefineConstraints;
                    if (assemblyDefinition.OptionalUnityReferences.Contains(ObsoleteOptionalReferenceForTests))
                    {
                        defineConstraints.Add(TestsDefineConstraint);
                    }
                    foreach (string defineConstraint in defineConstraints)
                    {
                        if (!Directives.Contains(defineConstraint))
                        {
                            result = false;
                            break;
                        }
                    }
                }
                
                return result;
            }
            
            
            void OnBeforeAssemblyReload()
            {
                appDomainAssemblies.Clear();
                ReferencedAssemblies.Clear();
            }
            
            
            IEnumerable<string> GetUnityReferencesAssemblies(string name)
            {
                List<string> result = new List<string>();

                try
                {
                    result = Assembly
                        .Load(name)
                        .GetReferencedAssemblies()
                        .Select(s => s.Name)
                        .ToList();
                }
                catch (Exception)
                {
                    // Ignored
                }

                UnityAssemblyDefinition asmdef = UnityAssemblyDefinition.OpenForAssemblyName(name);
                if (asmdef != null)
                {
                    foreach (string s in asmdef.ReferencesList)
                    {
                        // We need last check in case of broken references in asmdef
                        if (!string.IsNullOrEmpty(s) && 
                            !result.Contains(s) && 
                            appDomainAssemblies.Contains(s))
                        {
                            result.Add(s);
                        }
                    }
                }
                
                return result;
            }
        }
        
        #endregion
    }
}
