using System;
using System.Collections.Generic;
using System.Text;


namespace Modules.Hive.Editor
{
    public class PreprocessorDirectivesCollection : HashSet<string>, IPreprocessorDirectivesCollection
    {
        public const char DefaultSeparator = ';';


        public PreprocessorDirectivesCollection() { }


        public PreprocessorDirectivesCollection(IEnumerable<string> collection) : base(collection) { }


        public PreprocessorDirectivesCollection(string preprocessorDirectives)
        {
            UnionWith(preprocessorDirectives);
        }


        public void UnionWith(string preprocessorDirectives)
        {
            if (string.IsNullOrWhiteSpace(preprocessorDirectives))
            {
                return;
            }

            string[] array = preprocessorDirectives.Split(new[] { ';', ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            UnionWith(array);
        }


        public override string ToString()
        {
            StringBuilder result = new StringBuilder();

            foreach (var directive in this)
            {
                result.Append(directive);
                result.Append(DefaultSeparator);
            }

            if (result.Length > 0)
            {
                result.Remove(result.Length - 1, 1);
            }

            return result.ToString();
        }
    }
}
