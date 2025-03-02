using Spine;
using Spine.Unity;
using UnityEngine;
using System;
using System.Collections;


namespace PinataMasters
{
    public class PinataEmotions : MonoBehaviour
    {
        #region Types

        [Serializable]
        private class Effect
        {
            public string Name = null;
            public Transform Transform = null;
            public ParticleEffect ParticleEffect = null;
        }

        #endregion



        #region Variables

        private const int ANIMATION_INDEX = 0;

        [Header("BodyParts")]
        [SerializeField]
        private GameObject pinataBody = null;
        [SerializeField]
        private GameObject pinataHead = null;

        [Header("Animation")]
        [SerializeField]
        private SkeletonAnimation pinataSkeleton = null;
        [SerializeField]
        [SpineAnimation]
        private string IDLE = null;
        [SerializeField]
        [SpineAnimation]
        private string APPEAR = null;
        [SerializeField]
        [SpineAnimation]
        private string FAIL = null;
        [SerializeField]
        [SpineAnimation]
        private string ESCAPE = null;
        [SerializeField]
        [SpineAnimation]
        private string HIT_EMOTION_START = null;
        [SerializeField]
        [SpineAnimation]
        private string HIT_EMOTION_FINISH = null;
        [SerializeField]
        private bool isOnlyHeadAnimated = false;
        [SerializeField]
        private bool needIdleAnimation = false;
        [SerializeField]
        private float idleAnimationDelay = 2f;

        [Header("Effects")]
        [SerializeField]
        private Effect[] effects = null;


        private TrackEntry tracker;
        private Action OnStartPinataLeave = delegate { };

        private bool isHitEmotionAllow;

        private Coroutine idleCorutine;
        private Coroutine disableBodyCorutine;

        #endregion



        #region Unity lifecycle

        private void Awake()
        {
            Pinata.OnCollision += OnCollision;
        }

        private void OnDestroy()
        {
            Pinata.OnCollision -= OnCollision;
        }

        #endregion



        #region Public methods

        public virtual void PlayFailAnimation(Action OnFinish)
        {
            OnStartPinataLeave = OnFinish;

            if (idleCorutine != null)
            {
                StopCoroutine(idleCorutine);
                idleCorutine = null;
            }

            if (pinataSkeleton != null)
            {
                tracker.Complete -= DisableHeadAnimation;
                tracker.Complete -= InitFinishHitEmotion;

                pinataSkeleton.gameObject.SetActive(true);
                pinataBody.SetActive(false);

                tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, FAIL, false);
                tracker.Event += OnEvent;

                StartCoroutine(LeavePinataWithAnim());
            }
            else
            {
                OnStartPinataLeave();
            }
        }


        public void PlayAppearAnimation()
        {
            if (pinataSkeleton != null)
            {
                pinataSkeleton.gameObject.SetActive(true);
                pinataBody.SetActive(false);

                tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, APPEAR, true);
            }
        }


        public void PlayDestroyAnimation()
        {
            if (isOnlyHeadAnimated && pinataSkeleton != null)
            {
                pinataSkeleton.gameObject.SetActive(true);
                pinataBody.SetActive(false);

                tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, HIT_EMOTION_START, true);
            }
        }

        #endregion



        #region Private methods

        private void OnEvent(TrackEntry trackEntry, Spine.Event e)
        {
            if (effects == null || effects.Length == 0)
            {
                return;
            }

            foreach (Effect effect in effects)
            {
                if (effect.Name == e.Data.Name)
                {
                    EffectPlayer.Play(effect.ParticleEffect, effect.Transform.position, effect.Transform);
                }
            }
        }


        private void OnCollision()
        {
            if (tracker != null && tracker.Animation != null && (tracker.Animation.Name == APPEAR || tracker.Animation.Name == IDLE))
            {
                DisableBodyAnimation(tracker);
                isHitEmotionAllow = true;
            }

            if (isHitEmotionAllow)
            {
                isHitEmotionAllow = false;

                pinataHead.SetActive(false);

                tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, HIT_EMOTION_START, false);

                tracker.Complete += InitFinishHitEmotion;

                pinataSkeleton.gameObject.SetActive(true);
            }

            if (needIdleAnimation)
            {
                if (idleCorutine != null)
                {
                    StopCoroutine(idleCorutine);
                    idleCorutine = null;
                }

                idleCorutine = StartCoroutine(IdleAnimationCountdown());
            }
        }


        private void InitFinishHitEmotion(TrackEntry pinataTracker)
        {
            tracker.Complete -= InitFinishHitEmotion;

            if (string.IsNullOrEmpty(HIT_EMOTION_FINISH))
            {
                if (GetComponent<MileyEmotions>() != null)
                {
                    PlayIdleAnimation(pinataTracker);
                }

                return;
            }

            tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, HIT_EMOTION_FINISH, false);
            tracker.Complete += DisableHeadAnimation;

        }

        private void PlayIdleAnimation(TrackEntry pinataTracker)
        {
            if (string.IsNullOrEmpty(IDLE))
            {
                DisableHeadAnimation(pinataTracker);
                return;
            }

            if (!isOnlyHeadAnimated)
            {
                DisableHeadAnimation(pinataTracker);
            }

            EnableBodyAnimation(tracker);
        }


        private void DisableHeadAnimation(TrackEntry pinataTracker)
        {
            pinataHead.SetActive(true);
            pinataSkeleton.gameObject.SetActive(false);

            isHitEmotionAllow = true;
            tracker.Complete -= DisableHeadAnimation;
        }


        private void DisableBodyAnimation(TrackEntry pinataTracker)
        {
            pinataSkeleton.gameObject.SetActive(false);
            pinataBody.SetActive(true);
        }


        private void EnableBodyAnimation(TrackEntry pinataTracker)
        {
            pinataSkeleton.gameObject.SetActive(true);
            tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, IDLE, true);

            if (disableBodyCorutine == null)
            {
                StartCoroutine(DisableBody());
            }
        }

        private IEnumerator DisableBody()
        {
            yield return null;
            pinataBody.SetActive(false);
            disableBodyCorutine = null;
        }

        private IEnumerator LeavePinataWithAnim()
        {
            yield return new WaitForSeconds(pinataSkeleton.SkeletonDataAsset.GetSkeletonData(true).FindAnimation(FAIL).Duration);

            tracker = pinataSkeleton.AnimationState.SetAnimation(ANIMATION_INDEX, ESCAPE, false);

            OnStartPinataLeave();
        }


        private IEnumerator IdleAnimationCountdown()
        {
            yield return new WaitForSeconds(idleAnimationDelay);
            PlayIdleAnimation(tracker);
        }

        #endregion

    }
}
