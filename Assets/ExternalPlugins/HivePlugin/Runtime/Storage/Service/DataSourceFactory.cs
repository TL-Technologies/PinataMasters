using Modules.Hive.Serialization;
using System;
using UnityEngine;


namespace Modules.Hive.Storage
{
    public class DataSourceFactory : IDataSourceFactory
    {
        public bool AllowFallbackToPlayerPrefs { get; set; } = false;


        public IDataSource<T> CreateDefaultDataSource<T>(string identifier)
            where T : DataModel, new()
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = DataSourceUtilities.GetIdentifierFromType<T>();
            }

            #if UNITY_EDITOR || UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                return CreateDefaultFileSystemDataSource<T>(identifier, $"{identifier}.dat");
            #else
                return CreateDefaultKeyValueDataSource<T>(identifier, identifier);
            #endif
        }


        public IDataSource<T> CreateDefaultKeyValueDataSource<T>(string identifier, string key)
            where T : DataModel, new()
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = DataSourceUtilities.GetIdentifierFromType<T>();
            }

            return new KeyValueDataSource<T>(
                identifier, 
                key,
                new PlayerPrefsStorage(),
                new NewtonsoftJsonSerializer());
        }


        public IDataSource<T> CreateDefaultFileSystemDataSource<T>(
            string identifier,
            string path, 
            StorageScope scope = StorageScope.Roaming)
            where T : DataModel, new()
        {
            if (string.IsNullOrEmpty(identifier))
            {
                identifier = DataSourceUtilities.GetIdentifierFromType<T>();
            }

            #if UNITY_EDITOR
                var settings = NewtonsoftJsonSerializerBase.CreateDefaultSettings();
                settings.Formatting = Newtonsoft.Json.Formatting.Indented;
    
                return new FileSystemDataSource<T>(identifier, path,
                    new CommonFileSystemStorage(),
                    new NewtonsoftJsonSerializer(settings),
                    scope);
            #elif UNITY_STANDALONE || UNITY_ANDROID || UNITY_IOS
                return new FileSystemDataSource<T>(identifier, path,
                    new CommonFileSystemStorage(),
                    new NewtonsoftBsonSerializer(),
                    scope);
            // #elif UNITY_WSA
            // return new UwpFileSystemDataSource<T>(identifier, path, scope)
            #else
                if (!AllowFallbackToPlayerPrefs)
                    throw new NotImplementedException("There are no FileSystemDataSource implementations available on this platform.");
    
                return new KeyValueDataSource<T>(identifier, path,
                    new PlayerPrefsStorage(),
                    new NewtonsoftJsonSerializer());
            #endif
        }
    }
}
