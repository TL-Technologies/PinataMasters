using Modules.Hive.Editor.Pipeline;
using Modules.Hive.Editor.Reflection;
using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class LogExceptionsTask : EditorPipelineTask
    {
        [JsonProperty] private int errorsBeforeBuild = 0;
        

        public LogExceptionsTask()
        {
            RunCase = PipelineTaskRunCase.Always;
        }


        public override void Prepare(IEditorPipeline pipeline)
        {
            base.Prepare(pipeline);
            
            ConsoleFlag currentConsoleFlag = ConsoleWindowHelper.GetFlags();
            ConsoleWindowHelper.SetFlags(ConsoleFlag.LogLevelError);
            errorsBeforeBuild = ConsoleWindowHelper.EnumerateLogEntries().Count();
            ConsoleWindowHelper.SetFlags(currentConsoleFlag);
        }


        public override Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, IEditorPipelineContext context)
        {
            if (pipeline.HasFailedTasks)
            {
                ConsoleFlag currentConsoleFlag = ConsoleWindowHelper.GetFlags();
                // Aggregate problems
                StringBuilder stringBuilder = new StringBuilder("Hive build problems\n\n");
                ConsoleWindowHelper.SetFlags(ConsoleFlag.LogLevelError);
                
                int index = 0;
                foreach (object entry in ConsoleWindowHelper.EnumerateLogEntries())
                {
                    // Skip errors that existed before build start
                    if (errorsBeforeBuild > 0)
                    {
                        errorsBeforeBuild--;
                        continue;
                    }
                    
                    stringBuilder.Append($"Problem {index}: {ConsoleWindowHelper.GetLogEntryMode(entry)}\n");
                    stringBuilder.Append($"{ConsoleWindowHelper.GetLogEntryMessage(entry)}\n");
                    index++;
                }
                ConsoleWindowHelper.SetFlags(currentConsoleFlag);
                
                // Log problems
                StackTraceLogType stackTraceLog = Application.GetStackTraceLogType(LogType.Error);
                Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
                Debug.LogError("==================================================");
                Debug.LogError(stringBuilder.ToString());
                Debug.LogError("==================================================");
                Application.SetStackTraceLogType(LogType.Error, stackTraceLog);
            }
            
            return SetStatus(PipelineTaskStatus.Succeeded);
        }
    }
}