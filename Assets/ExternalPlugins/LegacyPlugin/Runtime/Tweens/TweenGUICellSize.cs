using UnityEngine;

namespace Modules.Legacy.Tweens
{
    public class TweenGUICellSize : Tweener
    {
        #region Variables

        public float beginSize;
        public float endSize;

        [SerializeField] GUILayoutCell target;
        [SerializeField] GUILayouter rootLayouter;
        [SerializeField] GUILayouter targetCellLayouter;

        #endregion


        #region Properties

        public GUILayoutCell Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
                targetCellLayouter = null;
                rootLayouter = null;

                InitializeLayouters();
            }
        }

        #endregion


        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();

            InitializeLayouters();
        }

        #endregion


        #region Protected methods

        protected override void TweenUpdateRuntime(float factor, bool isFinished)
        {
            if (target != null)
            {
                target.SizeValue = beginSize + (endSize - beginSize) * factor;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (rootLayouter != null)
                {
                    Vector2 debugScreenSize = Vector2.zero;
                    if (SizeHelperSettings.Instance.isLayoutUpdaterUseScreenDimensions)
                    {
                        debugScreenSize.Set(ScreenDimensions.Width, ScreenDimensions.Height);
                    }
                    else
                    {
                        float width;
                        float height;
                        float aspect;
                        tk2dCamera.Editor__GetGameViewSize(out width, out height, out aspect);
                        debugScreenSize.Set(width, height);
                    }

                    rootLayouter.UpdateLayoutDebug(debugScreenSize);
                }
            }
            else
#endif
            {
                if (targetCellLayouter != null)
                {
                    targetCellLayouter.UpdateLayout();
                }
            }
        }

        #endregion


        #region Private methods

        void InitializeLayouters()
        {
            if (target != null)
            {
                Transform targetParent = target.CachedTransform.parent;
                if (targetParent != null)
                {
                    if (targetCellLayouter == null)
                    {
                        targetCellLayouter = targetParent.GetComponent<GUILayouter>();
                    }

                    if (rootLayouter == null)
                    {
                        GUILayouter[] layouters = target.GetComponentsInParent<GUILayouter>();
                        for (int i = 0; i < layouters.Length; i++)
                        {
                            if (layouters[i].IsRootLayouter)
                            {
                                rootLayouter = layouters[i];
                                break;
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}