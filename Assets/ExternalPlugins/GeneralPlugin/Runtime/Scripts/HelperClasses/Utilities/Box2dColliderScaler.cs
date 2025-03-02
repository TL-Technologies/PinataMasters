using UnityEngine;
using UnityEngine.EventSystems;


namespace Modules.General.Utilities
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class Box2dColliderScaler : UIBehaviour
    {
        #region Fields

        [SerializeField] private RectTransform rootTransform = null;
        [SerializeField] private BoxCollider2D rootCollider = null;

        #endregion



        #region Unity lifecycle
        
        #if UNITY_EDITOR
        protected override void Reset()
        {
            base.Reset();
        #else
        protected virtual void Reset()
        {
        #endif
            if (rootTransform == null)
            {
                rootTransform = GetComponent<RectTransform>();
            }

            if (rootCollider == null)
            {
                rootCollider = GetComponent<BoxCollider2D>();
            }

            OnRectTransformDimensionsChange();
        }

        #endregion
        


        #region Methods
        
        protected override void OnRectTransformDimensionsChange()
        {
            base.OnRectTransformDimensionsChange();

            if (rootTransform == null || rootCollider == null)
            {
                return;
            }

            Rect rect = rootTransform.rect;

            rootCollider.size = rect.size;
            rootCollider.offset = rect.center;
        }

        #endregion
    }
}

