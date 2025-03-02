using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    [Obsolete("AndroidSupportDependency class is deprecated. Use AndroidXDependency instead.")]
    public class AndroidSupportDependency : GradleDependency
    {
        public static readonly ExtendedVersion DefaultVersion = new ExtendedVersion("28.0.0");

        public AndroidSupportDependency(string libraryName) : base("com.android.support:" + libraryName)
        {
            if (!HasVersion)
            {
                Version = DefaultVersion;
            }
        }
    }
}
