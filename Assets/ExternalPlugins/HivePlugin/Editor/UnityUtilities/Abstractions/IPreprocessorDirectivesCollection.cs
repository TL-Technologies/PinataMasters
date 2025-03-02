using System.Collections.Generic;


namespace Modules.Hive.Editor
{
    public interface IPreprocessorDirectivesCollection : IReadOnlyCollection<string>
    {
        bool Contains(string directive);
        bool Add(string directive);

        // Keep in mind that by design this collection should allow only appending.
        // Removing items violates data integrity. Do not define methods Remove, Clean, etc...
        // bool Remove(string directive);
    }
}
