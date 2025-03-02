using Modules.Hive.Pipeline;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;


namespace Modules.Hive.Editor.Pipeline
{
    public abstract class EditorPipeline : IEditorPipeline
    {
        [JsonProperty] protected List<Exception> exceptions = new List<Exception>();


        [JsonProperty]
        public PipelineStatus Status { get; private set; }


        [JsonIgnore]
        public bool IsCompleted => 
            Status == PipelineStatus.Succeeded ||
            Status == PipelineStatus.Failed ||
            Status == PipelineStatus.Canceled;


        [JsonProperty]
        public bool IsCancellationRequested { get; private set; }


        [JsonProperty]
        public bool HasFailedTasks { get; protected set; }


        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public IEditorPipelineView View { get; internal set; }


        [JsonIgnore]
        public AggregateException Exceptions => new AggregateException(exceptions);


        [JsonIgnore]
        protected internal abstract float Progress { get; }


        protected internal abstract Task<bool> ExecuteNextTask();


        public EditorPipeline()
        {
            Status = PipelineStatus.None;
        }


        public void Cancel()
        {
            IsCancellationRequested = true;
        }
        
        
        public void RegisterException(Exception exception)
        {
            exceptions.Add(exception);
        }
        
        
        internal bool TrySetWaitingToRun()
        {
            if (Status == PipelineStatus.None)
            {
                Status = PipelineStatus.WaitingToRun;
                return true;
            }

            return false;
        }


        protected internal virtual void BeginExecution()
        {
            if (Status != PipelineStatus.None && Status != PipelineStatus.WaitingToRun)
            {
                throw new InvalidOperationException("The pipeline is already awaiting execution.");
            }

            Status = PipelineStatus.Running;
        }


        protected internal PipelineStatus EndExecution()
        {
            if (IsCancellationRequested)
            {
                Status = PipelineStatus.Canceled;
            }
            else if (HasFailedTasks)
            {
                Status = PipelineStatus.Failed;
            }
            else
            {
                Status = PipelineStatus.Succeeded;
            }

            return Status;
        }
    }


    public class EditorPipeline<T> : EditorPipeline, IEditorPipeline<T> where T : IEditorPipelineContext
    {
        [JsonProperty] private int tasksCount = 0;
        [JsonProperty] private int currentTaskIndex = -1;


        [JsonProperty(ItemTypeNameHandling = TypeNameHandling.Objects)]
        private Queue<IEditorPipelineTask<T>> tasksQueue;


        [JsonProperty(TypeNameHandling = TypeNameHandling.Objects)]
        public T Context { get; }


        public EditorPipeline(T context, IEnumerable<IEditorPipelineTask<T>> tasks) : base()
        {
            Context = context;
            tasksQueue = new Queue<IEditorPipelineTask<T>>(tasks);
            tasksCount = tasksQueue.Count;
        }


        [JsonConstructor]
        internal EditorPipeline(T context, Queue<IEditorPipelineTask<T>> tasks) : base()
        {
            Context = context;
            tasksQueue = tasks;
            tasksCount = tasks != null ? tasks.Count : 0;
        }


        protected internal override void BeginExecution()
        {
            base.BeginExecution();
            
            // Warm up tasks
            foreach (IEditorPipelineTask<T> task in tasksQueue)
            {
                task.Prepare(this);
            }
        }


        protected internal override async Task<bool> ExecuteNextTask()
        {
            if (tasksQueue.Count == 0)
            {
                return false;
            }

            currentTaskIndex++;
            var task = tasksQueue.Dequeue();

            // Execute task
            PipelineTaskStatus taskStatus = await PipelineUtilities.ExecuteTaskAsync(this, task, Context);

            // Check result
            if (taskStatus == PipelineTaskStatus.Failed)
            {
                UnityEngine.Debug.LogError(task);
                HasFailedTasks = true;
            }

            return true;
        }


        protected internal override float Progress
            => tasksCount == 0 ? 0 : (float)(currentTaskIndex + 1) / tasksCount;
    }
}
