using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.Pipeline
{
    public abstract class EditorPipelineTask<T> : IEditorPipelineTask<T> where T : IEditorPipelineContext
    {
        [JsonProperty]
        public PipelineTaskRunCase RunCase { get; protected set; } = PipelineTaskRunCase.OnSuccess;


        [JsonProperty]
        public PipelineTaskStatus Status { get; protected set; } = PipelineTaskStatus.None;


        [JsonProperty]
        public Exception Exception { get; protected set; }


        public virtual void Prepare(IEditorPipeline pipeline) { }
        public abstract Task<PipelineTaskStatus> ExecuteAsync(IEditorPipeline pipeline, T context);


        protected Task<PipelineTaskStatus> SetStatus(PipelineTaskStatus status)
        {
            Status = status;
            return Task.FromResult(status);
        }
    }


    public abstract class EditorPipelineTask : EditorPipelineTask<IEditorPipelineContext> { }
}
