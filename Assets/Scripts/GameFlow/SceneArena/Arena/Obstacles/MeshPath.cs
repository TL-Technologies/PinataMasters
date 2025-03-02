using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshPath : MonoBehaviour
    {
        #region Variables

        class Point
        {
            public Vector3 position;
            public float time;
        }

        [SerializeField]
        private bool shouldUseGradient;
        [SerializeField][ConditionalHide("shouldUseGradient")]
        private Gradient color;

        [SerializeField]
        private float rotationAngle = 0f;
        [SerializeField]
        [Range(0.1f, 3)]
        private float lifetime = 0f;


        [SerializeField]
        private float size = 0f;

        private Mesh mesh;
        private List<Point> points;
        private float rotationOffsetY;
        private float rotationOffsetX;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            points = new List<Point>();
            mesh = new Mesh();
            mesh.MarkDynamic();
            GetComponent<MeshFilter>().mesh = mesh;

            rotationOffsetY = Mathf.Sin(Mathf.Deg2Rad * rotationAngle) * size;
            rotationOffsetX = Mathf.Cos(Mathf.Deg2Rad * rotationAngle) * size;
        }


        private void Update()
        {
            for (int i = 0; i < points.Count; i++)
            {
                points[i].time -= Time.deltaTime;

                if (points[i].time < 0f)
                {
                    points.RemoveAt(i);
                    i--;
                }
            }

            points.Insert(0, new Point { position = transform.position, time = lifetime });
            GenarateMesh();
        }

        #endregion



        #region Private methods

        private void GenarateMesh()
        {
            mesh.Clear();

            Vector3 offset = points[0].position;
            Vector3[] vertices = new Vector3[points.Count * 2];
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int i = 0; i < points.Count; i++)
            {
                vertices[i * 2] = points[i].position - offset;
                vertices[i * 2] = new Vector3(vertices[i * 2].x + rotationOffsetX, vertices[i * 2].y + rotationOffsetY, vertices[i * 2].z);

                vertices[i * 2 + 1] = points[i].position - offset;
                vertices[i * 2 + 1] = new Vector3(vertices[i * 2 + 1].x - rotationOffsetX, vertices[i * 2 + 1].y - rotationOffsetY, vertices[i * 2 + 1].z);

                uvs[i * 2] = new Vector2(0f, (float)i / (points.Count - 1));
                uvs[i * 2 + 1] = new Vector2(1f, (float)i / (points.Count - 1));
            }

            int[] triangles = new int[(points.Count - 1) * 6];
            for (int i = 0; i < points.Count - 1; i++)
            {
                bool isReversed = Vector3.Cross((vertices[i * 2] - vertices[i * 2 + 1]), vertices[i * 2 + 0] - vertices[i * 2 + 2]).z > 0f;

                if (isReversed)
                {
                    triangles[i * 6 + 0] = i * 2 + 0;
                    triangles[i * 6 + 1] = i * 2 + 2;
                    triangles[i * 6 + 2] = i * 2 + 1;

                    triangles[i * 6 + 3] = i * 2 + 2;
                    triangles[i * 6 + 4] = i * 2 + 3;
                    triangles[i * 6 + 5] = i * 2 + 1;
                }
                else
                {
                    triangles[i * 6 + 0] = i * 2 + 1;
                    triangles[i * 6 + 1] = i * 2 + 2;
                    triangles[i * 6 + 2] = i * 2 + 0;

                    triangles[i * 6 + 3] = i * 2 + 1;
                    triangles[i * 6 + 4] = i * 2 + 3;
                    triangles[i * 6 + 5] = i * 2 + 2;
                }
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            if (shouldUseGradient)
            {
                Color[] colors = new Color[vertices.Length];
                for (int i = 0; i < points.Count; i++)
                {
                    colors[i * 2] = Color32.Lerp(color.Evaluate(0), color.Evaluate(1), uvs[i * 2].y);
                    colors[i * 2 + 1] = Color32.Lerp(color.Evaluate(0), color.Evaluate(1), uvs[i * 2 + 1].y);
                }

                mesh.colors = colors;
            }
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(new Vector3(transform.position.x + size * Mathf.Cos(rotationAngle * Mathf.Deg2Rad), transform.position.y + size * Mathf.Sin(rotationAngle * Mathf.Deg2Rad), transform.position.z),
             new Vector3(transform.position.x - size * Mathf.Cos(rotationAngle * Mathf.Deg2Rad), transform.position.y - size * Mathf.Sin(rotationAngle * Mathf.Deg2Rad), transform.position.z));
        }

        #endregion
    }
}
