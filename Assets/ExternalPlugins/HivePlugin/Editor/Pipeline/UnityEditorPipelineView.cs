using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Pipeline
{
    public class UnityEditorPipelineView : IEditorPipelineView
    {
        private string description = null;
        private float progress = 0f;
        private bool isVisible = false;
        private bool isCancelable = false;


        [JsonProperty]
        public string Title { get; }


        [JsonIgnore]
        public float Progress
        {
            get => progress;
            set
            {
                if (Mathf.Approximately(progress, value))
                {
                    return;
                }
                
                progress = value;
                UpdateView();
            }
        }


        [JsonIgnore]
        public string Description
        {
            get => description;
            set
            {
                if (value == description)
                {
                    return;
                }

                description = value;
                UpdateView();
            }
        }


        [JsonIgnore]
        public bool IsCancelable
        {
            get => isCancelable;
            set
            {
                if (value == isCancelable)
                {
                    return;
                }

                isCancelable = value;
                UpdateView();
            }
        }


        [JsonProperty]
        public bool IsCanceled { get; private set; }


        [JsonIgnore]
        public bool IsVisible
        {
            get => isVisible;
            set
            {
                if (value == isVisible)
                {
                    return;
                }

                isVisible = value;
                UpdateView();
            }
        }


        public UnityEditorPipelineView(string title)
        {
            Title = title;
        }


        private void UpdateView()
        {
            if (!isVisible)
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            if (IsCancelable)
            {
                EditorUtility.DisplayCancelableProgressBar(Title, Description, Progress);
            }
            else
            {
                EditorUtility.DisplayProgressBar(Title, Description, Progress);
            }
        }
    }
}
