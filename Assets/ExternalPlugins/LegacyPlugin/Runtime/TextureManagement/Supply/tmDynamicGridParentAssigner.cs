using Modules.Legacy.TextureManagement.Batching;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Modules.Legacy.TextureManagement.Supply
{
// Here assumed world plane (x,y,z) to be (x,0,y)
    public class tmDynamicGridParentAssigner : MonoBehaviour
    {
        [SerializeField] Transform extentsMarker;
        [SerializeField] Transform gridRoot;

        [SerializeField] [Range(1, 10)] int xCols;
        [SerializeField] [Range(1, 10)] int yCols;


        #region Unity Lifecycle

        void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                return;
            }

            if (extentsMarker == null)
            {
                return;
            }

            Color prevGizmoColor = Gizmos.color;
            Gizmos.color = Color.red;

            Vector3 navigableExtents = extentsMarker.localScale / 2.0f;

            Vector3 lu = extentsMarker.position - Vector3.right * navigableExtents.x +
                         Vector3.forward * navigableExtents.z;
            Vector3 ru = extentsMarker.position + Vector3.right * navigableExtents.x +
                         Vector3.forward * navigableExtents.z;
            Vector3 ld = extentsMarker.position - Vector3.right * navigableExtents.x -
                         Vector3.forward * navigableExtents.z;
            Vector3 rd = extentsMarker.position + Vector3.right * navigableExtents.x -
                         Vector3.forward * navigableExtents.z;

            Gizmos.DrawLine(lu, ru);
            Gizmos.DrawLine(ru, rd);
            Gizmos.DrawLine(rd, ld);
            Gizmos.DrawLine(ld, lu);

            Gizmos.color = prevGizmoColor;
        }

        #endregion


        public void RebuildCells()
        {
            while (gridRoot.childCount != 0)
            {
                DestroyImmediate(gridRoot.GetChild(0).gameObject);
            }

            for (int i = 0; i < xCols * yCols; ++i)
            {
                Transform gridCell = new GameObject(string.Format("Batch_cell_{0}:{1}", i % xCols, i / yCols))
                    .transform;
                gridCell.parent = gridRoot;
                gridCell.localPosition = Vector3.zero;
                gridCell.localRotation = Quaternion.identity;
                gridCell.localScale = Vector3.one;
            }


            float extentsXLo = extentsMarker.position.x - extentsMarker.localScale.x / 2.0f;
            float extentsXHi = extentsMarker.position.x + extentsMarker.localScale.x / 2.0f;
            float extentsYLo = extentsMarker.position.z - extentsMarker.localScale.z / 2.0f;
            float extentsYHi = extentsMarker.position.z + extentsMarker.localScale.z / 2.0f;

            tmBatchObject[] batchObjects = transform.GetComponentsInChildren<tmBatchObject>();
            for (int i = 0; i < batchObjects.Length; ++i)
            {
                Vector3 chunkPosition = batchObjects[i].CachedTransform.position;
                int xIdx = Mathf.Clamp((int) ((chunkPosition.x - extentsXLo) / (extentsXHi - extentsXLo) * xCols), 0,
                    xCols - 1);
                int yIdx = Mathf.Clamp((int) ((chunkPosition.z - extentsYLo) / (extentsYHi - extentsYLo) * yCols), 0,
                    yCols - 1);

                batchObjects[i].Root = gridRoot.GetChild(yCols * yIdx + xIdx);
                batchObjects[i].BatchingType = tmBatchingType.Dynamic;
            }
        }
    }


    #if UNITY_EDITOR
    [CustomEditor(typeof(tmDynamicGridParentAssigner))]
    class tmDynamicGridParentAssignerInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                return;
            }

            base.OnInspectorGUI();

            if (GUILayout.Button("Rebuild cells"))
            {
                (target as tmDynamicGridParentAssigner).RebuildCells();
            }
        }
    }

    #endif
}
