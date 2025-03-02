using Modules.Hive.Serialization;
using System;
using System.IO;
using UnityEngine;
using IoPath = System.IO.Path;


namespace Modules.Hive.Storage
{
    public enum DataSourceFileKind
    {
        Actual,
        Backup,
        Temp
    }


    public enum DataSourceBackupMode
    {
        Disabled = 0,
        KeepPreviousCopy = 1
    }


    public class FileSystemDataSource<T> : DataSource<T> where T : DataModel, new()
    {
        public string Path { get; }
        public StorageScope Scope { get; }
        public DataSourceBackupMode BackupMode { get; }

        public ISerializer Serializer { get; }
        public IFileSystemStorage Storage { get; }
        
        private string absolutePath = null;


        public FileSystemDataSource(
            string identifier,
            string path, 
            IFileSystemStorage storage,
            ISerializer serializer,
            StorageScope scope = StorageScope.Roaming, 
            DataSourceBackupMode backupMode = DataSourceBackupMode.KeepPreviousCopy) : base(identifier)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            // check the path. make it from base.Identifier if required
            //if (string.IsNullOrWhiteSpace(path))
            //    path = DataSourceUtils.GetPathFromIdentifier(Identifier);

            // check the scope and init an absolute path if possible
            if (IoPath.IsPathRooted(path))
            {
                scope = StorageScope.Undefined;
            }

            if (scope == StorageScope.Undefined)
            {
                absolutePath = path;
            }

            Path = path;
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            Scope = scope;
            BackupMode = backupMode;
        }


        public FileSystemDataSource(string path,
            IFileSystemStorage storage,
            ISerializer serializer,
            StorageScope scope = StorageScope.Roaming,
            DataSourceBackupMode backupMode = DataSourceBackupMode.KeepPreviousCopy) : 
            this(null, path, storage, serializer, scope, backupMode) { }


        private string GetAbsolutePath(DataSourceFileKind kind)
        {
            // Make absolute path
            if (string.IsNullOrEmpty(absolutePath))
            {
                absolutePath = IoPath.IsPathRooted(Path)
                    ? Path
                    : IoPath.Combine(Storage.GetRootPath(Scope), Path);
            }

            // Return path as is when backup mode is disabled
            if (BackupMode == DataSourceBackupMode.Disabled)
            {
                return absolutePath;
            }

            // Make path for specified replica type
            string suffix = null;
            switch (kind)
            {
                case DataSourceFileKind.Actual:
                    return absolutePath;
                case DataSourceFileKind.Backup:
                    suffix = "bak";
                    break;
                case DataSourceFileKind.Temp:
                    suffix = "tmp";
                    break;
                default:
                    throw new NotSupportedException($"File kind '{kind}' is not supported.");
            }

            return string.Format("{0}_{1}{2}",
                IoPath.Combine(
                    IoPath.GetDirectoryName(absolutePath),
                    IoPath.GetFileNameWithoutExtension(absolutePath)),
                suffix,
                IoPath.GetExtension(absolutePath));
        }


        #region Exists

        public override bool IsExists
        {
            get
            {
                switch (BackupMode)
                {
                    case DataSourceBackupMode.Disabled:
                        return Storage.FileExists(GetAbsolutePath(DataSourceFileKind.Actual));
                    case DataSourceBackupMode.KeepPreviousCopy:
                        return
                            Storage.FileExists(GetAbsolutePath(DataSourceFileKind.Actual)) ||
                            Storage.FileExists(GetAbsolutePath(DataSourceFileKind.Backup));
                    default:
                        throw new NotSupportedException($"Backup mode '{BackupMode}' is not supported.");
                }
            }
        }


        public bool Exists(DataSourceFileKind kind)
        {
            return Storage.FileExists(GetAbsolutePath(kind));
        }

        #endregion



        #region Load

        protected override object LoadDataModel()
        {
            switch (BackupMode)
            {
                case DataSourceBackupMode.Disabled:
                    return Load(GetAbsolutePath(DataSourceFileKind.Actual));
                case DataSourceBackupMode.KeepPreviousCopy:
                    return
                        Load(GetAbsolutePath(DataSourceFileKind.Actual)) ??
                        Load(GetAbsolutePath(DataSourceFileKind.Backup));
                default:
                    throw new NotSupportedException($"Backup mode '{BackupMode}' is not supported.");
            }
        }


        public T Peek(DataSourceFileKind kind)
        {
            object data = Load(GetAbsolutePath(kind));
            T dataModel = DataSourceUtilities.ConvertToDataModel<T>(data);

            if (dataModel != null)
            {
                dataModel.OnAfterLoad();
            }

            return dataModel;
        }


        private object Load(string path)
        {
            if (!Storage.FileExists(path))
            {
                return null;
            }

            ISerializer serializer = Serializer;
            Type dataType = DataModelType;
            object data = null;
            try
            {
                bool rs = false;
                using (var stream = Storage.OpenFile(path))
                    rs =
                        TryDeserializeAsStream(serializer, stream, out data, dataType) || // stream deserializer first
                        TryDeserializeAsString(serializer, stream, out data, dataType);

                if (!rs)
                {
                    Debug.LogError($"Unsupported serializer type: {serializer.GetType().FullName}");
                    return null;
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to load file '{path}'. See below for details.");
                Debug.LogException(e);
                return null;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to deserialize object from file '{path}'. See below for details.");
                Debug.LogException(e);
                return null;
            }

            return data;
        }

        #endregion



        #region Save

        protected override bool SaveDataModel(T dataModel)
        {
            switch (BackupMode)
            {
                case DataSourceBackupMode.Disabled:
                    return Save(GetAbsolutePath(DataSourceFileKind.Actual), dataModel);

                case DataSourceBackupMode.KeepPreviousCopy:
                    string cur_f = GetAbsolutePath(DataSourceFileKind.Actual);
                    string tmp_f = GetAbsolutePath(DataSourceFileKind.Temp);
                    string bak_f = GetAbsolutePath(DataSourceFileKind.Backup);

                    if (!Save(tmp_f, dataModel))
                        return false;

                    if (Storage.FileExists(cur_f))
                    {
                        Storage.DeleteFile(bak_f);
                        Storage.MoveFile(cur_f, bak_f);
                    }
                    Storage.MoveFile(tmp_f, cur_f);
                    return true;

                default:
                    throw new NotSupportedException($"Backup mode '{BackupMode}' is not supported.");
            }
        }


        bool Save(string path, object data)
        {
            // check path exists
            if (Storage is IHierarchicalFileSystemStorage hs)
            {
                string dirname = IoPath.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(dirname) && !hs.DirectoryExists(dirname))
                {
                    hs.CreateDirectory(dirname);
                }
            }

            // serialize
            ISerializer serializer = Serializer;
            try
            {
                bool result = false;
                using (var stream = Storage.CreateFile(path))
                {
                    result =
                        TrySerializeAsStream(serializer, stream, data) || // Stream serializer first
                        TrySerializeAsString(serializer, stream, data);
                }

                if (!result)
                {
                    Debug.LogError($"Unsupported serializer type: {serializer.GetType().FullName}");
                    return false;
                }
            }
            catch (IOException e)
            {
                Debug.LogError($"Failed to save file '{path}'. See below for details.");
                Debug.LogException(e);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to serialize object to file '{path}'. See below for details.");
                Debug.LogException(e);
                return false;
            }

            return true;
        }

        #endregion



        #region Delete

        public override void Delete()
        {
            switch (BackupMode)
            {
                case DataSourceBackupMode.Disabled:
                    Delete(DataSourceFileKind.Actual);
                    break;
                case DataSourceBackupMode.KeepPreviousCopy:
                    Storage.DeleteFile(GetAbsolutePath(DataSourceFileKind.Temp));
                    Storage.DeleteFile(GetAbsolutePath(DataSourceFileKind.Actual));
                    Storage.DeleteFile(GetAbsolutePath(DataSourceFileKind.Backup));
                    break;
                default:
                    throw new NotSupportedException($"Backup mode '{BackupMode}' is not supported.");
            }
        }


        public void Delete(DataSourceFileKind kind)
        {
            Storage.DeleteFile(GetAbsolutePath(kind));
        }

        #endregion
    }
}
