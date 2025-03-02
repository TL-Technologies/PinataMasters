using System.Text;
using UnityEngine;


namespace Modules.Hive.Utilities
{
    public class PlatformSplash
    {
        public string Description { get; }


        internal string SerializedPropertyName { get; }


        public int Width { get; }


        public int Height { get; }


        public PlatformSplashOrientation Orientation { get; }


        public int TargetDevice { get; }


        public Texture2D Texture { get; set; }


        public bool HasFixedSize => Width > 0 && Height > 0;


        internal PlatformSplash(
            string serializedPropertyName,
            string description,
            int width,
            int height,
            int targetDevice,
            PlatformSplashOrientation orientation = PlatformSplashOrientation.Universal)
        {
            SerializedPropertyName = serializedPropertyName;
            Description = description;
            Width = width;
            Height = height;
            Orientation = orientation;
            TargetDevice = targetDevice;
        }


        internal PlatformSplash(
            string serializedPropertyName, 
            string description,
            int targetDevice, 
            PlatformSplashOrientation orientation = PlatformSplashOrientation.Universal) : 
            this(serializedPropertyName, description, 0, 0, targetDevice, orientation) { }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(Description))
            {
                sb.Append(Description);
                sb.Append(" ");
            }

            bool isEmpty = true;
            if (HasFixedSize)
            {
                sb.Append("(");
                sb.Append($"{Width}x{Height}");
                isEmpty = false;
            }

            if (Orientation != PlatformSplashOrientation.Universal)
            {
                sb.Append(isEmpty ? "(" : ", ");
                sb.Append(Orientation);
                isEmpty = false;
            }

            if (!isEmpty)
            {
                sb.Append(")");
            }

            return sb.ToString();
        }
    }
}
