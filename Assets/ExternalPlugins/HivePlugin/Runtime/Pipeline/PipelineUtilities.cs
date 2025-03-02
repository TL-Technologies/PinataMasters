using System;
using System.Threading.Tasks;
using UnityEngine;


namespace Modules.Hive.Pipeline
{
    internal static class PipelineUtilities
    {
        public static async Task<PipelineTaskStatus> ExecuteTaskAsync<TPipeline, TContext>(
            TPipeline pipeline, 
            IPipelineTask<TPipeline, TContext> task, 
            TContext context)
            where TPipeline : IPipeline
            where TContext : IPipelineContext
        {
            // Checks run case
            if (pipeline.HasFailedTasks && (task.RunCase & PipelineTaskRunCase.OnFailure) == 0)
            {
                return PipelineTaskStatus.None;
            }

            if (pipeline.IsCancellationRequested && (task.RunCase & PipelineTaskRunCase.OnCancel) == 0)
            {
                return PipelineTaskStatus.None;
            }

            // Execute task
            PipelineTaskStatus taskStatus;
            try
            {
//                Debug.LogError(task.GetType().ToString()+"  :::  "+task.ToString());
                taskStatus = await task.ExecuteAsync(pipeline, context);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                taskStatus = PipelineTaskStatus.Failed;
                pipeline.RegisterException(e);
            }

            return taskStatus;
        }
    }
}
