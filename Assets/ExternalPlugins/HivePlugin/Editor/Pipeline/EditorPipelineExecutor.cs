using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    public class EditorPipelineExecutor : IPipelineExecutor<EditorPipeline>
    {
        private const string DefaultTitle = "Modules.Hive";
        private const int ColdStartFramesCount = 2;

        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Objects)]
        private Queue<EditorPipeline> pipelines = new Queue<EditorPipeline>();

        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        private EditorPipeline currentPipeline;

        private Task<bool> currentTask;
        private bool waitForCompilingScripts;
        private int skipFrames = ColdStartFramesCount;


        #region Instancing

        private static EditorPipelineExecutor instance;

        public static EditorPipelineExecutor Instance => instance ?? (instance = Deserialize());

        #endregion



        #region Serialization

        private static readonly string PathToFile = UnityPath.Combine(UnityPath.ProjectTempPath, "EditorPipelineExecutor.json");


        private static JsonSerializer GetSerializer()
        {
            var serializer = JsonSerializer.CreateDefault();
            serializer.Formatting = Formatting.Indented;
            serializer.Converters.Add(new DictionarySerializationConverter<BuildSetting, string>());
            return serializer;
        }


        private static EditorPipelineExecutor Deserialize()
        {
            if (!File.Exists(PathToFile))
            {
                return new EditorPipelineExecutor();
            }

            EditorPipelineExecutor pipelineProcessor;
            try
            {
                pipelineProcessor = DeserializeFromFile();
            }
            catch (Exception e)
            {
                pipelineProcessor = new EditorPipelineExecutor();
                pipelineProcessor.StopProcessing();

                Debug.LogException(e);
                EditorUtility.DisplayDialog(DefaultTitle, "An exception occurred while executing a pipeline.\nSee log for details.", "Ok");
            }

            return pipelineProcessor;
        }


        private static EditorPipelineExecutor DeserializeFromFile()
        {
            using (var stream = File.OpenText(PathToFile))
            using (var reader = new JsonTextReader(stream))
            {
                return GetSerializer().Deserialize<EditorPipelineExecutor>(reader);
            }
        }


        private void Serialize()
        {
            using (var stream = File.CreateText(PathToFile))
            using (var writer = new JsonTextWriter(stream))
            {
                GetSerializer().Serialize(writer, this);
            }
        }


        private void RemoveSerializedState()
        {
            if (!File.Exists(PathToFile))
            {
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append(Path.Combine(Path.GetDirectoryName(PathToFile), Path.GetFileNameWithoutExtension(PathToFile)));
            sb.Append("_backup");
            sb.Append(Path.GetExtension(PathToFile));
            string backupFileName = sb.ToString();

            File.Delete(backupFileName);
            File.Move(PathToFile, backupFileName);
        }

        #endregion



        #region Unity events and triggers

        [DidReloadScripts]
        private static void ExecuteAfterReloadScripts()
        {
            Instance.Execute();
        }


        private void OnBeforeAssemblyReload()
        {
            Serialize();
        }

        #endregion



        #region Processing

        private void Execute()
        {
            if (IsRunning)
            {
                return;
            }

            if (currentTask == null && currentPipeline == null && pipelines.Count == 0)
            {
                return;
            }

            StartProcessing();
            Update();
        }


        private void StartProcessing()
        {
            IsRunning = true;

            if (EditorApplication.isCompiling)
            {
                ShowCompilingScriptsAwaiter();
            }

            EditorApplication.update += Update;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeAssemblyReload;
        }


        private void StopProcessing()
        {
            IsRunning = false;
            // EditorUtility.ClearProgressBar();
            EditorApplication.update -= Update;
            AssemblyReloadEvents.beforeAssemblyReload -= OnBeforeAssemblyReload;
            RemoveSerializedState();
        }


        private void Update()
        {
            // 1. Do nothing while project is compiling
            if (waitForCompilingScripts)
            {
                if (EditorApplication.isCompiling)
                {
                    return;
                }

                StopCompilingScriptsAwaiter();
            }

            if (currentTask == null && EditorApplication.isCompiling)
            {
                ShowCompilingScriptsAwaiter();
                return;
            }

            // 2. Skip several frames (cold start)
            if (skipFrames > 0)
            {
                skipFrames--;
                return;
            }

            // 3. Waiting for completion of the current task
            if (currentTask != null)
            {
                if (currentTask.IsCompleted)
                {
                    // Is it the last task in pipeline?
                    if (!currentTask.Result)
                    {
                        currentPipeline.View.Hide();
                        currentPipeline.EndExecution();
                        currentPipeline = null;
                    }

                    // Finalize current task
                    currentTask = null;
                    EditorApplication.UnlockReloadAssemblies();

                    // Check whether Unity wants to recompile scripts
                    if (EditorApplication.isCompiling)
                    {
                        ShowCompilingScriptsAwaiter();
                    }
                }

                return;
            }

            // 4. Dequeue next pipeline if needed
            if (currentPipeline == null)
            {
                // Stop processing if pipelines queue is empty
                if (pipelines.Count == 0)
                {
                    StopProcessing();
                    return;
                }

                // Try to start next pipeline
                currentPipeline = pipelines.Dequeue();
                try
                {
                    currentPipeline.BeginExecution();
                }
                catch (Exception e)
                {
                    // Skip pipeline if it fails
                    currentPipeline = null;
                    Debug.LogException(e);
                    return;
                }
            }

            // 5. Start next task in current pipeline
            currentPipeline.View.ShowCancelable("Processing a pipeline...", currentPipeline.Progress);
            if (currentPipeline.View.IsCanceled && !currentPipeline.IsCancellationRequested)
            {
                currentPipeline.Cancel();
            }

            EditorApplication.LockReloadAssemblies();
            currentTask = currentPipeline.ExecuteNextTask();
        }


        private void ShowCompilingScriptsAwaiter()
        {
            skipFrames = ColdStartFramesCount;
            waitForCompilingScripts = true;
            currentPipeline?.View.Show("Compiling scripts...", 0);
        }


        private void StopCompilingScriptsAwaiter()
        {
            waitForCompilingScripts = false;
        }

        #endregion



        #region IPipelineExecutor implementation

        [JsonIgnore]
        public bool IsRunning { get; private set; }


        public void Add(EditorPipeline pipeline)
        {
            if (!pipeline.TrySetWaitingToRun())
            {
                throw new InvalidOperationException("The pipeline is already added to another pipeline executor");
            }

            pipelines.Enqueue(pipeline);
            Execute();
        }

        #endregion



        #region Debug tools

        [MenuItem("Modules/Hive/Debug/Lock Reload Assemblies")]
        public static void LockReloadAssemblies()
        {
            EditorApplication.LockReloadAssemblies();
        }


        [MenuItem("Modules/Hive/Debug/Unlock Reload Assemblies")]
        public static void UnlockReloadAssemblies()
        {
            EditorApplication.UnlockReloadAssemblies();
            EditorUtility.ClearProgressBar();
        }

        #endregion
    }
}
