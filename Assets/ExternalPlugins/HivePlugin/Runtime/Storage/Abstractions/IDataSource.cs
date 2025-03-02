using System;


namespace Modules.Hive.Storage
{
    public interface IDataSource
    {
        string Identifier { get; }

        DataModel DataModel { get; }

        Type DataModelType { get; }

        bool IsLoaded { get; }

        bool IsExists { get; }

        void Load();

        bool Save();

        void Delete();

        void Close(bool save = true);
    }

    
    public interface IDataSource<out T> : IDataSource where T : DataModel
    {
        new T DataModel { get; }
    }
}
