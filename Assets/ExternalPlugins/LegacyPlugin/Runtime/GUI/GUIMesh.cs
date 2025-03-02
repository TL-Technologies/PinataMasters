using UnityEngine;
using System.Collections;


public class GUIMesh : MonoBehaviour, ILayoutCellHandler
{
    #region Variables

    [SerializeField] Renderer meshRenderer;
    [SerializeField] bool isUniformScale = true;

    Vector3 cachedMeshDimensions = Vector3.zero;
    Renderer cachedRenderer;
    Transform cachedTransform;

    #endregion


    #region Properties

    Vector3 CachedMeshDimensions
    {
        get 
        {
            if (cachedMeshDimensions == Vector3.zero)
            {                
                if (cachedRenderer != null)
                {
                    cachedMeshDimensions = cachedRenderer.bounds.size;
                    Vector3 meshScale = CachedTransform.lossyScale;

                    meshScale.x = (Mathf.Abs(meshScale.x) > 0f) ? (1f / Mathf.Abs(meshScale.x)) : (1f);
                    meshScale.y = (Mathf.Abs(meshScale.y) > 0f) ? (1f / Mathf.Abs(meshScale.y)) : (1f);
                    meshScale.z = (Mathf.Abs(meshScale.z) > 0f) ? (1f / Mathf.Abs(meshScale.z)) : (1f);

                    cachedMeshDimensions.Scale(meshScale);
                }
            }

            return cachedMeshDimensions; 
        }
    }


    Transform CachedTransform
    {
        get
        {
            if (cachedTransform == null)
            {
                cachedTransform = transform;
            }

            return cachedTransform;
        }
    }
        
    #endregion


    #region Unity lifecycle

    void Awake()
    {
        InitRendererReference();
    }

    #endregion


    #region Private methods

    void InitRendererReference()
    {
        cachedRenderer = (meshRenderer != null) ? (meshRenderer) : (GetComponent<Renderer>());
    }

    #endregion


    #region ILayoutCellHandler

    virtual public void RepositionForCell(LayoutCellInfo info)
    {
        #if UNITY_EDITOR
        if (cachedRenderer == null && !Application.isPlaying)
        {
            InitRendererReference();
        }
        #endif

        if (cachedRenderer != null)
        {         
            Vector3 currentLocalPosition = CachedTransform.localPosition;
            if (info.anchor == AnchorType.Center)
            {
                currentLocalPosition.x = 0f;
                currentLocalPosition.y = 0f;
            }
            else if (info.anchor == AnchorType.Left)
            {
                currentLocalPosition.x = -info.cellRect.width * 0.5f;
            }
            else if (info.anchor == AnchorType.Right)
            {
                currentLocalPosition.x = info.cellRect.width * 0.5f;
            }
            else if (info.anchor == AnchorType.Bottom)
            {
                currentLocalPosition.y = -info.cellRect.height * 0.5f;
            }
            else if (info.anchor == AnchorType.Top)
            {
                currentLocalPosition.y = info.cellRect.height * 0.5f;
            }  
            CachedTransform.localPosition = currentLocalPosition;

            Vector3 targetScale = new Vector3(info.cellRect.width / CachedMeshDimensions.x, 
                info.cellRect.height / CachedMeshDimensions.y, CachedTransform.localScale.z);
            if (isUniformScale)
            {
                float minScale = Mathf.Min(targetScale.x, targetScale.y);
                targetScale.x = minScale;
                targetScale.y = minScale;
                targetScale.z = minScale;
            }
            CachedTransform.localScale = targetScale;
        }
    }

    #endregion
}