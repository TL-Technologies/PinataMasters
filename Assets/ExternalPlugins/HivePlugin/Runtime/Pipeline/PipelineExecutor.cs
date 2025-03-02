using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Hive.Pipeline
{
    public class PipelineExecutor : IPipelineExecutor<PipelineBase>
    {
        private Queue<PipelineBase> pipelines = new Queue<PipelineBase>();


        public bool IsRunning { get; private set; }


        #region Instancing

        private static PipelineExecutor instance;
        

        public static PipelineExecutor Instance => instance ?? (instance = new PipelineExecutor());

        #endregion



        #region Processing

        private void Execute()
        {
            if (IsRunning)
            {
                return;
            }

            if (pipelines.Count == 0)
            {
                return;
            }

            Debug.Log("Executor: Started.");
            IsRunning = true;
            ExecuteInternal();
        }


        private void ExecuteInternal()
        {
            if (pipelines.Count == 0)
            {
                Debug.Log("Executor: Finished.");
                IsRunning = false;
                return;
            }

            PipelineBase pipeline = pipelines.Dequeue();
            pipeline.ExecuteAsync().ContinueWith(
                task => ExecuteInternal(),
                TaskScheduler.FromCurrentSynchronizationContext());
        }

        #endregion


        public void Add(PipelineBase pipeline)
        {
            if (!pipeline.TrySetWaitingToRun())
            {
                throw new InvalidOperationException("The pipeline is already added to another pipeline executor");
            }

            pipelines.Enqueue(pipeline);
            Execute();
        }
    }
}
