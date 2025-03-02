namespace Modules.Hive.Storage
{
    public interface IDataSourceFactory
    {
        IDataSource<T> CreateDefaultDataSource<T>(string identifier)
            where T : DataModel, new();

        IDataSource<T> CreateDefaultKeyValueDataSource<T>(string identifier, string key)
            where T : DataModel, new();

        IDataSource<T> CreateDefaultFileSystemDataSource<T>(
            string identifier,
            string path,
            StorageScope scope = StorageScope.Roaming)
            where T : DataModel, new();
    }


    public static class DataSourceFactoryExtensions
    {
        public static IDataSource<T> CreateDefaultDataSource<T>(this IDataSourceFactory factory)
            where T : DataModel, new()
        {
            return factory.CreateDefaultDataSource<T>(null);
        }


        public static IDataSource<T> CreateDefaultKeyValueDataSource<T>(this IDataSourceFactory factory, string key)
            where T : DataModel, new()
        {
            return factory.CreateDefaultKeyValueDataSource<T>(null, key);
        }


        public static IDataSource<T> CreateDefaultFileSystemDataSource<T>(this IDataSourceFactory factory,
            string path,
            StorageScope scope = StorageScope.Roaming)
            where T : DataModel, new()
        {
            return factory.CreateDefaultFileSystemDataSource<T>(null, path, scope);
        }
    }
}
