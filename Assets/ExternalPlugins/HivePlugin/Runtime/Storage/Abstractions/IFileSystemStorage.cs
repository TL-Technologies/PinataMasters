using System.IO;


namespace Modules.Hive.Storage
{
    public interface IFileSystemStorage
    {
        string GetRootPath(StorageScope scope);

        bool FileExists(string path);
        Stream CreateFile(string path);
        Stream OpenFile(string path);
        void DeleteFile(string path);
        void CopyFile(string source, string destination);
        void MoveFile(string source, string destination);
    }
}
