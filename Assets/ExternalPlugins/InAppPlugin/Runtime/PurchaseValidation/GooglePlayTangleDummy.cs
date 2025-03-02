// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangleDummy
    {
        private static byte[] data = System.Convert.FromBase64String("hkmYs3mGNxmoOZB3HuQJ+5AnOjJOzcPM/E7Nxs5Ozc3MXMywM6Vnx+kLt2qLUPrzvcXM4bQOz+hxKGf6WtZLMlvM2OmlfZAq8uAuqfyaxU2sUHtbvrGaE+8+wRCRvIwPBwtVY/SvQh061683haZOL7N403g4Yz8WblkLwcT3Mm54yomSpMCQLXPX6KyfuYPzzGkhBvs21A0ujG6npawwuDSDaQvuL5rECv5hrP6t3+KD9HsaVK9oHsb61lySiFWM9lbjL2SfIvJ3tNno25VQzH+Yqzcbuc0iIsIE52iPZtQiwf4UmWLOKNSMoo4iuoBO/E7N7vzBysXmSoRKO8HNzc3JzM9+wvYjlR+srJjuMaggHDP7WJl5BG6wcKZfgsSMlc7PzczN");
        private static int[] order = new int[] { 8,1,6,6,13,6,6,12,12,11,10,11,12,13,14 };
        private static int key = 204;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
