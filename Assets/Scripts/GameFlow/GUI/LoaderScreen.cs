using DG.Tweening;
using Modules.General.Utilities;
using PinataMasters;
using System;
using UnityEngine;
using UnityEngine.UI;


public class LoaderScreen : MonoBehaviour
{
    #region Fields

    public static readonly ResourceGameObject<LoaderScreen> Prefab = new ResourceGameObject<LoaderScreen>("Game/GUI/LoaderScreen");
    
    public event Action OnLoaderHide;
    
    [SerializeField] private RawImage loaderImage;
    [SerializeField] private RawImage backImage;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private LayoutElement loaderLayoutElement;
    [SerializeField] private LayoutElement backLayoutElement;
    
    private ISplashHelper splashHelper;
    
    #endregion
    
    
    
    #region Properties
    
    private ISplashHelper SplashHelper => splashHelper ?? (splashHelper = new SplashHelper());
    
    #endregion

    
    
    #region Methods
    
    public void Show()
    {
        loaderImage.gameObject.SetActive(true);
        loaderImage.texture = SplashHelper.LoadSplash();
        loaderImage.color = Color.white;

        Texture texture = loaderImage.texture;
        if (texture != null)
        {
            Vector2 splashSize = SplashHelper.SplashSize;
            loaderLayoutElement.preferredWidth = splashSize.x;
            loaderLayoutElement.preferredHeight = splashSize.y;
        }
        
        backImage.gameObject.SetActive(true);
        backImage.color = Color.black;
        backLayoutElement.preferredWidth = Screen.width;
        backLayoutElement.preferredHeight = Screen.height;
    } 
    
    
    public void Hide()
    {
        if (fadeDuration > float.Epsilon)
        {
            backImage.DOFade(0.0f, fadeDuration).SetEase(fadeCurve);
            loaderImage.DOFade(0.0f, fadeDuration).SetEase(fadeCurve).OnComplete(OnComplete);
        } 
        else
        {
            OnComplete();
        }
    }


    private void OnDestroy()
    {
        SplashHelper.UnloadSplash(loaderImage.texture as Texture2D);
    }


    private void OnComplete()
    {
        backImage.gameObject.SetActive(false);
        loaderImage.gameObject.SetActive(false);
        OnLoaderHide?.Invoke();
    }

    #endregion
    
}
