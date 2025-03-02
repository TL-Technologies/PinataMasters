using Modules.Hive.Editor.BuildUtilities;
using Modules.Hive.Editor.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace Modules.Hive.Editor.Pipeline
{
    internal static class EditorPipelineBroadcaster
    {
        public static void InvokeProcessors(Type processorInterface, string methodName, object processorContext)
        {
            // Get a collection of processor types
            var processorTypes = EditorReflectionHelper.GetDefinedTypesFromUnityAssemblies()
                .Where(p => IsProcessorClass(p, processorInterface))
                .OrderByDescending(p => GetPipelineOptionsAttribute(p, methodName), new EditorPipelineOptionsComparer());

            // Enumerate all processor types and invoke method
            foreach (var processorType in processorTypes)
            {
                var method = processorType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                var processor = Activator.CreateInstance(processorType);
                method.Invoke(processor, new [] { processorContext });
            }
        }

        
        public static IEnumerable<IBuildPipeline> GetPipelineInstances()
        {
            var pipelineTypes = EditorReflectionHelper.GetDefinedTypesFromUnityAssemblies()
                .Where(p => IsProcessorClass(p, typeof(IBuildPipeline)));
            foreach (var pipelineType in pipelineTypes)
            {
                var pipeLine = (IBuildPipeline)Activator.CreateInstance(pipelineType);
                yield return pipeLine;
            }
        }
        
        
        public static void InvokeGenericProcessors(Type processorGenericInterface, string methodName, object processorContext)
        {
            Type processorContextType = processorContext.GetType();
            Type processorInterface = processorGenericInterface.MakeGenericType(processorContextType);

            // Get a collection of processor types
            var processorTypes = EditorReflectionHelper.GetDefinedTypesFromUnityAssemblies()
                .Where(p => IsGenericProcessorClass(p, processorInterface))
                .OrderByDescending(p => GetPipelineOptionsAttribute(p, null), new EditorPipelineOptionsComparer());

            // Enumerate all processor types
            foreach (var processorType in processorTypes)
            {
                var methods = processorType.ImplementedInterfaces
                    // Extract all relevant context types and sort by inheritance hierarchy
                    .Where(p => p.IsGenericType && processorInterface.IsAssignableFrom(p))
                    .Select(p => p.GenericTypeArguments[0])
                    .OrderBy(p => p, new TypeHierarchyComparer())
                    // Select method with specified context
                    .Select(p => processorType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, new[] { p }, null));

                // Create a processor instance and invoke relevant methods
                object processor = Activator.CreateInstance(processorType);
                foreach (var method in methods)
                {
                    method.Invoke(processor, new [] { processorContext });
                }
            }
        }


        private static Type GetContextInterface(object processorContext)
        {
            return processorContext.GetType().GetInterfaces()
                .OrderByDescending(p => p, new TypeHierarchyComparer())
                .First();
        }


        private static bool IsProcessorClass(Type type, Type processorInterface)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            return processorInterface.IsAssignableFrom(type);
        }


        private static bool IsGenericProcessorClass(TypeInfo type, Type processorInterface)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return false;
            }

            return type.ImplementedInterfaces
                .FirstOrDefault(p => p.IsGenericType && processorInterface.IsAssignableFrom(p)) != null;
        }


        private static EditorPipelineOptionsAttribute GetPipelineOptionsAttribute(Type processorType, string methodName)
        {
            EditorPipelineOptionsAttribute classAttribute = processorType.GetCustomAttribute<EditorPipelineOptionsAttribute>();
            EditorPipelineOptionsAttribute methodAttribute = null;

            if (!string.IsNullOrWhiteSpace(methodName))
            {
                MethodInfo method = processorType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
                methodAttribute = method.GetCustomAttribute<EditorPipelineOptionsAttribute>();
            }

            if (classAttribute != null && methodAttribute != null)
            {
                classAttribute.Override(methodAttribute);
            }

            return classAttribute ?? methodAttribute ?? new EditorPipelineOptionsAttribute();
        }
    }
}
