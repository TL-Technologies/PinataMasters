using System.Collections.Generic;


namespace Modules.Hive.Editor.Pipeline
{
    internal class EditorPipelineOptionsComparer : IComparer<EditorPipelineOptionsAttribute>
    {
        public int Compare(EditorPipelineOptionsAttribute x, EditorPipelineOptionsAttribute y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x.AppHostLayer < y.AppHostLayer)
            {
                return -1;
            }

            if (x.AppHostLayer > y.AppHostLayer)
            {
                return 1;
            }

            if (x.Priority < y.Priority)
            {
                return -1;
            }

            if (x.Priority > y.Priority)
            {
                return 1;
            }

            return 0;
        }
    }
}
