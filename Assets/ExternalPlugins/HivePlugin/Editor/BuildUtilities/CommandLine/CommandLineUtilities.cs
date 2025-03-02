using System;
using UnityEditor;


namespace Modules.Hive.Editor.BuildUtilities
{
    public static class CommandLineUtilities
    {
        public static BuildTargetGroup GetBuildTargetGroup()
        {
            BuildTargetGroup result = BuildTargetGroup.Unknown;
            
            string[] args = Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i].Equals("-buildTarget"))
                {
                    string platformName = args[i + 1];
                    if (platformName.Equals("iOS"))
                    {
                        result = BuildTargetGroup.iOS;
                    }
                    if (platformName.Equals("Android"))
                    {
                        result = BuildTargetGroup.Android;
                    }
                        
                    break;
                }
            }
            
            return result;
        }
    }
}