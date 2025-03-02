namespace Modules.Hive.Pipeline
{
    public interface IPipelineExecutor<in T> where T : IPipeline
    {
        bool IsRunning { get; }

        void Add(T pipeline);
    }
}
