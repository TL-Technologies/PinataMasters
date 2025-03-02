using System.Collections.Generic;


namespace Modules.Hive.Storage
{
    public class InMemoryKeyValueStorage : Dictionary<string, string>, IKeyValueStorage
    {
        public void Save()
        {
            // Do nothing here
        }
    }
}
