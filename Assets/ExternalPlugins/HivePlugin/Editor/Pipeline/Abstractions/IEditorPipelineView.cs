namespace Modules.Hive.Editor.Pipeline
{
    public interface IEditorPipelineView
    {
        string Title { get; }

        float Progress { get; set; }

        string Description { get; set; }

        bool IsCancelable { get; set; }

        bool IsCanceled { get; }

        bool IsVisible { get; set; }
    }
}
