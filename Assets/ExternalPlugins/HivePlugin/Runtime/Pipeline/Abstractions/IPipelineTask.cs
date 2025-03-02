using System.Threading.Tasks;


namespace Modules.Hive.Pipeline
{
    public interface IPipelineTask<in TPipeline, in TContext> 
        where TPipeline : IPipeline
        where TContext : IPipelineContext
    {
        PipelineTaskRunCase RunCase { get; }
        PipelineTaskStatus Status { get; }

        void Prepare(TPipeline pipeline);
        Task<PipelineTaskStatus> ExecuteAsync(TPipeline pipeline, TContext context);
    }


    public interface IPipelineTask<in TContext> : IPipelineTask<IPipeline, TContext>
        where TContext : IPipelineContext { }
}
