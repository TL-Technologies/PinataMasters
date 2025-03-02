using System;
using System.Text;


public static class StringExtension 
{
    const char UNDERSCORE_SYMBOL = '_';
    const int STANDARD_VARIABLE_NAME_MAX_WORDS_COUNT = 5;
    public static char ASCII_NL = '\n';
    public static char ASCII_WS = ' ';

    public static string GetShortNumberForm(this string s)
    {
        StringBuilder formattedText = new StringBuilder(s);
        
        int currentNumberCount = 0;
        for (int i = formattedText.Length - 1; i >= 0; i--)
        {
            bool digitCharacter = false;
            var curChar = formattedText[i];
            if (curChar >= '0' && curChar <= '9')
            {
                digitCharacter = true;
                currentNumberCount += 1;
            }
            if ((!digitCharacter) || ((digitCharacter) && (i == 0)))
            {
                if (currentNumberCount > 3)
                {
                    int numberStartIdx = (digitCharacter) ? i : (i + 1);

                    int lettersToCut = currentNumberCount > 6 ? 6 : 3;

                    string endLetter = lettersToCut == 6 ? "M" : "k";

                    int numberFinishIdx = numberStartIdx + currentNumberCount;
                    int pointPlace = numberFinishIdx - lettersToCut;
                    int cutStartIdx = pointPlace;
                    if (formattedText[cutStartIdx] != '0' && ((currentNumberCount - lettersToCut) <= 2))
                    {
                        formattedText.Insert(pointPlace, ".");
                        cutStartIdx += 2;
                        lettersToCut -= 1;
                    }
                    formattedText.Remove(cutStartIdx, lettersToCut);
                    formattedText.Insert(cutStartIdx, endLetter);
                }
                currentNumberCount = 0;
            }
        }

        return formattedText.ToString();
    }
    

    /// <summary>
    /// Remove first 32 non-printing characters, except new line, from string
    /// </summary>
    public static string RemoveCharsControl(this string s)
    {        
        if (s != null)
        {
            var len = s.Length;
            var src = s.ToCharArray();
            int dstIdx = 0;
            for (int i = 0; i < len; i++) {
                var ch = src[i];
                if ((ch < ASCII_WS) && (ch != ASCII_NL))
                {
                    continue;
                }
                src[dstIdx++] = ch;
            }
            s = new string(src, 0, dstIdx);
        }
        return s;
    }


    /// <summary>
    /// Remove new line character, from string
    /// </summary>
    public static string RemoveCharsNewline(this string s)
    {
        return s.RemoveChars(ASCII_NL);
    }


    /// <summary>
    /// Remove white space character, from string
    /// </summary>
    public static string RemoveCharsWhiteSpace(this string s)
    {
        return s.RemoveChars(ASCII_WS);
    }


    /// <summary>
    /// Remove selected character, from string
    /// </summary>
    public static string RemoveChars(this string s, char removeChar) 
    {
        if (s != null)
        {
            var len = s.Length;
            var src = s.ToCharArray();
            int dstIdx = 0;
            for (int i = 0; i < len; i++) {
                var ch = src[i];
                if (ch != removeChar)
                {
                    src[dstIdx++] = ch;
                    continue;
                }
            }
            s = new string(src, 0, dstIdx);
        }
        return s;
    }


    /// <summary>
    /// Convert string from PascalCase (camelCase) to snake_case
    /// </summary>
    public static string PascalToSnakeCase(this string s)
    {
        StringBuilder result = new StringBuilder(s, s.Length + STANDARD_VARIABLE_NAME_MAX_WORDS_COUNT);

        for (int i = result.Length - 1; i >= 0; i--)
        {
            if (char.IsUpper(result[i]))
            {
                result[i] = char.ToLowerInvariant(result[i]);

                if (i > 0)
                {
                    result.Insert(i, UNDERSCORE_SYMBOL);
                }
            }
        }

        return result.ToString();
    }
    
    
    public static string ReplaceLastOccurrence(this string source, string find, string replace)
    {
        string result = source;
        int place = source.LastIndexOf(find, StringComparison.InvariantCulture);

        if (place >= 0)
        {
            result = source.Remove(place, find.Length).Insert(place, replace);
        }
        
        return result;
    }


    public static float ParseFloat(this string str, float defaultValue = 0f)
    {
        float result;
        if (!float.TryParse(str, out result))
        {
            result = defaultValue;
        }

        return result;
    }


    public static int ParseInt(this string str, int defaultValue = 0)
    {
        int result;
        if (!int.TryParse(str, out result))
        {
            result = defaultValue;
        }

        return result;
    }


    public static ushort ParseUShort(this string str, ushort defaultValue = 0)
    {
        ushort result;
        if (!ushort.TryParse(str, out result))
        {
            result = defaultValue;
        }

        return result;
    }


    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);


    public static int LastIndexOf(this string s, Func<char, bool> callback)
    {
        for (int i = s.Length - 1; i >= 0; i--)
        {
            if (callback(s[i]))
            {
                return i;
            }
        }

        return -1;
    }
    
    
    public static int IndexOf(this string s, Func<char, bool> callback)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (callback(s[i]))
            {
                return i;
            }
        }

        return -1;
    }
}
