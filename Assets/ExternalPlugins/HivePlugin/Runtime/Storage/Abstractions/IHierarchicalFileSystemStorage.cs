namespace Modules.Hive.Storage
{
    public interface IHierarchicalFileSystemStorage : IFileSystemStorage
    {
        bool DirectoryExists(string path);
        void CreateDirectory(string path);
        void DeleteDirectory(string path);
    }
}
