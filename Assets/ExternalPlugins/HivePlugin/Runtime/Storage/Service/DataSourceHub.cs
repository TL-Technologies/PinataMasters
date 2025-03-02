using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace Modules.Hive.Storage
{
    public class DataSourceHub : IDataSourceHub
    {
        private Dictionary<string, IDataSource> dataSources = new Dictionary<string, IDataSource>();


        public int Count => dataSources.Count;


        public bool IsReadOnly => false;


        public bool Contains(string identifier)
        {
            return dataSources.ContainsKey(identifier);
        }


        public bool Contains(IDataSource dataSource)
        {
            return dataSources.TryGetValue(dataSource.Identifier, out IDataSource item) &&
                object.ReferenceEquals(dataSource, item);
        }


        public void Add(IDataSource dataSource)
        {
            dataSources.Add(dataSource.Identifier, dataSource);
        }


        public bool Remove(string identifier)
        {
            return dataSources.Remove(identifier);
        }


        public bool Remove(IDataSource dataSource)
        {
            return Contains(dataSource) ? dataSources.Remove(dataSource.Identifier) : false;
        }


        public void Clear()
        {
            dataSources.Clear();
        }


        public IDataSource GetDataSource(string identifier)
        {
            if (dataSources.TryGetValue(identifier, out IDataSource dataSource))
            {
                return dataSource;
            }

            return null;
        }


        public IDataSource this[string identifier] => dataSources[identifier];


        public void CopyTo(IDataSource[] array, int arrayIndex)
        {
            foreach (var item in dataSources)
            {
                array[arrayIndex] = item.Value;
                arrayIndex++;
            }
        }


        public IEnumerator<IDataSource> GetEnumerator()
        {
            return dataSources.Select(p => p.Value).GetEnumerator();
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void SaveAll()
        {
            EnumerateSafety(nameof(SaveAll), d => d.Save());
        }

        
        public void CloseAll(bool save = true)
        {
            EnumerateSafety(nameof(CloseAll), d => d.Close(save));
        }
        

        public void DeleteAll()
        {
            EnumerateSafety(nameof(DeleteAll), d => d.Delete());
        }


        private void EnumerateSafety(string methodName, Action<IDataSource> action)
        {
            List<Exception> exceptions = null;

            var enumerator = dataSources.GetEnumerator();
            while (enumerator.MoveNext())
            {
                try
                {
                    action(enumerator.Current.Value);
                }
                catch (Exception e)
                {
                    if (exceptions == null)
                    {
                        exceptions = new List<Exception>();
                    }

                    exceptions.Add(e);
                }
            }
            enumerator.Dispose();

            if (exceptions != null)
            {
                throw new AggregateException($"One or more exceptions occurred in method '{methodName}'.", exceptions);
            }
        }
    }
}
