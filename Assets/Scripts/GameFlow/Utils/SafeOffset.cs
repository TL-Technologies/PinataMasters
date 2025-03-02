using UnityEngine;


namespace PinataMasters
{
    public static class SafeOffset
    {
        public static float GetSafeTopRatio(Rect canvasRect)
        {
            float areaHeight = canvasRect.height * Screen.safeArea.yMax / Screen.height;
            return (canvasRect.height - areaHeight) / canvasRect.height;
        }


        public static float GetSafeTopWithBannerRatio(Rect canvasRect)
        {
            float areaHeight = canvasRect.height * Screen.safeArea.yMax / Screen.height;
            float bannerHeight = GetBannerheight() * canvasRect.height / Screen.height;
            return (canvasRect.height - areaHeight + bannerHeight) / canvasRect.height;
        }


        private static float GetBannerheight()
        {
            float result = 180.0f;
            
            #if UNITY_IOS && !UNITY_EDITOR
                if (Screen.height / Screen.width < 1.34f)
                {
                    result = 100.0f;
                }
            #elif UNITY_ANDROID && !UNITY_EDITOR
                if (isTablet)
                {
                    result = 100.0f;
                }
            #endif

            return result;
        }
        
        
        private static float DeviceDiagonalSizeInInches()
        {
            float diagonalInches = Mathf.Sqrt (Mathf.Pow (Screen.width / Screen.dpi, 2) + Mathf.Pow (Screen.height / Screen.dpi, 2));

            return diagonalInches;
        }

        
        private static bool isTablet => (DeviceDiagonalSizeInInches() > 6.5f && (Screen.height / Screen.width) < 2.0f);
    }
}