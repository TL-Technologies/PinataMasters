using System;
using System.Reflection;
using UnityEditor;


namespace Modules.Hive.Editor.Reflection
{
    public static class UnityModulesManagerHelper
    {
        private static readonly Type ModuleManagerType = Type.GetType("UnityEditor.Modules.ModuleManager, UnityEditor.dll");
        private static MethodInfo getTargetStringFromBuildTarget = null;
        private static MethodInfo getTargetStringFromBuildTargetGroup = null;
        private static MethodInfo isPlatformSupportLoaded = null;

        /// <summary>
        /// Returns a target string for specified build target
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public static string GetTargetStringFromBuildTarget(BuildTarget buildTarget)
        {
            if (getTargetStringFromBuildTarget == null)
            {
                getTargetStringFromBuildTarget = ModuleManagerType.GetMethod("GetTargetStringFromBuildTarget", BindingFlags.Static | BindingFlags.NonPublic);
            }

            return (string)getTargetStringFromBuildTarget.Invoke(null, new object[] { buildTarget });
        }
        

        /// <summary>
        /// Returns a target string for specified build target group
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        /// <returns></returns>
        public static string GetTargetStringFromBuildTargetGroup(BuildTargetGroup buildTargetGroup)
        {
            if (getTargetStringFromBuildTargetGroup == null)
            {
                getTargetStringFromBuildTargetGroup = ModuleManagerType.GetMethod("GetTargetStringFromBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);
            }

            return (string)getTargetStringFromBuildTargetGroup.Invoke(null, new object[] { buildTargetGroup });
        }

        
        /// <summary>
        /// Returns true if platform by target string is supported
        /// </summary>
        /// <param name="targetString"></param>
        /// <returns></returns>
        public static bool IsPlatformSupportLoaded(string targetString)
        {
            if (isPlatformSupportLoaded == null)
            {
                isPlatformSupportLoaded = ModuleManagerType.GetMethod("IsPlatformSupportLoaded", BindingFlags.Static | BindingFlags.NonPublic);
            }

            return (bool)isPlatformSupportLoaded.Invoke(null, new object[] { targetString });
        }
        

        /// <summary>
        /// Returns true if platform by build target is supported
        /// </summary>
        /// <param name="buildTarget"></param>
        /// <returns></returns>
        public static bool IsPlatformSupportLoaded(BuildTarget buildTarget)
        {
            return IsPlatformSupportLoaded(GetTargetStringFromBuildTarget(buildTarget));
        }
        

        /// <summary>
        /// Do action for each loaded platform support.
        /// </summary>
        public static void ForEachLoadedPlatformSupport(Action<BuildTargetGroup> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            foreach (var value in Enum.GetValues(typeof(BuildTargetGroup)))
            {
                BuildTargetGroup buildTargetGroup = (BuildTargetGroup)value;
                if (buildTargetGroup != BuildTargetGroup.Unknown && IsPlatformSupportLoaded(buildTargetGroup))
                {
                    action(buildTargetGroup);
                }
            }
        }
        

        /// <summary>
        /// Returns true if platform by build target group is supported.
        /// Standalone is always supported
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        /// <returns></returns>
        public static bool IsPlatformSupportLoaded(BuildTargetGroup buildTargetGroup)
        {
            return buildTargetGroup == BuildTargetGroup.Standalone || IsPlatformSupportLoaded(GetTargetStringFromBuildTargetGroup(buildTargetGroup));
        }

        
        /// <summary>
        /// Returns an array of ScriptingImplementation available for specified BuildTargetGroup.        
        /// </summary>
        /// <param name="buildTargetGroup"></param>
        /// <returns>Returns array or null.</returns>
        public static ScriptingImplementation[] GetEnabledScriptingImplementations(BuildTargetGroup buildTargetGroup)
        {
            MethodInfo mi = ModuleManagerType.GetMethod(
                "GetScriptingImplementations", 
                BindingFlags.Static | BindingFlags.NonPublic, 
                null, 
                new [] { typeof(BuildTargetGroup) }, 
                null);
            object siObj = mi.Invoke(null, new object[] { buildTargetGroup });

            if (siObj == null)
            {
                return null;
            }

            MethodInfo mi2 = siObj.GetType().GetMethod("Enabled", BindingFlags.Instance | BindingFlags.Public);
            ScriptingImplementation[] arr = mi2.Invoke(siObj, null) as ScriptingImplementation[];
            return arr;
        }
    }
}
