using UnityEngine;


namespace PinataMasters
{
    public abstract class Spline
    {
        #region Fields

        protected Vector3[] points;

        #endregion



        #region Class lifecycle

        public Spline(Vector3[] _points)
        {
            points = _points;
        }

        #endregion



        #region Public methods

        public abstract Vector3 GetSplinePoint(float _t);

        #endregion
    }
}
