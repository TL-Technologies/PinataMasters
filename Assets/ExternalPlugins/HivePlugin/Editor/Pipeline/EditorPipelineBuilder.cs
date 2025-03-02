using System.Collections.Generic;


namespace Modules.Hive.Editor.Pipeline
{
    public static class EditorPipelineBuilder
    {
        public static EditorPipelineBuilder<IEditorPipelineContext> Create()
        {
            return new EditorPipelineBuilder<IEditorPipelineContext>(new NullEditorPipelineContext());
        }


        public static EditorPipelineBuilder<T> Create<T>() where T : IEditorPipelineContext, new()
        {
            return new EditorPipelineBuilder<T>(new T());
        }


        public static EditorPipelineBuilder<T> Create<T>(T context) where T : IEditorPipelineContext
        {
            return new EditorPipelineBuilder<T>(context);
        }
    }


    public class EditorPipelineBuilder<T> where T : IEditorPipelineContext
    {
        private T context;
        private Queue<IEditorPipelineTask<T>> tasks = new Queue<IEditorPipelineTask<T>>();
        private IEditorPipelineView view = new UnityEditorPipelineView("Modules.Hive");


        public EditorPipelineBuilder(T context)
        {
            this.context = context;
        }


        public EditorPipelineBuilder<T> SetView(IEditorPipelineView view)
        {
            this.view = view;
            return this;
        }


        public EditorPipelineBuilder<T> AddTask(IEditorPipelineTask<T> task)
        {
            tasks.Enqueue(task);
            return this;
        }


        public EditorPipeline<T> Build()
        {
            var pipeline = new EditorPipeline<T>(context, tasks)
            {
                View = view ?? new NullEditorPipelineView()
            };
            return pipeline;
        }


        public EditorPipeline<T> BuildAndSubmit()
        {
            var pipeline = Build();
            EditorPipelineExecutor.Instance.Add(pipeline);
            return pipeline;
        }
    }
}
