using Modules.Hive.Serialization;
using System;
using System.IO;


namespace Modules.Hive.Storage
{
    public abstract class DataSource<T> : IDataSource<T> where T : DataModel, new()
    {
        public string Identifier { get; }

        public T DataModel { get; protected set; }
        DataModel IDataSource.DataModel => DataModel;

        public Type DataModelType => typeof(T);

        public bool IsLoaded => DataModel != null;


        public DataSource(string identifier)
        {
            Identifier = string.IsNullOrEmpty(identifier) ? DataSourceUtilities.GetIdentifierFromType<T>() : identifier;
        }


        #region Main IO methods

        public abstract bool IsExists { get; }

        protected abstract object LoadDataModel();

        protected abstract bool SaveDataModel(T dataModel);

        public abstract void Delete();


        public void Load()
        {
            if (DataModel != null)
            {
                return;
            }

            T dataModel = DataSourceUtilities.ConvertToDataModel<T>(LoadDataModel()) ?? new T();
            dataModel.OnAfterLoad();
            dataModel.DataSource = this;
            DataModel = dataModel;
        }


        public bool Save()
        {
            if (DataModel == null)
            {
                return false;
            }

            T dataModel = DataModel;
            dataModel.OnBeforeSave();

            return SaveDataModel(dataModel);
        }


        public void Close(bool save = true)
        {
            if (DataModel == null)
            {
                return;
            }

            if (save)
            {
                Save();
            }

            DataModel = null;
        }

        #endregion



        #region Serialization

        protected static bool TrySerializeAsStream(ISerializer serializer, Stream stream, object data)
        {
            var sr = serializer as IStreamSerializer;
            if (sr == null)
            {
                return false;
            }

            sr.Serialize(stream, data);
            return true;
        }


        protected static bool TrySerializeAsString(ISerializer serializer, Stream stream, object data)
        {
            var sr = serializer as IStringSerializer;
            if (sr == null)
            {
                return false;
            }

            using (var w = new StreamWriter(stream))
            {
                string buffer = sr.Serialize(data);
                w.Write(buffer);
            }

            return true;
        }


        protected static bool TrySerializeAsStream(ISerializer serializer, out string encodedString, object data)
        {
            encodedString = null;

            var sr = serializer as IStreamSerializer;
            if (sr == null)
            {
                return false;
            }

            using (var stream = new MemoryStream())
            {
                sr.Serialize(stream, data);
                encodedString = Convert.ToBase64String(stream.ToArray());
            }
            //using (var stream = new MemoryStream())
            //using (var writer = new Base64Stream(stream, Base64StreamMode.Encode))
            //{
            //    sr.Serialize(writer, data);
            //    writer.Flush();
            //    encodedString = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            //}

            return true;
        }


        protected static bool TrySerializeAsString(ISerializer serializer, out string encodedString, object data)
        {
            encodedString = null;

            var sr = serializer as IStringSerializer;
            if (sr == null)
            {
                return false;
            }

            encodedString = sr.Serialize(data);
            return true;
        }

        #endregion



        #region Deserialization

        protected static bool TryDeserializeAsStream(ISerializer serializer, Stream stream, out object data, Type dataType)
        {
            data = null;

            var sr = serializer as IStreamSerializer;
            if (sr == null)
            {
                return false;
            }

            data = sr.Deserialize(stream, dataType);
            return true;
        }


        protected static bool TryDeserializeAsString(ISerializer serializer, Stream stream, out object data, Type dataType)
        {
            data = null;

            var sr = serializer as IStringSerializer;
            if (sr == null)
            {
                return false;
            }

            using (var r = new StreamReader(stream))
            {
                string buffer = r.ReadToEnd();
                data = sr.Deserialize(buffer, dataType);
            }

            return true;
        }


        protected static bool TryDeserializeAsStream(ISerializer serializer, string encodedString, out object data, Type dataType)
        {
            data = null;

            var sr = serializer as IStreamSerializer;
            if (sr == null)
            {
                return false;
            }

            using (var stream = new MemoryStream(Convert.FromBase64String(encodedString)))
                data = sr.Deserialize(stream, dataType);

            return true;
        }


        protected static bool TryDeserializeAsString(ISerializer serializer, string encodedString, out object data, Type dataType)
        {
            data = null;

            var sr = serializer as IStringSerializer;
            if (sr == null)
            {
                return false;
            }

            data = sr.Deserialize(encodedString, dataType);
            return true;
        }

        #endregion
    }
}
