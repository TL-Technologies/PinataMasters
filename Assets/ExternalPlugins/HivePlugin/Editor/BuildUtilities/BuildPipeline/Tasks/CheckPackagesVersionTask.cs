using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class CheckPackagesVersionTask : EditorPipelineTask
    {
        [Serializable]
        private struct PackageManifest
        {
            public Dictionary<string, string> dependencies;
        }


        public CheckPackagesVersionTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IEditorPipelineContext context)
        {
            SearchAndPrintPreviewPackages();

            return SetStatus(PipelineTaskStatus.Succeeded);
        }


        private void SearchAndPrintPreviewPackages()
        {
            string manifestPath = UnityPath.Combine(UnityPath.ProjectPath, "Packages", "manifest.json");

            if (File.Exists(manifestPath))
            {
                string json = File.ReadAllText(manifestPath);
                var manifest = JsonConvert.DeserializeObject<PackageManifest>(json);

                StringBuilder packageNames = new StringBuilder();

                int index = 0;
                foreach (var package in manifest.dependencies)
                {
                    if (Regex.IsMatch(package.Value, "preview"))
                    {
                        index++;
                        packageNames.Append($"\n<color=yellow>Package {index}: {package.Key} {package.Value}; </color>");
                    }
                }

                if (packageNames.Length > 0)
                {
                    Debug.LogWarning($"Hive build warning. The build includes '{index}' preview packages: {packageNames} ");
                }
            }
        }
    }
}
