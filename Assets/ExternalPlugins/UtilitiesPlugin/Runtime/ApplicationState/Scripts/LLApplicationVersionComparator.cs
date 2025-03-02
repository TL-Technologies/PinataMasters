using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR 
using System.Runtime.InteropServices;
#endif


public class LLApplicationVersionComparator : MonoBehaviour 
{
    #region Extern methods

    #if UNITY_IOS && !UNITY_EDITOR 
    [DllImport ("__Internal")]
    static extern bool LLApplicationVersionComparatorIsCurrentGameVersionLatest();
    #endif

    #endregion



    #region Properties

    public static bool IsCurrentGameVersionLatest
    {
        get
        {
            bool result = false;

            #if UNITY_IOS && !UNITY_EDITOR
                result = LLApplicationVersionComparatorIsCurrentGameVersionLatest();
            #elif UNITY_ANDROID && !UNITY_EDITOR
                result = !LLActivity.IsNeedUpdate;
            #endif

            return result;
        }
    }

    #endregion
}
