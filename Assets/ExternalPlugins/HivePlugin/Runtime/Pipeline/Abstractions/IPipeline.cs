using System;
using System.Threading.Tasks;


namespace Modules.Hive.Pipeline
{
    public interface IPipeline
    {
        PipelineStatus Status { get; }

        /// <summary>
        /// Gets a value that indicates whether the pipeline has completed
        /// (that is, the pipeline is in one of the three final states: Succeeded, Failed, or Canceled).
        /// </summary>
        bool IsCompleted { get; }

        bool IsCancellationRequested { get; }

        bool HasFailedTasks { get; }

        void RegisterException(Exception exception);
        void Cancel();
    }


    public interface IPipeline<out T> : IPipeline where T : IPipelineContext
    {
        T Context { get; }
    }


    public interface IExecutablePipeline : IPipeline
    {
        Task<PipelineStatus> ExecuteAsync();
    }
}
