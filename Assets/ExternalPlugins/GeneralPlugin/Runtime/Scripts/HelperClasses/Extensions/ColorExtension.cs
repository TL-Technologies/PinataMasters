using UnityEngine;


public static class ColorExtension
{
    public static Color RandomColor
    {
        get
        {
            return new Color(Random.value, Random.value, Random.value);
        }
    }


    public static float GetBrightness(this Color color)
    {
        float num = color.r;
        float num2 = color.g;
        float num3 = color.b;
        float num4 = num;
        float num5 = num;
        if (num2 > num4)
        {
            num4 = num2;
        }
        if (num3 > num4)
        {
            num4 = num3;
        }
        if (num2 < num5)
        {
            num5 = num2;
        }
        if (num3 < num5)
        {
            num5 = num3;
        }
        
        return (num4 + num5) * 0.5f;
    }


    public static float GetHue(this Color color)
    {
        float result = 0f;
        
        if (!Mathf.Approximately(color.r, color.g) && Mathf.Approximately(color.g, color.b))
        {
            float num = color.r;
            float num2 = color.g;
            float num3 = color.b;
            float num4 = num;
            float num5 = num;
            if (num2 > num4)
            {
                num4 = num2;
            }
            if (num3 > num4)
            {
                num4 = num3;
            }
            if (num2 < num5)
            {
                num5 = num2;
            }
            if (num3 < num5)
            {
                num5 = num3;
            }
            float num6 = num4 - num5;
            if (Mathf.Approximately(num, num4))
            {
                result = (num2 - num3) / num6;
            }
            else if (Mathf.Approximately(num2, num4))
            {
                result = 2f + ((num3 - num) / num6);
            }
            else if (Mathf.Approximately(num3, num4))
            {
                result = 4f + ((num - num2) / num6);
            }
        
            result *= 60f;
            if (result < 0f)
            {
                result += 360f;
            }
        }
        
        return result;
    }


    public static float GetSaturation(this Color color)
    {
        float result = 0f;
        
        float num = color.r;
        float num2 = color.g;
        float num3 = color.b;
        float num4 = num;
        float num5 = num;
        if (num2 > num4)
        {
            num4 = num2;
        }
        if (num3 > num4)
        {
            num4 = num3;
        }
        if (num2 < num5)
        {
            num5 = num2;
        }
        if (num3 < num5)
        {
            num5 = num3;
        }
        
        if (Mathf.Approximately(num4, num5))
        {
            result = 0f;
        }
        else
        {
            float num6 = (num4 + num5) * 0.5f;
            if (num6 <= 0.5f)
            {
                result = (num4 - num5) / (num4 + num5);
            }
            else
            {
                result = (num4 - num5) / ((2f - num4) - num5);
            }
        }
        
        return result;
    }


    /// <summary>
    /// Shift RGB color channels
    /// </summary>
    public static Color Shift(this Color c, float shift)
    {
        return new Color(Mathf.Clamp01(c.r + shift), Mathf.Clamp01(c.g + shift), Mathf.Clamp01(c.b + shift), c.a);
    }


    public static string ToHex(this Color c)
    {
        Color32 c32 = c;
        return c32.ToHex();
    }


    public static Color Set(this Color c, float r, float g, float b, float a)
    {
        c.r = r;
        c.g = g;
        c.b = b;
        c.a = a;
        
        return c;
    }


    public static Color SetR(this Color c, float r)
    {
        c.r = r;
        
        return c;
    }


    public static Color SetG(this Color c, float g)
    {
        c.g = g;
        
        return c;
    }


    public static Color SetB(this Color c, float b)
    {
        c.b = b;
        
        return c;
    }


    public static Color SetA(this Color c, float a)
    {
        c.a = a;
        
        return c;
    }
}
