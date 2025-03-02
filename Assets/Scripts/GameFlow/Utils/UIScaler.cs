using UnityEngine;


public class UIScaler : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private float k = 0f;
    [SerializeField]
    private float b = 0f;

    #endregion



    #region Properties

    public float Scale
    {
        get { return k * (Camera.main.aspect) + b; }
    }

    #endregion
}
