using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace Modules.Hive.Editor
{
    internal static class FileSystemUtilitiesExtensions
    {
        public static IEnumerable<T> WhereMatchRegex<T>(this IEnumerable<T> collection, string regex)
            where T : FileSystemInfo
        {
            return collection.Where(p => Regex.IsMatch(UnityPath.FixPathSeparator(p.FullName), regex, RegexOptions.IgnoreCase));
        }


        public static IEnumerable<T> WhereEndsWith<T>(this IEnumerable<T> collection, string value)
            where T : FileSystemInfo
        {
            return collection.Where(p => UnityPath.FixPathSeparator(p.FullName).EndsWith(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
