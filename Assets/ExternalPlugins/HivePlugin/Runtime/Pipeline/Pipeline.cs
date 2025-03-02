using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Hive.Pipeline
{
    public abstract class PipelineBase : IExecutablePipeline
    {
        protected readonly List<Exception> exceptions = new List<Exception>();


        public PipelineStatus Status { get; protected set; }


        public bool IsCompleted =>
            Status == PipelineStatus.Succeeded ||
            Status == PipelineStatus.Failed ||
            Status == PipelineStatus.Canceled;


        public bool IsCancellationRequested { get; private set; }


        public bool HasFailedTasks { get; protected set; }


        public AggregateException Exceptions => new AggregateException(exceptions);


        public void RegisterException(Exception exception)
        {
            exceptions.Add(exception);
        }


        public void Cancel() => IsCancellationRequested = true;


        public abstract Task<PipelineStatus> ExecuteAsync();


        internal bool TrySetWaitingToRun()
        {
            if (Status == PipelineStatus.None)
            {
                Status = PipelineStatus.WaitingToRun;
                return true;
            }

            return false;
        }
    }


    public class Pipeline<T> : PipelineBase, IPipeline<T> where T : IPipelineContext
    {
        private Queue<IPipelineTask<T>> tasksQueue;


        public T Context { get; }


        public Pipeline(T context, IEnumerable<IPipelineTask<T>> tasks)
        {
            Status = PipelineStatus.None;
            Context = context;
            tasksQueue = new Queue<IPipelineTask<T>>(tasks);
        }


        internal Pipeline(T context, Queue<IPipelineTask<T>> tasks)
        {

            Status = PipelineStatus.None;
            Context = context;
            tasksQueue = tasks;
        }


        public override async Task<PipelineStatus> ExecuteAsync()
        {
            // Start pipeline
            if (Status != PipelineStatus.None && Status != PipelineStatus.WaitingToRun)
            {
                throw new InvalidOperationException("The pipeline is already awaiting execution.");
            }

            Status = PipelineStatus.Running;

            // Warm up tasks
            foreach (IPipelineTask<T> task in tasksQueue)
            {
                task.Prepare(this);
            }
            
            // Pipeline cycle
            while (tasksQueue.Count > 0)
            { 
                var task = tasksQueue.Dequeue();

                // Execute task
                PipelineTaskStatus taskStatus = await PipelineUtilities.ExecuteTaskAsync(this, task, Context);

                // Check result
                if (taskStatus == PipelineTaskStatus.Failed)
                {
                    HasFailedTasks = true;
                }
            }

            // End of pipeline
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
}
