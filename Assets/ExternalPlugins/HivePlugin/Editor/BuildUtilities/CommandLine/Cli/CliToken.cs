using System;


namespace Modules.Hive.Editor.BuildUtilities
{
    public class CliToken
    {
        public readonly CliTokenType type;
        public readonly string text;

        internal CliToken(CliTokenType tokenType, string text)
        {
            type = tokenType;
            this.text = text;
        }


        public static bool Equals(CliToken token, string text, StringComparison stringComparison = StringComparison.OrdinalIgnoreCase)
        {
            return token != null && string.Equals(token.text, text, stringComparison);
        }
    }
}
