using System.Threading;


namespace Modules.General.Utilities
{
    public class UnityCancellationTokenSource : CancellationTokenSource
    {
        public UnityCancellationTokenSource()
        {
            #if UNITY_EDITOR
                // force to stop all tasks in editor mode
                UnityEditor.EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            #endif
        }


        #if UNITY_EDITOR
        private void OnPlayModeStateChanged(UnityEditor.PlayModeStateChange state)
        {
            if (state != UnityEditor.PlayModeStateChange.ExitingPlayMode)
            {
                return;
            }

            Cancel();
        }
        #endif
    }
}
