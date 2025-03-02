namespace Modules.Hive.Storage
{
   public interface IStorageService
    {
        IDataSourceHub DataSources { get; }

        IDataSourceFactory DataSourceFactory { get; }

        string GetScopeLocation(StorageScope scope);
        
        //void Clear();
    }
}
