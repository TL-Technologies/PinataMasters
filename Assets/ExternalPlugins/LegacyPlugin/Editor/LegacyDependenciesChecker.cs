using Modules.Hive.Editor;
using System;
using System.Reflection;
using UnityEditor;


namespace Modules.Legacy.Editor
{
    internal static class LegacyDependenciesChecker
    {
        private const string DoTweenAssemblyName = "Modules.DOTween";
        private const string DoTweenEditorAssemblyName = "Modules.DOTween.Editor";
                
        private const string TmproMainTypeClassName = "TMP_Text";
        private const string TmProAssemblyName = "Unity.TextMeshPro";
        private const string TmProEditorAssemblyName = "Unity.TextMeshPro.Editor";
        private const string DoTweenTmproDefine = "DOTWEEN_TMPRO";
            
        private const string Tk2dMainTypeClassName = "tk2dBaseMesh";
        private const string Tk2dAssemblyName = "Modules.Legacy";
        private const string Tk2dEditorAssemblyName = "Modules.Legacy.Editor";
        private const string DoTweenTk2dDefine = "DOTWEEN_TOOLKIT_2D";
    
        private const string SpineAssemblyName = "Modules.Spine";
        private const string SpineEditorAssemblyName = "Modules.Spine.Editor";
        
        
        [MenuItem("Modules/Legacy/CheckLegacyDependencies")]
        [InitializeOnLoadMethod]
        private static void CheckLegacyDependencies()
        {
            // DoTween dependencies
            AssureDependencies(
                DoTweenAssemblyName,
                DoTweenEditorAssemblyName,
                TmproMainTypeClassName,
                TmProAssemblyName,
                TmProEditorAssemblyName,
                DoTweenTmproDefine);
                
            AssureDependencies(
                DoTweenAssemblyName,
                DoTweenEditorAssemblyName,
                Tk2dMainTypeClassName,
                Tk2dAssemblyName,
                Tk2dEditorAssemblyName,
                DoTweenTk2dDefine);
            
            // Spine dependencies
            AssureDependencies(
                SpineAssemblyName,
                SpineEditorAssemblyName,
                Tk2dMainTypeClassName,
                Tk2dAssemblyName,
                Tk2dEditorAssemblyName,
                string.Empty);
        }
        
        
        private static void AssureDependencies(
            string asmdefName,
            string editorAsmdefName,
            string dependencyMainTypeName,
            string dependencyAsmdefName,
            string dependencyEditorAsmdefName,
            string dependencyDefineName)
        {
            Type mainType = GetTypeByName(dependencyMainTypeName);
            bool isNeedAddDependencies = (mainType != null);
            if (!string.IsNullOrEmpty(dependencyDefineName))
            {
                if (isNeedAddDependencies)
                {
                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(BuildTargetGroup.Android, dependencyDefineName);
                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(BuildTargetGroup.iOS, dependencyDefineName);
                    PreprocessorDirectivesUtilities.AddPreprocessorDirectives(BuildTargetGroup.Standalone, dependencyDefineName);
                }
                else
                {
                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(BuildTargetGroup.Android, dependencyDefineName);
                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(BuildTargetGroup.iOS, dependencyDefineName);
                    PreprocessorDirectivesUtilities.RemovePreprocessorDirectives(BuildTargetGroup.Standalone, dependencyDefineName);
                }
            }
            
            UnityAssemblyDefinition assemblyDefinition = UnityAssemblyDefinition.OpenForAssemblyName(asmdefName);
            if (assemblyDefinition != null)
            {
                if (isNeedAddDependencies)
                {
                    assemblyDefinition.AddReferenceToAssembly(dependencyAsmdefName);
                }
                else
                {
                    assemblyDefinition.RemoveReferenceToAssembly(dependencyAsmdefName);
                }
                assemblyDefinition.Save();
            }
            
            UnityAssemblyDefinition editorAssemblyDefinition = UnityAssemblyDefinition.OpenForAssemblyName(editorAsmdefName);
            if (editorAssemblyDefinition != null)
            {
                if (isNeedAddDependencies)
                {
                    editorAssemblyDefinition.AddReferenceToAssembly(dependencyAsmdefName);
                    editorAssemblyDefinition.AddReferenceToAssembly(dependencyEditorAsmdefName);
                }
                else
                {
                    editorAssemblyDefinition.RemoveReferenceToAssembly(dependencyAsmdefName);
                    editorAssemblyDefinition.RemoveReferenceToAssembly(dependencyEditorAsmdefName);
                }
                editorAssemblyDefinition.Save();
            }
            
            
            Type GetTypeByName(string typeName)
            {
                Assembly[] currentAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (Assembly a in currentAssemblies)
                {
                    Type[] types = a.GetTypes();
    
                    foreach (Type t in types)
                    {
                        if (t.Name.Equals(typeName))
                        {
                            return t;
                        }
                    }
                }
    
                return null;
            }
        }
    }
}
