using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;


namespace PinataMasters
{
    public class IngameOfferFlyAnimation : IngameOfferAnimation
    {
        enum OfferFlyingType
        {
            Up             = 1,
            Down           = 2,
            Inner          = 3,
            Outer          = 4
        }


        #region Fields

        const float PathZPosition = -110.0f;
        const int FlyPathPointsCount = 12;

        public static event Action OnBirdLeaveFlyZone;

        [SerializeField] IngameOfferContentAnimation contentAnimation = null;

        Type animationType;

        IngameOfferFlyAnimationSettings cachedSettings;

        Action animationCallback;

        IngameOfferAnimationDirection flyDirection;
        SplineAnimation flyAnimation;

        OfferFlyingType offerFlyingType;

        Sequence flyingSequence;

        IngameOfferHandler offerHandler;

        bool shouldBirdLeaveFlyZone;

        #endregion



        #region Unity lifecycle

        void Update()
        {
            if (flyAnimation != null)
            {
                flyAnimation.CustomUpdate(Time.deltaTime);
            }
        }

        #endregion



        #region Public methods

        public override void Initialize(IngameOfferAnimationSettings settings, IngameOfferHandler offerHandler)
        {
            cachedSettings = settings as IngameOfferFlyAnimationSettings;
            this.offerHandler = offerHandler;
        }


        public override void ApplyAnimation(Type animationType, Action callback = null)
        {
            this.animationType = animationType; 
          
            if (callback != null)
            {
                animationCallback = callback;
            }

            switch (animationType)
            {
                case Type.Spawn:
                    ApplySpawnAnimation();
                    break;

                case Type.Idle:
                    ApplyIdleFlyAnimation();
                    break;

                case Type.Remove:
                    shouldBirdLeaveFlyZone = true;
                    break;
            }
        }

        #endregion



        #region Private methods

        void ApplySpawnAnimation()
        {
            if (flyingSequence != null)
            {
                flyingSequence.Kill();
            }

            shouldBirdLeaveFlyZone = false;

            flyDirection = (Random.value > 0.5f) ? (IngameOfferAnimationDirection.Left) : (IngameOfferAnimationDirection.Right);
            contentAnimation.UpdateContentForDirection(flyDirection);

            offerFlyingType = OfferFlyingType.Inner;

            float startPositionX = (flyDirection == IngameOfferAnimationDirection.Left) ? (cachedSettings.RightBorderPositionX) : (cachedSettings.LeftBorderPositionX);
            float finishPositionX = (flyDirection == IngameOfferAnimationDirection.Left) ? (cachedSettings.RightFlyingBorderPositionX) : (cachedSettings.LeftFlyingBorderPositionX);

            transform.position = new Vector3(startPositionX, cachedSettings.FlyingCenterY, PathZPosition);

            CalculateWayToPoint(finishPositionX, () =>
            {
                if (animationType != Type.Remove)
                {
                    ApplyAnimation(Type.Idle);
                }
                else
                {
                    ApplyRemoveAnimation();
                }
            });
        }


        void ApplyIdleFlyAnimation()
        {
            float finishPositionX = 0.0f;

            switch (flyDirection)
            {
                case IngameOfferAnimationDirection.Left:

                    if (offerFlyingType == OfferFlyingType.Inner)
                    {
                        finishPositionX = cachedSettings.LeftFlyingBorderPositionX;
                        offerFlyingType = OfferFlyingType.Up;
                    }
                    else
                    {
                        flyDirection = IngameOfferAnimationDirection.Right;
                        offerFlyingType = OfferFlyingType.Down;

                        finishPositionX = cachedSettings.RightFlyingBorderPositionX;
                    }

                    break;

                case IngameOfferAnimationDirection.Right:

                    if (offerFlyingType == OfferFlyingType.Inner)
                    {
                        finishPositionX = cachedSettings.RightFlyingBorderPositionX;
                        offerFlyingType = OfferFlyingType.Down;
                    }
                    else
                    {
                        flyDirection = IngameOfferAnimationDirection.Left;
                        offerFlyingType = OfferFlyingType.Up;

                        finishPositionX = cachedSettings.LeftFlyingBorderPositionX;
                    }

                    break;
            }

            contentAnimation.UpdateContentForDirection(flyDirection);

            CalculateWayToPoint(finishPositionX, () =>
            {
                if (offerHandler.Controller.IsOfferTouchesAvailable && shouldBirdLeaveFlyZone)
                {
                    ApplyRemoveAnimation();
                }
                else
                {
                    ApplyAnimation(Type.Idle);
                }
            });
        }
 

        void ApplyRemoveAnimation()
        {
            float finishPositionX = 0.0f;

            switch (flyDirection)
            {
                case IngameOfferAnimationDirection.Left:
                    finishPositionX = cachedSettings.LeftBorderPositionX;

                    break;

                case IngameOfferAnimationDirection.Right:
                    finishPositionX = cachedSettings.RightBorderPositionX;

                    break;
            }

            offerFlyingType = OfferFlyingType.Outer;
            OnBirdLeaveFlyZone?.Invoke();

            CalculateWayToPoint(finishPositionX, () =>
            {
                animationCallback?.Invoke();
            });
        }


        void CalculateWayToPoint(float finishPositionX, Action callback = null)
        {
            flyingSequence = DOTween.Sequence();
            AnimationCurve currentWayCurve = null;

            float startCurveTime = 0.0f;
            float finishCurveTime = 1.0f;

            switch (offerFlyingType)
            {
                case OfferFlyingType.Up:
                    currentWayCurve = cachedSettings.UpperWayCurve;

                    break;

                case OfferFlyingType.Down:
                    currentWayCurve = cachedSettings.DownWayCurve;

                    break;

                case OfferFlyingType.Inner:

                    if (flyDirection == IngameOfferAnimationDirection.Left)
                    {
                        currentWayCurve = cachedSettings.UpperWayCurve;

                        startCurveTime = currentWayCurve.keys[currentWayCurve.length - 1].time;
                        finishCurveTime = 1.0f;
                    }
                    else if (flyDirection == IngameOfferAnimationDirection.Right)
                    {
                        currentWayCurve = cachedSettings.DownWayCurve;

                        startCurveTime = currentWayCurve.keys[0].time;
                        finishCurveTime = 0.0f;
                    }

                    break;

                case OfferFlyingType.Outer:
                
                    if (flyDirection == IngameOfferAnimationDirection.Left)
                    {
                        currentWayCurve = cachedSettings.UpperWayCurve;

                        startCurveTime = 0.0f;
                        finishCurveTime = currentWayCurve.keys[0].time;
                    }
                    else if (flyDirection == IngameOfferAnimationDirection.Right)
                    {
                        currentWayCurve = cachedSettings.DownWayCurve;

                        startCurveTime = 1.0f;
                        finishCurveTime = currentWayCurve.keys[currentWayCurve.length - 1].time;
                    }

                    break;
            }

            float startPositionX = transform.position.x;
            float pathLength = Mathf.Abs(finishPositionX - startPositionX);
            float centerPositionY = (offerFlyingType == OfferFlyingType.Outer || offerFlyingType == OfferFlyingType.Inner) ? (transform.position.y) : (cachedSettings.FlyingCenterY);

            Vector3 previousPointPosition = transform.position;

            for (int i = 0; i < FlyPathPointsCount; i++)
            {
                float wayCurveCurrentTime = (float)i / FlyPathPointsCount * (finishCurveTime - startCurveTime) + startCurveTime;
                float stepX = pathLength / FlyPathPointsCount * i;

                if (flyDirection == IngameOfferAnimationDirection.Left)
                {
                    stepX *= -1.0f;
                }

                float x = startPositionX + stepX;
                float y = centerPositionY + cachedSettings.Amplitude * currentWayCurve.Evaluate(wayCurveCurrentTime);

                Vector3 pointToMovePosition = new Vector3(x, y, PathZPosition);
                flyingSequence.Append(transform.DOMove(pointToMovePosition, (pointToMovePosition - previousPointPosition).magnitude / cachedSettings.MoveSpeed).SetEase(Ease.Linear));

                previousPointPosition = pointToMovePosition;
            }

            flyingSequence.OnComplete(() =>
            {
                callback?.Invoke();
            });
        }

        #endregion
    }
}
