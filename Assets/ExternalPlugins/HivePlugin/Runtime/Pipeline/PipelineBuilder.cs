using System.Collections.Generic;


namespace Modules.Hive.Pipeline
{
    public static class PipelineBuilder
    {
        public static PipelineBuilder<IPipelineContext> Create()
        {
            return new PipelineBuilder<IPipelineContext>(new NullPipelineContext());
        }


        public static PipelineBuilder<T> Create<T>() where T : IPipelineContext, new()
        {
            return new PipelineBuilder<T>(new T());
        }


        public static PipelineBuilder<T> Create<T>(T context) where T : IPipelineContext
        {
            return new PipelineBuilder<T>(context);
        }
    }


    public class PipelineBuilder<T> where T : IPipelineContext
    {
        private Queue<IPipelineTask<T>> tasks = new Queue<IPipelineTask<T>>();


        public T Context { get; }


        public PipelineExecutor Executor { get; private set; }


        public PipelineBuilder(T context)
        {
            Context = context;
        }


        public PipelineBuilder<T> SetExecutor(PipelineExecutor executor)
        {
            Executor = executor;
            return this;
        }


        public PipelineBuilder<T> AddTask(IPipelineTask<T> task)
        {
            tasks.Enqueue(task);
            return this;
        }


        public Pipeline<T> Build()
        {
            return new Pipeline<T>(Context, tasks);
        }


        public Pipeline<T> BuildAndSubmit()
        {
            PipelineExecutor executor = Executor ?? PipelineExecutor.Instance;
            var pipeline = new Pipeline<T>(Context, tasks);
            executor.Add(pipeline);
            return pipeline;
        }
    }
}
