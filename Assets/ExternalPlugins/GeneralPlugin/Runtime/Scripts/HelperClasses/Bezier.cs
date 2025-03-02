using System;
using UnityEngine;


namespace Modules.General.HelperClasses
{
    public static class Bezier
    {
        #region Public Methods
        
        public static Vector3 GetLinearPoint(Vector3 start, Vector3 end, float t)
        {
            return start + t * (end - start);
        }
        
        
        public static Vector3 GetQuadraticPoint(Vector3 start, Vector3 end, Vector3 pivot, float t)
        {
            float u = 1 - t;
            
            return (u * u * start) + (2 * u * t * pivot) + (t * t * end);
        }
        
        
        public static Vector3 GetCubicPoint(Vector3 start, Vector3 end, Vector3 pivot1, Vector3 pivot2, float t)
        {
            float u = 1 - t;
            
            return (u * u * u * start) + (3 * u * u * t * pivot1) + (3 * u * t * t * pivot2) + (t * t * t * end);
        }


        public static Vector3[] GetLinearCurve(Vector3 start, Vector3 end, int pointsCount)
        {
            float step = 1.0f / pointsCount;
            Vector3[] result = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                result[i] = GetLinearPoint(start, end, step * (i + 1));
            }

            return result;
        }


        public static Vector3[] GetQuadraticCurve(Vector3 start, Vector3 end, float curviness, int pointsCount)
        {
            // Fallback
            if (Math.Abs(curviness) < float.Epsilon)
            {
                return GetLinearCurve(start, end, pointsCount);
            }

            float pathLength = Vector3.Distance(start, end);
            float pivotShift = pathLength * 0.5f * curviness;
            Vector3 pivotCenter = start + (end - start) / 2;
            // Basically normalize the vector from center to the end, rotate it 90 degrees and multiply to the new length
            float ly = pivotCenter.y - end.y;
            float lx = pivotCenter.x - end.x;
            float length = Mathf.Sqrt(ly * ly + lx * lx);
            pivotCenter = new Vector3(pivotCenter.x - pivotShift * ly / length, pivotCenter.y + pivotShift * lx / length);

            return GetQuadraticCurve(start, end, pivotCenter, pointsCount);
        }


        public static Vector3[] GetQuadraticCurve(Vector3 start, Vector3 end, Vector3 pivot, int pointsCount)
        {
            // TODO Add fallback
            
            float step = 1.0f / pointsCount;
            Vector3[] result = new Vector3[pointsCount];
            for (int i = 0; i < pointsCount; i++)
            {
                result[i] = GetQuadraticPoint(start, end, pivot, step * (i + 1));
            }

            return result;
        }


        public static Vector3[] GetCubicCurve(Vector3 start, Vector3 end, Vector3 pivot1, Vector3 pivot2, int points)
        {
            // Fallback
            if (pivot1 == pivot2)
            {
                return GetQuadraticCurve(start, end, pivot1, points);
            }
            // TODO Add more fallbacks
            
            float step = 1.0f / points;
            Vector3[] result = new Vector3[points];
            for (int i = 0; i < points; i++)
            {
                result[i] = GetCubicPoint(start, end, pivot1, pivot2, step * (i + 1));
            }

            return result;
        }
        
        #endregion
    }
}