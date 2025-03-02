using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


namespace Modules.Hive.Editor.Reflection
{
    public static class EditorReflectionHelper
    {
        #region Assemmblies

        public static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }


        public static Assembly[] GetUnityAssemblies()
        {
            var unityAssemblyNames = GetUnityAssemblyNames();
            return GetAssemblies()
                .Where(p => IsUnityAssembly(p, unityAssemblyNames))
                .ToArray();
        }


        public static bool IsUnityAssembly(Assembly assembly)
        {
            var unityAssemblyNames = GetUnityAssemblyNames();
            return IsUnityAssembly(assembly, unityAssemblyNames);
        }


        private static IReadOnlyList<string> GetUnityAssemblyNames()
        {
            List<string> result = new List<string>(128);

            // All script assemblies compiled by Unity 
            foreach (var assembly in UnityEditor.Compilation.CompilationPipeline.GetAssemblies())
            {
                result.Add(assembly.name);
            }

            // Precompiled assemblies in project
            foreach (var name in UnityEditor.Compilation.CompilationPipeline.GetPrecompiledAssemblyNames())
            {
                result.Add(name.EndsWith(".dll") ? 
                    name.Remove(name.Length - 4) : 
                    name);
            }

            return result;
        }


        private static bool IsUnityAssembly(Assembly assembly, IReadOnlyList<string> unityAssemblyNames)
        {
            string assemblyName = assembly.GetName().Name;
            foreach (var unityAssemblyName in unityAssemblyNames)
            {
                if (string.Equals(unityAssemblyName, assemblyName, StringComparison.Ordinal))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion



        #region Types

        public static IEnumerable<TypeInfo> GetDefinedTypes(Assembly[] assemblies = null)
        {
            if (assemblies == null)
            {
                assemblies = GetAssemblies();
            }

            return assemblies.SelectMany(p => p.DefinedTypes);
        }


        public static IEnumerable<TypeInfo> GetDefinedTypesFromUnityAssemblies()
        {
            return GetUnityAssemblies()
                .SelectMany(p => p.DefinedTypes);
        }


        public static Type GetType(string typeName, Assembly[] assemblies = null)
        {
            return GetDefinedTypes(assemblies)
                .FirstOrDefault(t => string.Equals(typeName, t.Name, StringComparison.Ordinal) ||
                                    string.Equals(typeName, t.FullName, StringComparison.Ordinal));
        }

        #endregion
    }
}
