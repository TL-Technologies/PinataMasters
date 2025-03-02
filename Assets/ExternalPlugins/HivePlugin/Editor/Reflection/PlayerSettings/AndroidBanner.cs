using UnityEngine;


namespace Modules.Hive.Editor.Reflection
{
    public class AndroidBanner
    {
        public int Width { get; }
        public int Height { get; }
        public Texture2D Texture { get; set; }


        internal AndroidBanner(int width, int height, Texture2D texture)
        {
            Width = width;
            Height = height;
            Texture = texture;
        }


        public override string ToString() => $"Android banner {Width}x{Height}";
    }
}
