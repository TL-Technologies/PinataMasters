using System.IO;
using Modules.Haptic.Editor;
using Modules.Hive.Editor;
using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.BuildUtilities.Android;
//using UnityEditor.iOS.Xcode;
//using UnityEditor.iOS.Xcode.Extensions;
using UnityEngine;

namespace MoreMountains.NiceVibrations
{
    public class HapticPluginAdapterBuildProcess : IBuildPreprocessor<IAndroidBuildPreprocessorContext>
    {
        public void OnPreprocessBuild(IAndroidBuildPreprocessorContext context)
        {
            context.AndroidManifest.AddPermissionElement(Permission.Vibrate);
        }
    }
}

