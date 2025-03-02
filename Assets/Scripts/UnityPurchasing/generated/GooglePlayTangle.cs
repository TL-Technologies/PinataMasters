// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("28Io7mTlkreEP4uEKigTf3OWE6nur0z7nIF8ECTaNWYPY5hQJ51UtgVwN4L4/BvxToduKLBWXzb6O09Kghvau0qjli74HlGlR1K/ebMfz7m1rU6+b/S+UMG0JFm98VcEpWdB5Fk3I6e5xWZ3y9w+0c0e3GHYCPcj1CFhFenSCvsdnC/SKONGmsKin0eekEk8u7BeTx9FEFiXr8QGzy1LsRCiIQIQLSYpCqZoptctISEhJSAjfSoY+9/0p+EAbLrYV+r7Tdf/0qefMfj+nctL6AyuOoiFtodeOv3l4M8cGAl8RLyMjaM30bkY1WyWeqvynfNQDjM0GA/fjM2DBhhV30ISqGCiIS8gEKIhKiKiISEgh8ilcPnEpiW7QLxZeKQ9USIjISAh");
        private static int[] order = new int[] { 11,13,8,9,7,12,10,7,11,11,12,12,12,13,14 };
        private static int key = 32;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
