using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu(fileName = "offer_animation", menuName = "PinataMasters/Offers/Animations/IngameOfferFlyAnimation")]
    public class IngameOfferFlyAnimationSettings : IngameOfferAnimationSettings
    {
        #region Fields

        [SerializeField] float moveSpeed = 0.0f;
        [SerializeField] float amplitude = 0.0f;
        [SerializeField] AnimationCurve upperWayCurve = null;
        [SerializeField] AnimationCurve downWayCurve = null;
        [SerializeField] float flyingCenterY = 0.0f;

        [Header("Border Settings")]
        [SerializeField] float leftFlyingBorderPositionX = 0.0f;
        [SerializeField] float rightFlyingBorderPositionX = 0.0f;
        [Space]
        [SerializeField] float leftBorderPositionX = 0.0f;
        [SerializeField] float rightBorderPositionX = 0.0f;

        #endregion



        #region Properties

        public float MoveSpeed => moveSpeed;

        public float Amplitude => amplitude;

        public AnimationCurve UpperWayCurve => upperWayCurve;

        public AnimationCurve DownWayCurve => downWayCurve;

        public float FlyingCenterY => flyingCenterY;

        public float LeftFlyingBorderPositionX => leftFlyingBorderPositionX;

        public float RightFlyingBorderPositionX => rightFlyingBorderPositionX;

        public float LeftBorderPositionX => leftBorderPositionX;

        public float RightBorderPositionX => rightBorderPositionX;

        #endregion
    }
}
