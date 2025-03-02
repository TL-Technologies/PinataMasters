using System;
using System.Collections.Generic;


namespace Modules.Hive.Editor.BuildUtilities
{
    public abstract class CommandModel
    {
        private Dictionary<string, Action<CliToken, CliReader>> options = new Dictionary<string, Action<CliToken, CliReader>>();


        protected void RegisterOptionParser(string option, Action<CliToken, CliReader> action)
        {
            options.Add(option.ToLowerInvariant(), action);
        }


        protected void Parse(string[] args)
        {
            CliReader reader = new CliReader(args);

            while (true)
            {
                CliToken token = reader.GetNextOption();
                if (token == null)
                {
                    break;
                }

                if (options.TryGetValue(token.text.ToLowerInvariant(), out Action<CliToken, CliReader> action))
                {
                    action(token, reader);
                }
            }

            OnValidate();
        }


        protected virtual void OnValidate() { }
    }
}
