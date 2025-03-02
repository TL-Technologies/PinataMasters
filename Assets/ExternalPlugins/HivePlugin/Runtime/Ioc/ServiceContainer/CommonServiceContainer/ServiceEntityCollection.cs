using System;
using System.Collections.Generic;


namespace Modules.Hive.Ioc
{
    internal struct ServiceEntityCollection
    {
        private ServiceEntity serviceEntity;
        private List<ServiceEntity> serviceEntities;
        private ServiceBindingOptions serviceBindingOptions;

        
        public bool Empty => serviceEntity == null && serviceEntities == null;

        
        public int Count
        {
            get
            {
                if (serviceEntities != null)
                {
                    return serviceEntities.Count;
                }

                return serviceEntity != null ? 1 : 0;
            }
        }

        
        public ServiceEntity Last => serviceEntities != null  ? serviceEntities[serviceEntities.Count - 1] : serviceEntity;

        
        public ServiceEntity this[int index]
        {
            get
            {
                if (serviceEntities != null)
                {
                    return serviceEntities[index];
                }
                if (index == 0)
                {
                    return serviceEntity;
                }

                throw new IndexOutOfRangeException();
            }
        }

        
        public ServiceEntityCollection CheckAndAdd(ServiceEntity entity, ServiceBinding binding)
        {
            // Is it an attempt to override an exclusive binding?
            if ((serviceBindingOptions & ServiceBindingOptions.Exclusive) != 0)
            {
                throw new InvalidOperationException(ServiceProviderUtilities.CannotOverrideExclusiveBindingExceptionMessage(
                                                        TypeNameHelper.GetTypeDisplayName(entity.ImplementationType),
                                                        TypeNameHelper.GetTypeDisplayName(binding.Type)));
            }

            // Is it an attempt to add an exclusive binding to not empty collection?
            if ((binding.Options & ServiceBindingOptions.Exclusive) != 0 && !Empty)
            {
                throw new InvalidOperationException(ServiceProviderUtilities.CannotApplyExclusiveBindingExceptionMessage(
                                                        TypeNameHelper.GetTypeDisplayName(entity.ImplementationType),
                                                        TypeNameHelper.GetTypeDisplayName(binding.Type)));
            }

            return Add(entity, binding.Options);
        }

        
        public ServiceEntityCollection CheckAndAdd(ServiceEntity entity, Type abstractionType)
            => CheckAndAdd(entity, entity.GetServiceAbstraction(abstractionType));

        
        private ServiceEntityCollection Add(ServiceEntity entity, ServiceBindingOptions bindingOptions)
        {
            ServiceEntityCollection newItem = new ServiceEntityCollection
            {
                serviceBindingOptions = bindingOptions
            };

            if (serviceEntities != null)
            {
                newItem.serviceEntity = null;
                newItem.serviceEntities = serviceEntities;
                newItem.serviceEntities.Add(entity);
                return newItem;
            }

            if (serviceEntity == null)
            {
                newItem.serviceEntity = entity;
                return newItem;
            }

            newItem.serviceEntity = null;
            newItem.serviceEntities = new List<ServiceEntity> { this.serviceEntity, entity };
            return newItem;
        }
        

        public ServiceEntityCollection Remove(ServiceEntity entity)
        {
            ServiceEntityCollection newItem = new ServiceEntityCollection
            {
                serviceBindingOptions = serviceBindingOptions
            };

            if (serviceEntity != null)
            {
                newItem.serviceEntity = this.serviceEntity == entity ? null : this.serviceEntity;
                newItem.serviceEntities = null;
            }

            else if (serviceEntities != null)
            {
                serviceEntities.Remove(entity);

                if (serviceEntities.Count > 1)
                {
                    newItem.serviceEntity = null;
                    newItem.serviceEntities = serviceEntities;
                }
                else
                {
                    newItem.serviceEntity = serviceEntities[0];
                    newItem.serviceEntities = null;
                    serviceEntities.Clear();
                }
            }

            return newItem;
        }
    }
}
