using UnityEngine;
using System;
using Spine;
using Spine.Unity;
using System.Collections.Generic;


namespace PinataMasters
{
    using HelperTypes;

    public class IngameOfferBirdAnimation : IngameOfferContentAnimation
    {
        #region Fields

        const string BlinkingAnimationName = "blinking";
        const float BlinkingInterval = 3f;

        static readonly string BoxFlyAnimation = "fly";

        static readonly Dictionary<IngameOfferAnimationDirection, string> BirdFlyAnimationsByDirection = new Dictionary<IngameOfferAnimationDirection, string>()
        {
            {IngameOfferAnimationDirection.Left, "fly_right"},
            {IngameOfferAnimationDirection.Right, "fly"},
        };

        static readonly Dictionary<IngameOfferAnimationDirection, string> BirdTurnAnimationsByDirection = new Dictionary<IngameOfferAnimationDirection, string>()
        {
            {IngameOfferAnimationDirection.Left, "turn_right"},
            {IngameOfferAnimationDirection.Right, "turn_left"},
        };

        static readonly Dictionary<IngameOfferAnimationDirection, string> UnboxingAnimationsByDirection = new Dictionary<IngameOfferAnimationDirection, string>()
        {
            {IngameOfferAnimationDirection.Left, "unboxing_right"},
            {IngameOfferAnimationDirection.Right, "unboxing"},
        };


        [SerializeField] SkeletonAnimation birdSkeletonAnimation;
        [SerializeField] SkeletonAnimation boxSkeletonAnimation;


        SimpleTimer blinkingTimer;
        Bone bodyBone;

        IngameOfferAnimationDirection animationDirection;
        Action animationCallback;

        #endregion



        #region Unity lifecycle

        void OnEnable()
        {
            animationDirection = IngameOfferAnimationDirection.None;

            ActivateBlinking(true);
        }


        void OnDisable()
        {
            ActivateBlinking(false);
        }


        void Update()
        {
            blinkingTimer?.CustomUpdate(Time.deltaTime);
        }

        #endregion



        #region Public methods

        public override void UpdateContentForDirection(IngameOfferAnimationDirection direction)
        {
            if (animationDirection != direction && !UnboxingAnimationsByDirection.ContainsValue(birdSkeletonAnimation.AnimationName))
            {
                if (animationDirection == IngameOfferAnimationDirection.None) 
                {
                    birdSkeletonAnimation.AnimationState.SetAnimation(0, BirdFlyAnimationsByDirection[direction], true);
                    boxSkeletonAnimation.AnimationState.SetAnimation(0, BoxFlyAnimation, true);
                }
                else
                {
                    birdSkeletonAnimation.AnimationState.SetAnimation(0, BirdTurnAnimationsByDirection[direction], false);
                    birdSkeletonAnimation.AnimationState.AddAnimation(0, BirdFlyAnimationsByDirection[direction], true, 0f);
                }

                animationDirection = direction;
            }
        }


        public void ApplyUnboxingAnimation(Action callback)
        {
            animationCallback = callback;

            birdSkeletonAnimation.AnimationState.Complete -= AnimationStateHandler;
            birdSkeletonAnimation.AnimationState.Complete += AnimationStateHandler;

            birdSkeletonAnimation.AnimationState.SetAnimation(0, UnboxingAnimationsByDirection[animationDirection], false);
            boxSkeletonAnimation.AnimationState.SetAnimation(0, UnboxingAnimationsByDirection[animationDirection], false);
        }

        #endregion



        #region Private methods

        void ActivateBlinking(bool activate)
        {
            if (activate)
            {
                if (birdSkeletonAnimation.AnimationState != null)
                {
                    birdSkeletonAnimation.AnimationState.SetAnimation(1, BlinkingAnimationName, false);
                }

                if (blinkingTimer == null)
                {
                    blinkingTimer = new SimpleTimer(BlinkingInterval, () =>
                    {
                        ActivateBlinking(true);
                    });
                }
                else
                {
                    blinkingTimer.ResetTimer(BlinkingInterval, () =>
                    {
                        ActivateBlinking(true);
                    });
                }
            }
            else
            {
                blinkingTimer = null;
            }
        }


        void ResetAnimations()
        {
            birdSkeletonAnimation.AnimationState.ClearTracks();
            boxSkeletonAnimation.AnimationState.ClearTracks();

            birdSkeletonAnimation.Skeleton.SetToSetupPose();
            boxSkeletonAnimation.Skeleton.SetToSetupPose();
        }


        void AnimationStateHandler(TrackEntry trackEntry)
        {
            if (UnboxingAnimationsByDirection.ContainsValue(trackEntry.Animation.Name))
            {          
                birdSkeletonAnimation.AnimationState.Complete -= AnimationStateHandler;

                ResetAnimations();

                animationCallback?.Invoke();
            }
        }

        #endregion
    }
}
