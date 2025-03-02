using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;


namespace Modules.Hive.Editor
{
    public static partial class FileSystemUtilities
    {
        #if UNITY_EDITOR_OSX

        private const long SymlinkBufferLength = 1024;
        
        // https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/readlink.2.html
        [DllImport("libSystem.dylib", SetLastError = true)]
        private static extern long readlink(
            [MarshalAs(UnmanagedType.LPArray)] string linkPath,
            [MarshalAs(UnmanagedType.LPArray)] byte[] buffer, 
            long bufferLen);

        // https://developer.apple.com/library/archive/documentation/System/Conceptual/ManPages_iPhoneOS/man2/symlink.2.html
        [DllImport("libSystem.dylib", SetLastError = true)]
        private static extern int symlink(
            [MarshalAs(UnmanagedType.LPArray)] string sourcePath,
            [MarshalAs(UnmanagedType.LPArray)] string linkPath);


        /// <summary>
        /// Gets whether the platform supports symbolic links
        /// </summary>
        private static bool HasSymlinksSupport => true;

        
        /// <summary>
        /// Gets a value of a symbolic link at specified path.
        /// The value is a path to target file or directory.
        /// It can be absolute or relative from the symbolic link location.
        /// </summary>
        /// <param name="path">A path to the symbolic link to read.</param>
        /// <returns>The value of the link.</returns>
        private static string ReadLink(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path should not be null or empty.");
            }

            byte[] buffer = new byte[SymlinkBufferLength];

            long result = readlink(
                path,
                buffer,
                buffer.Length);

            if (result < 0)
            {
                var errno = Marshal.GetLastWin32Error();
                var ci = new System.ComponentModel.Win32Exception();
                throw new IOException(ci.Message, errno);
            }

            return Encoding.UTF8.GetString(buffer, 0, (int)result);
        }


        /// <summary>
        /// Creates a symbolic link to a file or directory.
        /// </summary>
        /// <param name="targetPath">A path to the target file or directory.
        /// It can be absolute or relative from the symbolic link location.</param>
        /// <param name="linkPath">A path where the link will be created.</param>
        private static void CreateSymlink(string targetPath, string linkPath)
        {
            if (string.IsNullOrWhiteSpace(targetPath) || string.IsNullOrWhiteSpace(linkPath))
            {
                throw new ArgumentException("Source path and link path should not be null or empty.");
            }

            int result = symlink(targetPath, linkPath);
            if (result < 0)
            {
                var errno = Marshal.GetLastWin32Error();
                var ci = new System.ComponentModel.Win32Exception();
                throw new IOException(ci.Message, errno);
            }
        }
        #else
        
        private static bool HasSymlinksSupport => false;


        private static string ReadLink(string path)
        {
            throw new NotSupportedException("This feature not supported on this platform.");
        }
        
        
        private static void CreateSymlink(string targetPath, string linkPath)
        {
            throw new NotSupportedException("This feature not supported on this platform.");
        }
        
        #endif


        private static bool IsSymlink(string path)
        {
            return File.GetAttributes(path).HasFlag(FileAttributes.ReparsePoint);
        }
        

        private static void CopySymlink(string sourcePath, string destinationPath)
        {
            string symlinkValue = ReadLink(sourcePath);
            CreateSymlink(symlinkValue, destinationPath);
        }
        

        private static void MoveSymlink(string sourcePath, string destinationPath)
        {
            string symlinkValue = ReadLink(sourcePath);
            CreateSymlink(symlinkValue, destinationPath);
            File.Delete(sourcePath);
        }
    }
}