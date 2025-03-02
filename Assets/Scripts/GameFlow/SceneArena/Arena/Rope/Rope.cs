using System.Collections.Generic;
using UnityEngine;


namespace PinataMasters
{
    public class Rope : MonoBehaviour
    {
        #region Variables

        [SerializeField]
        private Rigidbody2D hook = null;
        [SerializeField]
        private RopeSegment ropeSegmentPrefab = null;
        [SerializeField]
        private int segmentsNumber = 10;


        private LineRenderer lineRenderer;
        private CurvedLineRenderer curvedLineRenderer;

        private Pinata pinata;
        private List<RopeSegment> ropeSegments = new List<RopeSegment>();

        private bool isFakeRopeGenerated;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            curvedLineRenderer = GetComponent<CurvedLineRenderer>();

            Pinata.OnPinataDestroy += DestroyRope;
        }


        private void OnDestroy()
        {
            Pinata.OnPinataDestroy -= DestroyRope;
            Pinata.OnPinataLeaveTween -= Leave;
        }


        private void FixedUpdate()
        {
            if (isFakeRopeGenerated)
            {
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, hook.transform.position);
                lineRenderer.SetPosition(1, pinata.transform.position);
            }
        }

        #endregion



        #region Public methods

        public void GenerateRope(Pinata target)
        {
            pinata = target;

            transform.localPosition = Vector3.zero;

            Rigidbody2D previousRB = hook;
            hook.GetComponent<HingeJoint2D>().connectedAnchor = hook.position;

            for (int i = 0; i < segmentsNumber; i++)
            {
                float segmentShiftY = (pinata.Place - hook.transform.position).magnitude / segmentsNumber;
                RopeSegment ropeSegment = Instantiate(ropeSegmentPrefab, pinata.transform.position, Quaternion.identity, transform);

                ropeSegments.Add(ropeSegment);

                Vector2 connectedPosition = new Vector2(0f, -segmentShiftY);
                ropeSegment.Init(previousRB, connectedPosition);

                if (i < segmentsNumber - 1)
                {
                    previousRB = ropeSegment.GetComponent<Rigidbody2D>();
                }
                else
                {
                    pinata.ConnectRopeEnd(ropeSegment.GetComponent<Rigidbody2D>());
                    ropeSegments.Add(ropeSegment);
                }
            }

            curvedLineRenderer.StartRendering(lineRenderer, ropeSegments);

            Pinata.OnPinataLeaveTween += Leave;
        }


        public void DestroyRope()
        {
            for (int i = 0; i < ropeSegments.Count; i++)
            {
                Destroy(ropeSegments[i].gameObject);
            }

            ropeSegments.Clear();

            lineRenderer.positionCount = 0;
            isFakeRopeGenerated = false;
        }

        #endregion



        #region Private methods

        private void Leave()
        {
            curvedLineRenderer.StopRendering();
            isFakeRopeGenerated = true;

            Pinata.OnPinataLeaveTween -= Leave;
        }

        #endregion
    }
}
