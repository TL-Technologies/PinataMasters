using System.IO;


namespace Modules.Hive.Editor
{
    public static partial class FileSystemUtilities
    {
        private struct FileSystemEntry
        {
            public string Path { get; private set; }

            public FileAttributes Attributes { get; private set; }

            public bool IsSymlink => (Attributes & FileAttributes.ReparsePoint) != 0;

            public bool IsDirectory => (Attributes & FileAttributes.Directory) != 0;

            public bool Exists => IsDirectory ? Directory.Exists(Path) : File.Exists(Path);


            public static FileSystemEntry Create(string path)
            {
                FileAttributes attributes = 0;

                try
                {
                    attributes = File.GetAttributes(path);
                }
                catch
                {
                    // Ignored
                }

                return new FileSystemEntry
                {
                    Path = path,
                    Attributes = attributes
                };
            }


            public bool CanStepInto(bool traverseSymlinks)
            {
                return IsDirectory && (traverseSymlinks || !IsSymlink);
            }


            public void Delete()
            {
                if (!Exists)
                {
                    return;
                }

                if (IsDirectory && !IsSymlink)
                {
                    Directory.Delete(Path, true);
                }
                else
                {
                    File.Delete(Path);
                }
            }
        }
    }
}
