using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using static Modules.General.InitializationQueue.InitializationQueueConfiguration;


namespace Modules.General.InitializationQueue
{
    public class InitializationQueueConfigurator : EditorWindow
    {
        #region Nested types

        private class ServiceClassEntry
        {
            public Type bindTo = null;
            public Type[] dependencies = null;
            public bool isUsedInConfiguration = false;
            public int order = 0;
            public ScriptableObject serviceAsset = null;
            public GameObject servicePrefab = null;
            public Type type = null;
            public string typeName = null;
        }

        #endregion



        #region Fields

        private static readonly List<ServiceClassEntry> ServiceTypes = new List<ServiceClassEntry>();
        private Vector2 scrollPosition;

        #endregion



        #region Unity lifecycle

        private void OnFocus()
        {
            CollectAllPlugins();
        }


        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            {
                EditorGUI.BeginChangeCheck();

                for (int i = 0; i < ServiceTypes.Count; ++i)
                {
                    ServiceClassEntry item = ServiceTypes.ElementAt(i);
                    bool oldValue = item.isUsedInConfiguration;

                    AddServiceEditor(item);

                    if (item.isUsedInConfiguration != oldValue)
                    {
                        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

                        if (item.isUsedInConfiguration)
                        {
                            if (DoesImplementationExist(item, out string serviceList))
                            {
                                EditorUtility.DisplayDialog(
                                    "ERROR!",
                                    $"The service implementation {item.typeName} exists in:\n{serviceList}",
                                    "OK");

                                continue;
                            }

                            if (item.dependencies != null && item.dependencies.Length > 0)
                            {
                                if (!DoesHaveAllDependencies(item, out string dependencyList))
                                {
                                    EditorUtility.DisplayDialog(
                                        "ERROR!",
                                        $"The {item.typeName} service has no implemented dependencies:\n{dependencyList}",
                                        "OK");

                                    continue;
                                }

                                var allDependencies = ServiceTypes.Where(
                                        serviceType => item.dependencies.Any(dependency => serviceType.bindTo == dependency));

                                if (DoesHaveMultipleDependenciesImplementation(item, allDependencies, out string dependencyMultipleList))
                                {
                                    EditorUtility.DisplayDialog(
                                        "WARNING!",
                                        $"The {item.typeName} service has multiple implementation dependencies.\n" +
                                        $"Select the ones you need manually.\n{dependencyMultipleList}",
                                        "OK");

                                    continue;
                                }
                                else
                                {
                                    SetInUseDependencies(item, allDependencies);
                                }
                            }

                            InitializationQueueConfiguration.Instance.AddBinding(item.type, item.servicePrefab,
                                    item.serviceAsset);

                            FieldInfo[] fields = item.type.GetFields(flags);
                            foreach (var field in fields)
                            {
                                Attribute attribute = Attribute.GetCustomAttribute(field, typeof(RequiredAbData));
                                if (attribute != null)
                                {
                                    RequiredAbDataStorage.Instance.AddTestData(field.FieldType);
                                }
                            }
                        }
                        else
                        {
                            if (DoesServiceInUse(item, out string services))
                            {
                                EditorUtility.DisplayDialog("ERROR!",
                                    $"The {item.typeName} service is used in:\n{services}",
                                    "OK");

                                continue;
                            }

                            InitializationQueueConfiguration.Instance.RemoveBinding(item.type);

                            FieldInfo[] fields = item.type.GetFields(flags);
                            foreach (var field in fields)
                            {
                                Attribute attribute = Attribute.GetCustomAttribute(field, typeof(RequiredAbData));
                                if (attribute != null)
                                {
                                    RequiredAbDataStorage.Instance.RemoveTestData(field.FieldType);
                                }
                            }
                        }
                    }
                }

                if (EditorGUI.EndChangeCheck())
                {
                    // Update associated prefabs
                    foreach (var entry in ServiceTypes)
                    {
                        if (entry.isUsedInConfiguration)
                        {
                            InitializationQueueConfiguration.Instance.AddBinding(entry.type, entry.servicePrefab,
                                entry.serviceAsset);
                        }
                    }

                    EditorUtility.SetDirty(InitializationQueueConfiguration.Instance);
                    EditorUtility.SetDirty(RequiredAbDataStorage.Instance);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        #endregion



        #region Methods

        [MenuItem("Modules/Initialization Queue")]
        public static void Init()
        {
            EditorWindow window = GetWindow<InitializationQueueConfigurator>();
            window.Show();

            CollectAllPlugins();
        }


        private static void CollectAllPlugins()
        {
            ServiceTypes.Clear();

            var allTypes =
                AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(wh => !wh.IsGenericType &&
                                 !wh.IsAbstract &&
                                 !wh.FullName.Contains("Modules.General.InitializationQueue.Sample"));

            var allIQServices = GetTypesWithInitQueuServiceAttribute(allTypes);

            foreach (var serviceType in allIQServices)
            {
                ServiceBinding storedEntry = InitializationQueueConfiguration.Instance.GetBinding(serviceType);
                InitQueueServiceAttribute iqServiceAttribute = serviceType.GetCustomAttribute<InitQueueServiceAttribute>();

                ServiceTypes.Add(new ServiceClassEntry
                {
                    type = serviceType,
                    typeName = serviceType.FullName,
                    isUsedInConfiguration = storedEntry != null,
                    bindTo = iqServiceAttribute.BindTo,
                    dependencies = iqServiceAttribute.Dependencies,
                    order = iqServiceAttribute.Order,
                    servicePrefab = storedEntry?.servicePrefab,
                    serviceAsset = storedEntry?.serviceAsset
                });
            }

            ServiceTypes.Sort((x, y) => { return x.order.CompareTo(y.order); });

            InitializationQueueConfiguration.Instance.ServiceBindings.RemoveAll(sb =>
                ServiceTypes.Find(st => st.type.AssemblyQualifiedName == sb.serviceClass) == null);
        }


        private static IEnumerable<Type> GetTypesWithInitQueuServiceAttribute(IEnumerable<Type> types)
        {
            Type iqServiceAttributeType = typeof(InitQueueServiceAttribute);

            foreach (Type type in types)
            {
                if (Attribute.IsDefined(type, iqServiceAttributeType))
                {
                    yield return type;
                }
            }
        }


        private void AddServiceEditor(ServiceClassEntry serviceEntry)
        {
            EditorGUILayout.BeginHorizontal();
            {
                serviceEntry.isUsedInConfiguration =
                    EditorGUILayout.ToggleLeft(serviceEntry.typeName, serviceEntry.isUsedInConfiguration);

                if (serviceEntry.type.IsSubclassOf(typeof(MonoBehaviour)))
                {
                    serviceEntry.servicePrefab = (GameObject)
                        EditorGUILayout.ObjectField(serviceEntry.servicePrefab, typeof(GameObject), false);
                }

                if (serviceEntry.type.IsSubclassOf(typeof(ScriptableObject)))
                {
                    serviceEntry.serviceAsset = (ScriptableObject)
                        EditorGUILayout.ObjectField(serviceEntry.serviceAsset, typeof(ScriptableObject), false);
                }
            }

            EditorGUILayout.EndHorizontal();
        }


        private bool DoesHaveAllDependencies(ServiceClassEntry serviceEntry, out string services)
        {
            services = string.Empty;

            var nonInitializationDependencies =
                serviceEntry.dependencies.Where(dependency => !ServiceTypes.Any(serviceType => serviceType.bindTo == dependency))
                                         .ToList();

            if (nonInitializationDependencies.Count == 0)
            {
                return true;
            }

            StringBuilder str = new StringBuilder();
            nonInitializationDependencies.ForEach(fe => str.Append(fe.Name + ", "));

            services = str.ToString().TrimEnd(' ', ',');
            serviceEntry.isUsedInConfiguration = false;

            return false;
        }


        private bool DoesHaveMultipleDependenciesImplementation(
            ServiceClassEntry serviceEntry,
            IEnumerable<ServiceClassEntry> allDependencies,
            out string dependencyList)
        {
            dependencyList = string.Empty;

            var hasMultipleBindTo =
                allDependencies.Where(wh => !wh.isUsedInConfiguration)
                               .GroupBy(gb => gb.bindTo)
                               .Where(wh => wh.Count() > 1)
                               .ToList();

            if (hasMultipleBindTo.Count == 0)
            {
                return false;
            }

            StringBuilder str = new StringBuilder();

            foreach (var multipleBindTo in hasMultipleBindTo)
            {
                if (serviceEntry.dependencies.Contains(multipleBindTo.Key))
                {
                    foreach (var service in multipleBindTo)
                    {
                        str.Append(service.typeName + ", ");
                    }
                }
            }

            if (str.Length == 0)
            {
                return false;
            }

            dependencyList = str.ToString().TrimEnd(' ', ',');
            serviceEntry.isUsedInConfiguration = false;

            return true;
        }


        private bool DoesImplementationExist(ServiceClassEntry serviceEntry, out string serviceList)
        {
            serviceList = string.Empty;

            var allImplementation = ServiceTypes.Where(wh => wh != serviceEntry &&
                                                             wh.isUsedInConfiguration &&
                                                             serviceEntry.bindTo != null &&
                                                             serviceEntry.bindTo == wh.bindTo).ToList();

            if (allImplementation.Count == 0)
            {
                return false;
            }
            
            StringBuilder str = new StringBuilder();
            allImplementation.ForEach(fe => str.Append(fe.typeName + ", "));

            serviceList = str.ToString().TrimEnd(' ', ',');
            serviceEntry.isUsedInConfiguration = false;
            
            return true;
        }


        private bool DoesServiceInUse(ServiceClassEntry serviceEntry, out string serviceList)
        {
            serviceList = string.Empty;

            var servicesUsed =
                ServiceTypes.Where(wh => wh.isUsedInConfiguration &&
                                         wh.dependencies != null &&
                                         wh.dependencies.Any(dependency => serviceEntry.bindTo == dependency))
                             .ToList();

            if (servicesUsed.Count == 0)
            {
                return false;
            }

            StringBuilder str = new StringBuilder();
            servicesUsed.ForEach(fe => str.Append(fe.typeName + ", "));

            serviceList = str.ToString().TrimEnd(' ', ',');

            serviceEntry.isUsedInConfiguration = true;

            return true;
        }


        private void SetInUseDependencies(ServiceClassEntry serviceEntry, IEnumerable<ServiceClassEntry> allDependencies)
        {
            var hasSingleBindTo =
               allDependencies.GroupBy(gb => gb.bindTo)
                              .Where(wh => wh.Count() == 1)
                              .ToList();

            foreach (var singleBindTo in hasSingleBindTo)
            {
                if (serviceEntry.dependencies.Contains(singleBindTo.Key))
                {
                    foreach (var service in singleBindTo.Where(wh => !wh.isUsedInConfiguration))
                    {
                        service.isUsedInConfiguration = true;

                        InitializationQueueConfiguration.Instance.AddBinding(service.type, service.servicePrefab,
                            serviceEntry.serviceAsset);
                    }
                }
            }
        }

        #endregion
    }
}
