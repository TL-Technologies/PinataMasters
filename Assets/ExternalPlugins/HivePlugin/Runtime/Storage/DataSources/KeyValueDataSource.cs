using Modules.Hive.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.Hive.Storage
{
    public class KeyValueDataSource<T> : DataSource<T> where T : DataModel, new()
    {
        public string Key{ get; }

        public ISerializer Serializer { get; }
        public IKeyValueStorage Storage { get; }


        public KeyValueDataSource(string identifier, string key,
            IKeyValueStorage storage,
            ISerializer serializer) : base(identifier)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            // Check the key. make it from base.Identifier if required
            // if (string.IsNullOrWhiteSpace(key))
            //     key = DataSourceUtils.GetPathFromIdentifier(Identifier);

            Key = key;
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }


        public KeyValueDataSource(string key,
            IKeyValueStorage storage,
            ISerializer serializer) : this(null, key, storage, serializer) { }


        #region Exists

        public override bool IsExists => Storage.ContainsKey(Key);

        #endregion



        #region Load

        protected override object LoadDataModel()
        {
            return Load(Key);
        }


        public T Peek()
        {
            object data = Load(Key);
            T dataModel = DataSourceUtilities.ConvertToDataModel<T>(data);

            if (dataModel != null)
            {
                dataModel.OnAfterLoad();
            }

            return dataModel;
        }


        private object Load(string key)
        {
            ISerializer serializer = Serializer;
            Type dataType = DataModelType;
            object data = null;
            try
            {
                if (!Storage.TryGetValue(key, out string encodedString))
                {
                    return null;
                }

                bool result =
                    TryDeserializeAsString(serializer, encodedString, out data, dataType) || // string deserializer first
                    TryDeserializeAsStream(serializer, encodedString, out data, dataType);

                if (!result)
                {
                    Debug.LogError($"Unsupported serializer type: {serializer.GetType().FullName}");
                    return null;
                }
            }
            catch (ArgumentNullException e)
            {
                Debug.LogError("Failed to get data. Key is null");
                Debug.LogException(e);
                return null;
            }
            catch (KeyNotFoundException e)
            {
                Debug.LogError($"Failed to get data by key '{key}'");
                Debug.LogException(e);
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize object from data by key '{key}'. See below for details.");
                Debug.LogException(e);
                return null;
            }

            return data;
        }

        #endregion



        #region Save

        protected override bool SaveDataModel(T dataModel)
        {
            string key = Key;
            ISerializer serializer = Serializer;
            try
            {
                bool result =
                    TrySerializeAsString(serializer, out string encodedString, dataModel) || // string serializer first
                    TrySerializeAsStream(serializer, out encodedString, dataModel);

                if (result)
                {
                    Storage[key] = encodedString;
                    Storage.Save();
                }
                else
                {
                    Debug.LogError($"Unsupported serializer type: '{serializer.GetType().FullName}");
                    return false;
                }
            }
            catch (ArgumentNullException e)
            {
                Debug.LogError("Failed to set data. Key is null.");
                Debug.LogException(e);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize object for key '{key}'. See below for details.");
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        #endregion
        


        #region Delete

        public override void Delete()
        {
            Storage.Remove(Key);
        }

        #endregion
    }
}
