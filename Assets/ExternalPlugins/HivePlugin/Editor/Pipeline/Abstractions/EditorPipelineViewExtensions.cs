namespace Modules.Hive.Editor.Pipeline
{
    public static class EditorPipelineViewExtensions
    {
        public static void Show(this IEditorPipelineView view)
        {
            view.IsCancelable = false;
            view.IsVisible = true;
        }


        public static void Show(this IEditorPipelineView view, string description)
        {
            view.Description = description;
            view.IsCancelable = false;
            view.IsVisible = true;
        }


        public static void Show(this IEditorPipelineView view, string description, float progress)
        {
            view.Description = description;
            view.Progress = progress;
            view.IsCancelable = false;
            view.IsVisible = true;
        }


        public static void ShowCancelable(this IEditorPipelineView view)
        {
            view.IsCancelable = true;
            view.IsVisible = true;
        }


        public static void ShowCancelable(this IEditorPipelineView view, string description)
        {
            view.Description = description;
            view.IsCancelable = true;
            view.IsVisible = true;
        }


        public static void ShowCancelable(this IEditorPipelineView view, string description, float progress)
        {
            view.Description = description;
            view.Progress = progress;
            view.IsCancelable = true;
            view.IsVisible = true;
        }


        public static void Hide(this IEditorPipelineView view)
        {
            view.IsVisible = false;
        }
    }
}
