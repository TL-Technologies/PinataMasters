using UnityEngine;


namespace PinataMasters
{
    public class HermiteSpline : Spline
    {
        #region Fields

        Vector3[] m_0;
        Vector3[] m_1;

        #endregion



        #region Class lifecycle

        public HermiteSpline(Vector3[] _points) : base(_points)
        {
            m_0 = new Vector3[points.Length];
            m_1 = new Vector3[points.Length];

            for (int idx = 0; idx < points.Length - 1; idx++)
            {
                if (idx > 0)
                {
                    m_0[idx] = 0.5f * (points[idx + 1] - points[idx - 1]);
                }
                else
                {
                    m_0[idx] = points[idx + 1] - points[idx];
                }

                if (idx < points.Length - 2)
                {
                    m_1[idx] = 0.5f * (points[idx + 2] - points[idx]);
                }
                else
                {
                    m_1[idx] = (points[idx + 1] - points[idx]);
                }
            }
        }

        #endregion



        #region Public methods

        public override Vector3 GetSplinePoint(float _t)
        {
            Vector3 pathPoint = points[points.Length - 1];

            if (!Mathf.Approximately(_t, 1f))
            {
                float pointStep = 1f / (points.Length - 1);

                int segment = (int)Mathf.Floor(_t / pointStep);

                float t = (_t - pointStep * segment) / pointStep;
                float t_2 = Mathf.Pow(t, 2f);
                float t_3 = Mathf.Pow(t, 3f);

                Vector3 p0 = points[segment];
                Vector3 p1 = points[segment + 1];

                pathPoint = (2f * t_3 - 3.0f * t_2 + 1.0f) * p0 + (t_3 - 2.0f * t_2 + t) * m_0[segment] + (-2.0f * t_3 + 3.0f * t_2) * p1 + (t_3 - t_2) * m_1[segment];
            }

            return pathPoint;
        }

        #endregion
    }
}
