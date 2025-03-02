using Modules.Hive.Editor.Reflection;
using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Compilation;


namespace Modules.Hive.Editor.Pipeline
{
    public class SetPreprocessorDirectivesTask : EditorPipelineTask
    {
        private static readonly List<string> PredefinedDirectives = new List<string> 
            { "ODIN_INSPECTOR", "DOTWEEN_TMPRO", "DOTWEEN_TOOLKIT_2D", "CROSSPROMO_TOOLKIT_2D", "SPINE_TK2D" };

        [JsonProperty] private readonly string preprocessorDirectives;
        [JsonProperty] private readonly bool onlyForActiveTargetGroup;


        [JsonConstructor]
        public SetPreprocessorDirectivesTask(string preprocessorDirectives, bool onlyForActiveTargetGroup = true)
        {
            this.preprocessorDirectives = preprocessorDirectives;
            this.onlyForActiveTargetGroup = onlyForActiveTargetGroup;
        }


        public SetPreprocessorDirectivesTask(bool onlyForActiveTargetGroup = true)
        {
            preprocessorDirectives = string.Empty;
            this.onlyForActiveTargetGroup = onlyForActiveTargetGroup;
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IEditorPipelineContext context)
        {
            pipeline.View.Description = $"Configure preprocessor directives and rebuild scripts...";
            var set = new PreprocessorDirectivesCollection(preprocessorDirectives);

            // Combine directives if context provide it
            if (context is IPreprocessorDirectivesTaskContext taskContext)
            {
                set.UnionWith(taskContext.PreprocessorDirectives);
            }

            if (onlyForActiveTargetGroup)
            {
                BuildTargetGroup buildTargetGroup = BuildPipeline.GetBuildTargetGroup(EditorUserBuildSettings.activeBuildTarget);
                PreprocessorDirectivesCollection extendedSet = ExtendDirectives(set, buildTargetGroup);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, extendedSet.ToString());
            }
            else
            {
                UnityModulesManagerHelper.ForEachLoadedPlatformSupport(buildTargetGroup =>
                {
                    PreprocessorDirectivesCollection extendedSet = ExtendDirectives(set, buildTargetGroup);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, extendedSet.ToString());
                });
            }

            // Force to reset AppDomain (foo batchmode too).
            AssetDatabase.SaveAssets(); // It's highly important to perform this to avoid issues in batchmode!!
            AssetDatabase.Refresh();
            CompilationPipeline.RequestScriptCompilation();

            return SetStatus(PipelineTaskStatus.Succeeded);
            
            
            // Apply predefined directives
            PreprocessorDirectivesCollection ExtendDirectives(PreprocessorDirectivesCollection currentDirectives, BuildTargetGroup targetGroup)
            {
                string previousDirectives = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
                PreprocessorDirectivesCollection result = new PreprocessorDirectivesCollection(currentDirectives);
                
                foreach (string directive in PredefinedDirectives)
                {
                    if (previousDirectives.Contains(directive))
                    {
                        result.Add(directive);
                    }
                }
                
                return result;
            }
        }
    }
}
