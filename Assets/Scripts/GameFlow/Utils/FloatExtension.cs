namespace PinataMasters
{
    public static class FloatExtension
    {
        public static string ToShortFormat(this float value, bool isThreeDigits = false)
        {
            int threeZeros = 0;

            while (value >= 1000f)
            {
                value *= 0.001f;
                threeZeros++;
            }

            string postfix = string.Empty;
            switch (threeZeros)
            {
                case 1:
                    postfix = "K";
                    break;

                case 2:
                    postfix = "M";
                    break;

                case 3:
                    postfix = "B";
                    break;

                case 4:
                    postfix = "T";
                    break;

                case 5:
                    postfix = "aa";
                    break;

                case 6:
                    postfix = "bb";
                    break;

                case 7:
                    postfix = "cc";
                    break;

                case 8:
                    postfix = "dd";
                    break;

                case 9:
                    postfix = "ee";
                    break;

                case 10:
                    postfix = "ff";
                    break;

                case 11:
                    postfix = "gg";
                    break;

                case 12:
                    postfix = "hh";
                    break;

                case 13:
                    postfix = "ii";
                    break;

                case 14:
                    postfix = "jj";
                    break;

                case 15:
                    postfix = "kk";
                    break;

                case 16:
                    postfix = "ll";
                    break;

                case 17:
                    postfix = "mm";
                    break;

                case 18:
                    postfix = "nn";
                    break;

                case 19:
                    postfix = "oo";
                    break;

                case 20:
                    postfix = "pp";
                    break;

                case 21:
                    postfix = "qq";
                    break;

                case 22:
                    postfix = "rr";
                    break;

                case 23:
                    postfix = "ss";
                    break;

                case 24:
                    postfix = "tt";
                    break;

                case 25:
                    postfix = "uu";
                    break;

                case 26:
                    postfix = "vv";
                    break;

                case 27:
                    postfix = "ww";
                    break;

                case 28:
                    postfix = "xx";
                    break;

                case 29:
                    postfix = "yy";
                    break;

                case 30:
                    postfix = "zz";
                    break;
            }

            if (isThreeDigits)
            {
                string resultFormat = string.Empty;

                if (value > (100.0f - float.Epsilon))
                {
                    resultFormat = "{0:F0}{1}";
                }
                else if (value > (10.0f - float.Epsilon))
                {
                    resultFormat = "{0:F1}{1}";
                }
                if (!string.IsNullOrEmpty(resultFormat))
                {
                    return string.Format(resultFormat, value, postfix);
                }
            }

            return string.Format(threeZeros == 0 ? "{0:F0}" : "{0:0.##}{1}", value, postfix);
        }
    }
}
