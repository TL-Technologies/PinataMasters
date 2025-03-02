using ICSharpCode.SharpZipLib.Tar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using UnityEngine;


namespace Modules.Hive.Editor
{
    public static partial class FileSystemUtilities
    {
        #region Copy, move, delete

        /// <summary>
        /// Copies a file or a directory from <paramref name="sourcePath"/> path to <paramref name="destinationPath"/> path.
        /// </summary>
        /// <param name="sourcePath">A source path.</param>
        /// <param name="destinationPath">A destination path.</param>
        /// <param name="options">An options of the operation.</param>
        public static void Copy(string sourcePath, string destinationPath, FileSystemOperationOptions options = FileSystemOperationOptions.Default)
        {
            // It's always true due the symlinks is not supported yet
            const bool traverseSymlinks = true;

            FileSystemEntry entry = FileSystemEntry.Create(sourcePath);

            if (!entry.Exists)
            {
                return;
            }

            CreateOutputDirectory(destinationPath);
            ShallowCopyFileSystemEntry(entry, destinationPath, options);
            CopyMetaFile(entry.Path, destinationPath, options);

            if (entry.CanStepInto(traverseSymlinks))
            {
                foreach (FileSystemEntry innerEntry in EnumerateFileSystemEntries(sourcePath, traverseSymlinks))
                {
                    string dstPath = UnityPath.Combine(destinationPath, innerEntry.Path.Substring(sourcePath.Length));
                    ShallowCopyFileSystemEntry(innerEntry, dstPath, options);
                }
            }
        }


        /// <summary>
        /// Moves a file or a directory from <paramref name="sourcePath"/> path to <paramref name="destinationPath"/> path.
        /// </summary>
        /// <param name="sourcePath">A source path.</param>
        /// <param name="destinationPath">A destination path.</param>
        /// <param name="options">An options of the operation.</param>
        public static void Move(string sourcePath, string destinationPath, FileSystemOperationOptions options = FileSystemOperationOptions.Default)
        {
            // It's always true due the symlinks is not supported yet
            const bool traverseSymlinks = true;

            FileSystemEntry entry = FileSystemEntry.Create(sourcePath);

            if (!entry.Exists)
            {
                return;
            }

            CreateOutputDirectory(destinationPath);
            ShallowMoveFileSystemEntry(entry, destinationPath, options);
            MoveMetaFile(entry.Path, destinationPath, options);

            if (entry.CanStepInto(traverseSymlinks))
            {
                foreach (FileSystemEntry innerEntry in EnumerateFileSystemEntries(sourcePath, traverseSymlinks))
                {
                    string dstPath = UnityPath.Combine(destinationPath, innerEntry.Path.Substring(sourcePath.Length));
                    ShallowMoveFileSystemEntry(innerEntry, dstPath, options);
                }
            }
            entry.Delete();
        }


        /// <summary>
        /// Deletes a file or a directory from <paramref name="path"/>.
        /// </summary>
        /// <param name="path">An entry path.</param>
        /// <param name="options">An options of the operation.</param>
        public static void Delete(string path, FileSystemOperationOptions options = FileSystemOperationOptions.None)
        {
            FileSystemEntry entry = FileSystemEntry.Create(path);
            if (!entry.Exists)
            {
                return;
            }

            DeleteMetaFile(entry.Path, options);
            entry.Delete();
        }


        /// <summary>
        /// Deletes entry and every empty directory in an ascending hierarchy.
        /// </summary>
        /// <param name="entryPath">The path to the entry.</param>
        public static void DeleteEntryAndEmptyParentsDirectories(string entryPath)
        {
            if (!Directory.Exists(entryPath) && !File.Exists(entryPath))
            {
                return;
            }

            string currentEntryPath = entryPath;
            do
            {
                string parentDirectoryPath = UnityPath.GetDirectoryName(currentEntryPath);
                Delete(currentEntryPath);
                currentEntryPath = parentDirectoryPath;
            } while (IsDirectoryEmpty(currentEntryPath));
        }


        /// <summary>
        /// Copy <paramref name="sourceDirectoryPath"/> directory content (files and directories) to <paramref name="destinationDirectoryPath"/> directory.
        /// </summary>
        /// <param name="sourceDirectoryPath">A source path.</param>
        /// <param name="destinationDirectoryPath">A destination path.</param>
        /// <param name="options">An options of the operation.</param>
        public static void CopyDirectoryContent(
            string sourceDirectoryPath,
            string destinationDirectoryPath,
            FileSystemOperationOptions options = FileSystemOperationOptions.Default)
        {
            FileSystemEntry entry = FileSystemEntry.Create(sourceDirectoryPath);

            if (!entry.Exists)
            {
                return;
            }
            if (!entry.IsDirectory)
            {
                UnityEngine.Debug.LogWarning(sourceDirectoryPath + " is not a directory!");
                return;
            }
            if (!Directory.Exists(destinationDirectoryPath))
            {
                Directory.CreateDirectory(destinationDirectoryPath);
            }

            foreach (FileInfo fileInfo in EnumerateFiles(sourceDirectoryPath, SearchOption.TopDirectoryOnly))
            {
                Copy(fileInfo.FullName, UnityPath.Combine(destinationDirectoryPath, fileInfo.Name), options);
            }

            foreach (DirectoryInfo directoryInfo in EnumerateDirectories(sourceDirectoryPath, SearchOption.TopDirectoryOnly))
            {
                Copy(directoryInfo.FullName, UnityPath.Combine(destinationDirectoryPath, directoryInfo.Name), options);
            }
        }


        private static void CreateOutputDirectory(string path)
        {
            path = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        private static void ShallowCopyFileSystemEntry(FileSystemEntry entry, string destinationPath, FileSystemOperationOptions options)
        {
            if (entry.IsSymlink)
            {
                UnityEngine.Debug.LogWarning($"The file or directory at path '{entry.Path}' is a symbolic link. Copying symbolic links is currently not supported. A deep copy will be made instead of that.");
            }

            if (entry.IsDirectory)
            {
                Directory.CreateDirectory(destinationPath);
                return;
            }

            // Skip meta-files if options has IgnoreMetaFiles flag.
            if ((options & FileSystemOperationOptions.IgnoreMetaFiles) != 0 &&
                entry.Path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            File.Copy(entry.Path, destinationPath, (options & FileSystemOperationOptions.Override) != 0);
        }


        private static void ShallowMoveFileSystemEntry(FileSystemEntry entry, string destinationPath, FileSystemOperationOptions options)
        {
            if (entry.IsSymlink)
            {
                UnityEngine.Debug.LogWarning($"The file or directory at path '{entry.Path}' is a symbolic link. Copying symbolic links is currently not supported. A deep copy will be made instead of that.");
            }

            if (entry.IsDirectory)
            {
                Directory.CreateDirectory(destinationPath);
                return;
            }

            // Skip meta-files if options has IgnoreMetaFiles flag.
            if ((options & FileSystemOperationOptions.IgnoreMetaFiles) != 0 &&
                entry.Path.EndsWith(".meta", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            File.Copy(entry.Path, destinationPath, (options & FileSystemOperationOptions.Override) != 0);
        }


        private static void CopyMetaFile(string sourcePath, string destinationPath, FileSystemOperationOptions options)
        {
            // Skip meta-files if options has IgnoreMetaFiles flag.
            if ((options & FileSystemOperationOptions.IgnoreMetaFiles) != 0)
            {
                return;
            }

            string sourceMeta = $"{sourcePath}.meta";
            string destinationMeta = $"{destinationPath}.meta";
            if (File.Exists(sourceMeta))
            {
                File.Copy(sourceMeta, destinationMeta, (options & FileSystemOperationOptions.Override) != 0);
            }
        }


        private static void MoveMetaFile(string sourcePath, string destinationPath, FileSystemOperationOptions options)
        {
            string sourceMeta = $"{sourcePath}.meta";
            string destinationMeta = $"{destinationPath}.meta";

            // Skip meta-files if options has IgnoreMetaFiles flag.
            if ((options & FileSystemOperationOptions.IgnoreMetaFiles) != 0)
            {
                if (File.Exists(sourceMeta))
                {
                    File.Delete(sourceMeta);
                }

                return;
            }

            if (!File.Exists(sourceMeta))
            {
                return;
            }

            if ((options & FileSystemOperationOptions.Override) != 0 && File.Exists(destinationMeta))
            {
                File.Delete(destinationMeta);
            }

            File.Move(sourceMeta, destinationMeta);
        }


        private static void DeleteMetaFile(string path, FileSystemOperationOptions options)
        {
            // Skip meta-files if options has IgnoreMetaFiles flag.
            if ((options & FileSystemOperationOptions.IgnoreMetaFiles) != 0)
            {
                return;
            }

            string pathMeta = $"{path}.meta";
            if (File.Exists(pathMeta))
            {
                File.Delete(pathMeta);
            }
        }


        private static IEnumerable<FileSystemEntry> EnumerateFileSystemEntries(string directoryPath, bool traverseSymlinks = true)
        {
            // Traverse directory or symlink to directory (this approach avoids recursion in code)
            Stack<string> directoryStack = new Stack<string>(new[] { directoryPath });
            while (directoryStack.Count > 0)
            {
                foreach (string entryPath in Directory.EnumerateFileSystemEntries(directoryStack.Pop()))
                {
                    FileSystemEntry entry = FileSystemEntry.Create(entryPath);
                    yield return entry;

                    if (entry.IsDirectory && (traverseSymlinks || !entry.IsSymlink) && entry.Exists)
                    {
                        directoryStack.Push(entryPath);
                    }
                }
            }
        }

        #endregion



        #region Compression

        /// <summary>
        /// Extracts an archive to specified path.
        /// It supports .zip on Windows and .zip, .tag.gz on *nix-based OS.
        /// </summary>
        /// <param name="pathToArchive">A path to archive.</param>
        /// <param name="outputPath">A path to extract to.</param>
        /// <param name="isClearOutputPath">Should output path be cleared.</param>
        public static void ExtractArchive(string pathToArchive, string outputPath, bool isClearOutputPath = true)
        {
            if (!File.Exists(pathToArchive))
            {
                throw new FileNotFoundException($"File not found: {pathToArchive}.");
            }

            string archiveExtension = pathToArchive.Split('.').LastOrDefault()?.ToLower();

            #if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                // *nix-based os
                // Using a command line tools to support symlinks
                string command;
                if (archiveExtension == "zip")
                {
                    command = $"unzip -o \"{pathToArchive}\" -d \"{outputPath}\"";
                }
                else if (archiveExtension == "tar.gz" || archiveExtension == "tgz")
                {
                    command = $"tar -xvf \"{pathToArchive}\" -C \"{outputPath}\"";
                }
                else
                {
                    throw new NotSupportedException($"Unsupported archive type {pathToArchive}");
                }

                if (isClearOutputPath)
                {
                    ClearOutputPath(outputPath);
                }
                else if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
                ExecuteCommandAndWait(command, true);
            #else
                if (archiveExtension != "zip" & archiveExtension != "tgz" & archiveExtension != "tar.gz")
                {
                    throw new NotSupportedException($"Unsupported archive type {pathToArchive}");
                }

                if (isClearOutputPath)
                {
                    ClearOutputPath(outputPath);
                }

                if (archiveExtension == "zip")
                {
                    using (var zip = Ionic.Zip.ZipFile.Read(pathToArchive))
                    {
                        zip.ExtractAll(outputPath, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
                    }
                }
                else if (archiveExtension == "tgz" || archiveExtension == "tar.gz")
                {
                    using (var stream = File.OpenRead(pathToArchive))
                    {
                        using (var tarFile = new GZipStream(stream, CompressionMode.Decompress))
                        {
                            using (var tarArchive = TarArchive.CreateInputTarArchive(tarFile, Encoding.UTF8))
                            {
                                tarArchive.ExtractContents(outputPath);
                            }
                        }
                    }
                }
            #endif
        }


        public static void CreateArchive(string inputPath, string pathToArchive, int relativePathDepth = 1)
        {
            if (!File.Exists(inputPath) && !Directory.Exists(inputPath))
            {
                throw new FileNotFoundException($"File or directory not found: {inputPath}.");
            }

            string archiveExtension = pathToArchive.Split('.').LastOrDefault()?.ToLower();

            if (archiveExtension != "zip")
            {
                throw new NotSupportedException($"Unsupported archive type {pathToArchive}");
            }

            if (File.Exists(pathToArchive))
            {
                File.Delete(pathToArchive);
            }
            // Zip command throws exception, if archive directory doesn't exist
            string pathToArchiveDirectory = Path.GetDirectoryName(pathToArchive);
            if (!string.IsNullOrEmpty(pathToArchiveDirectory) && !Directory.Exists(pathToArchiveDirectory))
            {
                Directory.CreateDirectory(pathToArchiveDirectory);
            }

            string pushDirectoryPath = inputPath;
            for (int i = 0; i < relativePathDepth && !string.IsNullOrEmpty(pushDirectoryPath); i++)
            {
                pushDirectoryPath = Path.GetDirectoryName(pushDirectoryPath);
            }

            #if UNITY_EDITOR_OSX
                inputPath = UnityPath.RemoveStart(inputPath, pushDirectoryPath);
                ExecuteCommandAndWait($"cd \"{pushDirectoryPath}\" && zip -rX \"{pathToArchive}\" \"./{inputPath}\"", true);
            #else
                using (var zip = new Ionic.Zip.ZipFile())
                {
                    if (Directory.Exists(inputPath))
                    {
                        zip.AddDirectory(inputPath, UnityPath.RemoveStart(inputPath, pushDirectoryPath));
                    }
                    else if (File.Exists(inputPath))
                    {
                        zip.AddFile(inputPath, UnityPath.RemoveStart(inputPath, pushDirectoryPath));
                    }
                    zip.Save(pathToArchive);
                }
            #endif
        }


        private static void ClearOutputPath(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }

            Directory.CreateDirectory(path);
        }


        private static int ExecuteCommandAndWait(string command, bool throwExceptionIfFails)
        {
            // Configure and run process
            Process process = new Process();
            process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "/bin/bash";
            process.StartInfo.Arguments = $"-c ' {command} '";
            process.Start();
            process.WaitForExit();

            // Check result
            int exitCode = process.ExitCode;
            if (exitCode != 0 && throwExceptionIfFails)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("An error occurred while executing the following command:");
                sb.AppendLine($"{command}");
                sb.AppendLine($"ExitCode = {exitCode}");
                sb.Append(process.StandardError.ReadToEnd());
                Exception e = new Exception(sb.ToString());
                throw e;
            }

            return exitCode;
        }

        #endregion



        #region Find

        /// <summary>
        /// Returns an enumerable collection of file information in specified path
        /// that matches a specified search pattern and search subdirectory option.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but it doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>
        /// An enumerable collection of file information in <paramref name="path"/> 
        /// that matches <paramref name="searchPattern"/> and <paramref name="searchOption"/>.
        /// </returns>
        public static IEnumerable<FileInfo> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(path);
            return rootDirectoryInfo.EnumerateFiles(searchPattern, searchOption);
        }


        /// <summary>
        /// Returns an enumerable collection of file information in specified path, 
        /// using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>An enumerable collection of file information.</returns>
        public static IEnumerable<FileInfo> EnumerateFiles(string path, SearchOption searchOption)
        {
            return EnumerateFiles(path, "*", searchOption);
        }


        /// <summary>
        /// Returns an enumerable collection of file information in the project
        /// that matches a specified search pattern.
        /// </summary>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but it doesn't support regular expressions.
        /// </param>
        /// <returns>An enumerable collection of file information.</returns>
        public static IEnumerable<FileInfo> EnumerateFiles(string searchPattern = null)
        {
            return EnumerateFiles(Application.dataPath, searchPattern ?? "*", SearchOption.AllDirectories);
        }


        /// <summary>
        /// Returns an absolute path to any file that ends with the specified string <paramref name="value"/>
        /// and located in specified path <paramref name="path"/>. Also, it returns null if the requested file is not found.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="value">The string that should be at the end of the file path.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>Absolute path to the file that matches conditions or null.</returns>
        public static string FindFileEndsWith(string path, string value, SearchOption searchOption)
        {
            var entry = EnumerateFiles(path, searchOption)
                .WhereEndsWith(value)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any file that ends with the specified string <paramref name="value"/>
        /// and located in the project.
        /// Also, it returns null if the requested file is not found.
        /// </summary>
        /// <param name="value">The string that should be at the end of the file path.</param>
        /// <returns>Absolute path to the file that matches conditions or null.</returns>
        public static string FindFileEndsWith(string value)
        {
            var entry = EnumerateFiles()
                .WhereEndsWith(value)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any file that matches the regular expression <paramref name="regex"/>
        /// and located in specified path <paramref name="path"/>. Also, it returns null if the requested file is not found.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>Absolute path to the file that matches conditions or null.</returns>
        public static string FindFileMatchRegex(string path, string regex, SearchOption searchOption)
        {
            var entry = EnumerateFiles(path, searchOption)
                .WhereMatchRegex(regex)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any file that matches the regular expression <paramref name="regex"/>
        /// and located in the project. Also, it returns null if the requested file is not found.
        /// </summary>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <returns>Absolute path to the file that matches conditions or null.</returns>
        public static string FindFileMatchRegex(string regex)
        {
            var entry = EnumerateFiles()
                .WhereMatchRegex(regex)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an enumerable collection of directory information in specified path
        /// that matches a specified search pattern and search subdirectory option.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but it doesn't support regular expressions.
        /// </param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>
        /// An enumerable collection of directory information in <paramref name="path"/> 
        /// that matches <paramref name="searchPattern"/> and <paramref name="searchOption"/>.
        /// </returns>
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            DirectoryInfo rootDirectoryInfo = new DirectoryInfo(path);
            return rootDirectoryInfo.EnumerateDirectories(searchPattern, searchOption);
        }


        /// <summary>
        /// Returns an enumerable collection of directory information in specified path, 
        /// using a value to determine whether to search subdirectories.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>An enumerable collection of directory information.</returns>
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(string path, SearchOption searchOption)
        {
            return EnumerateDirectories(path, "*", searchOption);
        }


        /// <summary>
        /// Returns an enumerable collection of directory information in the project
        /// that matches a specified search pattern.
        /// </summary>
        /// <param name="searchPattern">
        /// The search string to match against the names of files.
        /// This parameter can contain a combination of valid literal path and wildcard (* and ?) characters, 
        /// but it doesn't support regular expressions.
        /// </param>
        /// <returns>An enumerable collection of directory information.</returns>
        public static IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern = null)
        {
            return EnumerateDirectories(Application.dataPath, searchPattern ?? "*", SearchOption.AllDirectories);
        }


        /// <summary>
        /// Returns an absolute path to any directory that ends with the specified string <paramref name="value"/>
        /// and located in specified path <paramref name="path"/>.
        /// Also, it returns null if the requested directory is not found.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="value">The string that should be at the end of the directory path.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>Absolute path to the directory that matches conditions or null.</returns>
        public static string FindDirectoryEndsWith(string path, string value, SearchOption searchOption)
        {
            var entry = EnumerateDirectories(path, searchOption)
                .WhereEndsWith(value)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any directory that ends with the specified string <paramref name="value"/>
        /// and located in the project.
        /// Also, it returns null if the requested directory is not found.
        /// </summary>
        /// <param name="value">The string that should be at the end of the directory path.</param>
        /// <returns>Absolute path to the directory that matches conditions or null.</returns>
        public static string FindDirectoryEndsWith(string value)
        {
            var entry = EnumerateDirectories()
                .WhereEndsWith(value)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any directory that matches the regular expression <paramref name="regex"/>
        /// and located in specified path <paramref name="path"/>.
        /// Also, it returns null if the requested directory is not found.
        /// </summary>
        /// <param name="path">The path to the directory to search.</param>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <param name="searchOption">
        /// One of the enumeration values that specifies whether the search operation 
        /// should include only the current directory or all subdirectories.
        /// </param>
        /// <returns>Absolute path to the directory that matches conditions or null.</returns>
        public static string FindDirectoryMatchRegex(string path, string regex, SearchOption searchOption)
        {
            var entry = EnumerateDirectories(path, searchOption)
                .WhereMatchRegex(regex)
                .FirstOrDefault();

            return entry?.FullName;
        }


        /// <summary>
        /// Returns an absolute path to any directory that matches the regular expression <paramref name="regex"/>
        /// and located in the project. Also, it returns null if the requested directory is not found.
        /// </summary>
        /// <param name="regex">The regular expression pattern to match.</param>
        /// <returns>Absolute path to the directory that matches conditions or null.</returns>
        public static string FindDirectoryMatchRegex(string regex)
        {
            var entry = EnumerateDirectories()
                .WhereMatchRegex(regex)
                .FirstOrDefault();

            return entry?.FullName;
        }

        #endregion



        #region Other methods

        /// <summary>
        /// Replaces characters in <c>text</c> that are not allowed in file names with the specified replacement character.
        /// </summary>
        /// <param name="text">Text to make into a valid filename. The same string is returned if it is valid already.</param>
        /// <param name="replacement">Replacement character to simply remove bad characters.</param>
        /// <param name="invalidChars">Array of characters that should be replaced. Null means a default set of invalid chars.</param>
        /// <returns>A string that can be used as a filename. If the output string would otherwise be empty, returns "_".</returns>
        public static string GetValidFileName(string text, char replacement = '_', char[] invalidChars = null)
        {
            if (invalidChars == null)
            {
                invalidChars = Path.GetInvalidFileNameChars();
            }

            StringBuilder sb = new StringBuilder(text.Length);
            bool changed = false;
            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                if (invalidChars.Contains(c))
                {
                    changed = true;
                    sb.Append(replacement);
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length == 0)
            {
                return "_";
            }

            return changed ? sb.ToString() : text;
        }


        /// <summary>
        /// Checks whether directory specified in <paramref name="path"/> is empty.
        /// Returns true if directory doesn't exist.
        /// </summary>
        /// <param name="path">The path to the directory to check.</param>
        /// <returns>True if directory is empty or doesn't exist. False otherwise.</returns>
        public static bool IsDirectoryEmpty(string path)
        {
            if (!Directory.Exists(path))
            {
                return true;
            }

            return !Directory.EnumerateFileSystemEntries(path).Any();
        }

        #endregion
    }
}
