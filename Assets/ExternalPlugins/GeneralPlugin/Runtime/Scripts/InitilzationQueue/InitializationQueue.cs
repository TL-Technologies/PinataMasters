using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Modules.General.InitializationQueue
{
    public class InitializationQueue
    {
        #region Nested types

        private class InitializableService
        {
            public Type bindTo = null;
            public int order = 0;
            public ScriptableObject serviceAsset = null;
            public GameObject servicePrefab = null;
            public Type serviceType = null;
            public object registerable = null;
        }

        #endregion



        #region Fields

        public event Action OnServicePostInit;

        public event Action<object> OnServicePreInit;


        private static InitializationQueue instance = null;

        private IServiceContainer defaultContainer = Services.Container;

        private Action onComplete = null;

        private Action<object, InitializationStatus> onError = null;

        private List<InitializableService> queue = new List<InitializableService>();

        private List<InitializableService> services = new List<InitializableService>();

        #endregion



        #region Properties

        public static InitializationQueue Instance => instance ?? (instance = new InitializationQueue());

        #endregion



        #region Class lifecycle

        private InitializationQueue()
        {
            services.Clear();
        }

        #endregion



        #region Methods

        public InitializationQueue AddService<T>(GameObject prefab)
        {
            InitQueueServiceAttribute iqServiceAttribute = GetIQServiceAttribute(typeof(T));

            AddInitializableService<T>(iqServiceAttribute, prefab);

            return this;
        }


        public InitializationQueue AddService<T>(ScriptableObject asset)
        {
            InitQueueServiceAttribute iqServiceAttribute = GetIQServiceAttribute(typeof(T));

            AddInitializableService<T>(iqServiceAttribute, asset: asset);

            return this;
        }


        public InitializationQueue AddService<T>()
        {
            InitQueueServiceAttribute iqServiceAttribute = GetIQServiceAttribute(typeof(T));

            AddInitializableService<T>(iqServiceAttribute);

            return this;
        }


        public void Apply()
        {
            queue.Clear();

            foreach (var serviceClass in services)
            {
                object serviceInstance = null;
                if (serviceClass.servicePrefab != null)
                {
                    GameObject objectInstance = Object.Instantiate(serviceClass.servicePrefab);
                    if (objectInstance != null)
                    {
                        serviceInstance = objectInstance.GetComponent(serviceClass.serviceType);
                        Object.DontDestroyOnLoad(objectInstance);
                    }
                }
                else if (serviceClass.serviceAsset != null)
                {
                    serviceInstance = serviceClass.serviceAsset;
                }
                else
                {
                    serviceInstance = Activator.CreateInstance(serviceClass.serviceType);
                }

                if (serviceInstance != null)
                {
                    queue.Add(new InitializableService
                    {
                        registerable = serviceInstance,
                        serviceType = serviceClass.serviceType,
                        order = serviceClass.order,
                        bindTo = serviceClass.bindTo
                    });
                }
            }

            SortQueue();

            RegisterService();
        }


        public void Apply(InitializationQueueConfiguration configuration)
        {
            foreach (var binding in configuration.ServiceBindings)
            {
                Type serviceClass = Type.GetType(binding.serviceClass);

                if (serviceClass != null)
                {
                    InitQueueServiceAttribute iqServiceAttribute = GetIQServiceAttribute(serviceClass);

                    AddInitializableService(iqServiceAttribute, binding, serviceClass);
                }
                else
                {
                    CustomDebug.LogError(
                        $"Type {binding.serviceClass} doesn't exists. " +
                        "Check your Initialization Queue Configurator");
                }
            }

            Apply();
        }


        public InitializationQueue SetOnComplete(Action callback)
        {
            onComplete = callback;

            return this;
        }


        public InitializationQueue SetOnError(Action<object, InitializationStatus> callback)
        {
            onError = callback;

            return this;
        }


        private void AddInitializableService<T>(
            InitQueueServiceAttribute iqServiceAttribute,
            GameObject prefab = null,
            ScriptableObject asset = null)
        {
            services.Add(CreateInitializableService<T>(
                iqServiceAttribute.Order, prefab, asset, iqServiceAttribute.BindTo));
        }


        private void AddInitializableService(
            InitQueueServiceAttribute iqServiceAttribute,
            InitializationQueueConfiguration.ServiceBinding binding,
            Type serviceClass)
        {
            services.Add(CreateInitializableService(
                iqServiceAttribute.Order, serviceClass, binding, iqServiceAttribute.BindTo));
        }


        private InitializableService CreateInitializableService<T>(
            int order,
            GameObject prefab,
            ScriptableObject asset,
            Type bindTo = null)
        {
            return new InitializableService
            {
                serviceType = typeof(T),
                order = order,
                bindTo = bindTo,
                servicePrefab = prefab,
                serviceAsset = asset
            };
        }


        private InitializableService CreateInitializableService(
            int order,
            Type serviceClass,
            InitializationQueueConfiguration.ServiceBinding binding,
            Type bindTo = null)
        {
            return new InitializableService
            {
                serviceType = serviceClass,
                order = order,
                bindTo = bindTo,
                servicePrefab = binding.servicePrefab,
                serviceAsset = binding.serviceAsset
            };
        }


        private InitQueueServiceAttribute GetIQServiceAttribute(Type type)
        {
            InitQueueServiceAttribute iqServiceAttribute = type.GetCustomAttribute<InitQueueServiceAttribute>();

            if (iqServiceAttribute == null)
            {
                throw new ArgumentException(
                    $"Type {type.Name} must implement by {nameof(InitQueueServiceAttribute)} attribute!");
            }

            return iqServiceAttribute;
        }


        private void RegisterService()
        {
            InitializableService service = queue.Dequeue();

            OnServicePreInit?.Invoke(service.registerable);

            if (service.registerable is IInitializableService initializableService)
            {
                initializableService.InitializeService(defaultContainer, OnComplete, OnError);
            }
            else
            {
                OnComplete();
            }

            void OnComplete()
            {
                if (service.bindTo != null)
                {
                    defaultContainer.CreateService(service.serviceType)
                        .SetImplementationInstance(service.registerable)
                        .BindTo(service.bindTo)
                        .AsSingleton();
                }

                OnServicePostInit?.Invoke();

                if (queue.Count > 0)
                {
                    RegisterService();
                }
                else
                {
                    onComplete?.Invoke();
                }
            }

            void OnError(IInitializableService failedService, InitializationStatus errorCode)
            {
                onError?.Invoke(failedService, errorCode);

                // Ignore Warning error code and continue initialization
                if (errorCode == InitializationStatus.Warning)
                {
                    OnComplete();
                }
            }
        }


        private void SortQueue()
        {
            queue.Sort((a, b) =>
            {
                if (a.order > b.order)
                {
                    return 1;
                }

                if (a.order < b.order)
                {
                    return -1;
                }

                return 0;
            });
        }

        #endregion
    }
}
