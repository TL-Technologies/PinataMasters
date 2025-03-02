using System;
using UnityEditor;


namespace Modules.Hive.Editor
{
    public static class PreprocessorDirectivesUtilities
    {
        public static void AddPreprocessorDirectives(
            BuildTargetGroup targetGroup, 
            params string[] directives)
        {
            ModifyPreprocessorDirectives(targetGroup, directivesCollection =>
            {
                foreach (string directive in directives)
                {
                    directivesCollection.Add(directive);
                }
            });
        }
        
        
        public static void RemovePreprocessorDirectives(
            BuildTargetGroup targetGroup, 
            params string[] directives)
        {
            ModifyPreprocessorDirectives(targetGroup, directivesCollection =>
            {
                foreach (string directive in directives)
                {
                    directivesCollection.Remove(directive);
                }
            });
        }
        
        
        private static void ModifyPreprocessorDirectives(
            BuildTargetGroup targetGroup,
            Action<PreprocessorDirectivesCollection> callback)
        {
            if (callback == null)
            {
                return;
            }
            
            string currentDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            PreprocessorDirectivesCollection directivesCollection = new PreprocessorDirectivesCollection(currentDirectives);

            callback(directivesCollection);
            
            PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, directivesCollection.ToString());
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
