namespace Modules.Hive.Storage
{
    public abstract class DataModelSingleton<T> : DataModel where T : DataModelSingleton<T>, new()
    {
        private static T instance = null;
        

        public static T Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                string identifier = DataSourceUtilities.GetIdentifierFromType<T>();
                IStorageService storageService = AppHost.Instance.StorageService;
                IDataSource<T> dataSource = storageService.DataSources.GetDataSource<T>(identifier);
                if (dataSource == null)
                {
                    dataSource = storageService.DataSourceFactory.CreateDefaultDataSource<T>(identifier);
                    storageService.DataSources.Add(dataSource);
                }

                dataSource.Load();
                instance = dataSource.DataModel;

                return instance;
            }
        }


        public override void Close()
        {
            if (DataSource != null)
            {
                AppHost.Instance.StorageService.DataSources.Remove(DataSource);
            }

            base.Close();

            instance = null;
        }
    }
}
