using System;


namespace Modules.Hive.Editor.BuildUtilities.Android
{
    public interface IReadOnlyGradleScript
    {
        bool IsMultiDexEnabled { get; }
        ExtendedVersion GradlePackageVersion { get; }
    }
}
