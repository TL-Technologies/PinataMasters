using UnityEngine;


namespace PinataMasters
{
    public class RopeSegment : MonoBehaviour
    {

        [SerializeField]
        private HingeJoint2D joint = null;


        public void Init(Rigidbody2D previousRB, Vector2 connectedPosition)
        {
            joint.connectedBody = previousRB;
            joint.connectedAnchor = connectedPosition;
        }
    }
}
