using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    internal class NullEditorPipelineView : IEditorPipelineView
    {
        private string description;
        
        
        public string Title => "Modules.Hive";

        public float Progress { get; set; }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                Debug.Log($"Hive build: {description}");
            }
        }

        public bool IsCancelable { get; set; }

        public bool IsCanceled => false;

        public bool IsVisible { get => false; set { } }
    }
}
