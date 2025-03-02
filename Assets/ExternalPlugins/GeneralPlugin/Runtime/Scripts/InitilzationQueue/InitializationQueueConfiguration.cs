using Modules.General.HelperClasses;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.InitializationQueue
{
    public class InitializationQueueConfiguration : ScriptableSingleton<InitializationQueueConfiguration>
    {
        #region Nested types

        [Serializable]
        public class ServiceBinding
        {
            public string serviceClass;
            public GameObject servicePrefab;
            public ScriptableObject serviceAsset;
        }

        #endregion



        #region Fields

        [SerializeField] private List<ServiceBinding> serviceBindings = new List<ServiceBinding>();

        #endregion



        #region Properties

        public List<ServiceBinding> ServiceBindings => serviceBindings;

        #endregion



        #region Methods

        public void RemoveBinding(Type serviceInterface)
        {
            var binding = GetBinding(serviceInterface);
            if (binding != null)
            {
                ServiceBindings.Remove(binding);
            }
        }


        public ServiceBinding GetBinding(Type serviceInterface) =>
            ServiceBindings.Find(x => x.serviceClass == serviceInterface.AssemblyQualifiedName);


        public void AddBinding(Type serviceClass, GameObject servicePrefab = null, ScriptableObject serviceAsset = null)
        {
            var binding = GetBinding(serviceClass);
            if (binding != null)
            {
                binding.serviceClass = serviceClass.AssemblyQualifiedName;
                binding.servicePrefab = servicePrefab;
                binding.serviceAsset = serviceAsset;
            }
            else
            {
                if (serviceClass != null)
                {
                    ServiceBindings.Add(new ServiceBinding()
                    {
                        serviceClass = serviceClass.AssemblyQualifiedName,
                        servicePrefab = servicePrefab,
                        serviceAsset = serviceAsset,
                    });
                }
            }
        }

        #endregion
    }
}
