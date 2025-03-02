using System.Collections.Generic;


namespace Modules.Hive.Storage
{
    public interface IDataSourceHub : ICollection<IDataSource>
    {
        bool Contains(string identifier);
        bool Remove(string identifier);

        IDataSource GetDataSource(string identifier);

        IDataSource this[string identifier] { get; }

        void SaveAll();
        void CloseAll(bool save = true);
        void DeleteAll();
    }


    public static class DataSourceHubExtensions
    {
        public static IDataSource<T> GetDataSource<T>(this IDataSourceHub collection, string identifier) 
            where T : DataModel
        {
            IDataSource dataSource = collection.GetDataSource(identifier);
            if (dataSource == null)
            {
                return null;
            }

            if (dataSource is IDataSource<T> ds)
            {
                return ds;
            }

            throw new System.InvalidCastException(
                $"Unable to reinterpret a data source of type {dataSource.GetType()} as data source of type {typeof(T)}");
        }


        public static IDataSource<T> GetDataSource<T>(this IDataSourceHub collection)
            where T : DataModel
        {
            return GetDataSource<T>(collection, DataSourceUtilities.GetIdentifierFromType<T>());
        }
    }
}
