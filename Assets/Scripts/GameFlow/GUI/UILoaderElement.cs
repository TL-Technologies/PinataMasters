using DG.Tweening;
using UnityEngine;


public class UILoaderElement : MonoBehaviour
{
    #region Fields

    [SerializeField] float loaderDuration = 0f;
    [SerializeField] RectTransform loader;

    #endregion


    
    #region Unity lifecycle

    protected void Awake()
    {
        loader.DORotate(new Vector3(0f, 0f, 360f), loaderDuration, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear);
    }

    #endregion



    #region Public methods

    public void ShowOnParent(Transform parent, Vector3 position, Vector3 scale)
    {
        transform.parent = parent;
        transform.position = parent.transform.TransformPoint(position);
        transform.localScale = scale;
    }

    #endregion
}
