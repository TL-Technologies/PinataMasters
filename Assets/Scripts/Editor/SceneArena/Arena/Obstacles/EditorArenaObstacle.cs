using UnityEditor;


namespace PinataMasters
{
    [CanEditMultipleObjects][CustomEditor(typeof(ArenaObstacle))]
    public class EditorArenaObstacle : Editor
    {
        #region Variables

        private readonly string[] movableProperties = { "moveDuration", "end", "canStop", "stopDuration", "isSlowerSpeedShooter" };
        private readonly string[] stopableObstacle = { "stopDuration" };

        private SerializedProperty canMove;
        private SerializedProperty canStop;

        #endregion



        #region Unity lifecycle

        private void OnEnable()
        {
            canMove = serializedObject.FindProperty("canMove");
            canStop = serializedObject.FindProperty("canStop");
        }


        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (canMove.boolValue && canStop.boolValue)
            {
                DrawDefaultInspector();
            }
            else if (canMove.boolValue && !canStop.boolValue)
            {
                DrawPropertiesExcluding(serializedObject, stopableObstacle);
            }
            else if (!canMove.boolValue)
            {
                DrawPropertiesExcluding(serializedObject, movableProperties);
            }

            serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}
