using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    public class CurvedLineRenderer : MonoBehaviour
    {

        #region Variables

        [SerializeField]
        private int points = 100;

        private List<RopeSegment> linePoints = new List<RopeSegment>();
        private Vector3[] linePositions = new Vector3[0];

        private LineRenderer line;

        private int segmentsBetweenTwoPoints;
        private Vector3[] lineSegments;

        Coroutine renderingCoroutine;

        #endregion



        #region Public methods

        public void StartRendering(LineRenderer lineRenderer, List<RopeSegment> ropeSegments)
        {
            line = lineRenderer;
            linePoints = ropeSegments;
            linePositions = new Vector3[linePoints.Count];

            segmentsBetweenTwoPoints = Mathf.RoundToInt(points / (linePoints.Count - 1));
            lineSegments = new Vector3[segmentsBetweenTwoPoints * (linePositions.Length - 1) + 1];

            renderingCoroutine = StartCoroutine(Rendering());
        }


        public void StopRendering()
        {
            StopCoroutine(renderingCoroutine);
        }

        #endregion



        #region Private methods

        private IEnumerator Rendering()
        {
            while (linePoints.Count != 0)
            {
                yield return null;

                for (int i = 0; i < linePoints.Count; i++)
                {
                    linePositions[i] = linePoints[i].transform.position;
                }

                AnimationCurve curveX = new AnimationCurve();
                AnimationCurve curveY = new AnimationCurve();

                Keyframe[] keysX = new Keyframe[linePositions.Length];
                Keyframe[] keysY = new Keyframe[linePositions.Length];

                for (int i = 0; i < linePositions.Length; i++)
                {
                    keysX[i] = new Keyframe(i, linePositions[i].x);
                    keysY[i] = new Keyframe(i, linePositions[i].y);
                }

                curveX.keys = keysX;
                curveY.keys = keysY;

                for (int i = 0; i < linePositions.Length; i++)
                {
                    curveX.SmoothTangents(i, 0);
                    curveY.SmoothTangents(i, 0);
                }


                for (int i = 0; i < linePositions.Length; i++)
                {
                    lineSegments[i * segmentsBetweenTwoPoints] = linePositions[i];

                    if (i < linePositions.Length - 1)
                    {
                        for (int s = 1; s < segmentsBetweenTwoPoints; s++)
                        {
                            float time = (float)s / segmentsBetweenTwoPoints + i;
                            Vector2 newSegment = new Vector2(curveX.Evaluate(time), curveY.Evaluate(time));

                            lineSegments[i * segmentsBetweenTwoPoints + s] = newSegment;
                        }
                    }
                }

                line.positionCount = lineSegments.Length;
                line.SetPositions(lineSegments);
            }

            line.positionCount = 0;
        }

        #endregion
    }
}
