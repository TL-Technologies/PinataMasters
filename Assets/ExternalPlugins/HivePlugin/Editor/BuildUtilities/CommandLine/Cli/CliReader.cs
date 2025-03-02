namespace Modules.Hive.Editor.BuildUtilities
{
    public class CliReader
    {
        #region Fields
        
        public readonly string[] args;

        private int currentTokenIndex = -1;
        
        #endregion



        #region Methods

        public CliReader(string[] args)
        {
            this.args = args;
        }


        public CliToken GetNextToken()
        {
            if (currentTokenIndex >= args.Length - 1)
            {
                return null;
            }

            currentTokenIndex++;
            string text = args[currentTokenIndex];

            if (currentTokenIndex == 0)
            {
                return new CliToken(CliTokenType.Program, text);
            }

            if (text.Length > 1 && text.StartsWith("-"))
            {
                return new CliToken(CliTokenType.Option, text.TrimStart('-'));
            }

            return new CliToken(CliTokenType.Value, text);
        }


        public CliToken GetNextOption()
        {
            while (true)
            {
                CliToken token = GetNextToken();

                if (token == null)
                {
                    return null;
                }

                if (token.type == CliTokenType.Option)
                {
                    return token;
                }
            }
        }


        public CliToken GetNextValue()
        {
            CliToken token = GetNextToken();

            if (token == null)
            {
                return null;
            }

            if (token.type == CliTokenType.Value)
            {
                return token;
            }

            StepBack();
            return null;
        }
        
        
        private void StepBack()
        {
            if (currentTokenIndex > -1)
            {
                currentTokenIndex--;
            }
        }

        #endregion
    }
}
