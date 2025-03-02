namespace Modules.Hive.Storage
{
    public class StorageServiceConfig
    {
        public IDataSourceHub DataSources { get; }

        public IDataSourceFactory DataSourceFactory { get; set; }

        public StorageServiceConfig()
        {
            DataSources = new DataSourceHub();
            DataSourceFactory = new DataSourceFactory();
        }
    }
}
