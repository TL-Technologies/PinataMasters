using System.IO;


namespace Modules.Hive.Storage
{
    public class CommonFileSystemStorage : IHierarchicalFileSystemStorage
    {
        public string GetRootPath(StorageScope scope)
        {
            // This storage uses real FS path that provided by platform-depended StorageManager
            return AppHost.Instance.StorageService.GetScopeLocation(scope);
        }

        
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }
        

        public Stream CreateFile(string path)
        {
            return File.Create(path);
        }
        

        public Stream OpenFile(string path)
        {
            return File.OpenRead(path);
        }
        

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }
        

        public void CopyFile(string source, string destination)
        {
            File.Copy(source, destination);
        }
        

        public void MoveFile(string source, string destination)
        {
            File.Move(source, destination);
        }
        

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }
        

        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }
        

        public void DeleteDirectory(string path)
        {
            Directory.Delete(path, true);
        }
    }
}
