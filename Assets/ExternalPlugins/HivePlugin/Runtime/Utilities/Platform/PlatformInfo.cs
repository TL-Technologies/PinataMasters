namespace Modules.Hive
{
    public static class PlatformInfo
    {
        public static AndroidTarget AndroidTarget
        {
            get
            {
                #if UNITY_ANDROID && HIVE_GOOGLEPLAY
                    return AndroidTarget.GooglePlay;
                #elif UNITY_ANDROID && HIVE_AMAZON
                    return AndroidTarget.Amazon;
                #elif UNITY_ANDROID && HIVE_SAMSUNG
                    return AndroidTarget.Samsung;
                #elif UNITY_ANDROID && HIVE_HUAWEI
                    return AndroidTarget.Huawei;
                #else
                    return AndroidTarget.None;
                #endif
            }
        }
    }
}
