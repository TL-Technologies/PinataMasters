using UnityEditor;
using UnityEngine;


namespace Modules.Hive.Editor.Reflection
{
    public struct BuildPlatform
    {
        public string name;
        public string tooltip;
        public Texture2D smallIcon;
        public BuildTargetGroup buildTargetGroup;
    }
}
