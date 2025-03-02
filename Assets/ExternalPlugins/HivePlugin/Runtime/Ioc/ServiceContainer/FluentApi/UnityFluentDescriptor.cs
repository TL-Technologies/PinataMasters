using System;
using UnityEngine;


namespace Modules.Hive.Ioc
{
    /// <summary>
    /// Represents a fluent descriptor that can be used to configure a new Unity service
    /// and add it to <see cref="IServiceContainer"/>.
    /// </summary>
    public interface IUnityFluentDescriptor<in T> :
        IFluentGeneral<IUnityFluentDescriptor<T>>,
        IFluentImplementationInstance<IUnityFluentDescriptor<T>, T>
        where T : UnityEngine.Object
    {
        /// <summary>
        /// Sets an asset path to resource.
        /// </summary>
        /// <param name="pathToResource">An asset path to resource.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        IUnityFluentDescriptor<T> SetPathToResource(string pathToResource);


        /// <summary>
        /// Sets a reference to resource.
        /// </summary>
        /// <param name="referenceToResource">A reference to resource.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        IUnityFluentDescriptor<T> SetReferenceToResource(T referenceToResource);


        /// <summary>
        /// Sets whether to keep reference to resource (false by default).
        /// This allows to avoid loading one every time when factory creates an instance.
        /// It may be useful when you spawn many objects form one resource, 
        /// but please note that all resources use memory and be there while application is running.
        /// 
        /// Also you should know this option only works in conjunction with loading resource by path.
        /// </summary>
        /// <param name="keepReference">Set it true if you want to avoid resource loading every time when factory creates an instance. Otherwise - false.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        IUnityFluentDescriptor<T> SetKeepReferenceToResource(bool keepReference);


        /// <summary>
        /// Sets whether to instantiate a resource when <see cref="IServiceContainer"/> requests an instance of the service (true by default).
        /// This allows to provide a directly access to source object in resources without instantiating it in game space.
        /// Might be useful if you need to get data from target object rather than create an instance of it. 
        /// For example, a ScriptableObject asset used as a configuration storage or in other similar case.
        /// </summary>
        /// <param name="instantiateOnBuild">Set it true if you want to instantiate a resource when <see cref="IServiceContainer"/>
        /// requests an instance of the service. Otherwise - false.</param>
        /// <returns>Current instance of fluent service descriptor.</returns>
        IUnityFluentDescriptor<T> SetInstantiateOnDemand(bool instantiateOnBuild);


        IUnityFluentDescriptor<T> SetUseInstance();
    }


    internal class UnityFluentDescriptor<T> : FluentDescriptorBase, IUnityFluentDescriptor<T>
        where T : UnityEngine.Object
    {
        private string path = null;
        private T reference = null;
        private bool shouldKeepReference = false;
        private bool shouldInstantiate = true;


        public UnityFluentDescriptor(IServiceContainer container) : base(container)
        {
            ImplementationFactory = CreateInstance;
        }


        private T CreateInstance(IServiceProvider serviceProvider)
        {
            T res = reference;
            if (res == null)
            {
                res = Resources.Load<T>(path);
                if (shouldKeepReference)
                {
                    reference = res;
                }
            }

            return shouldInstantiate ? UnityEngine.Object.Instantiate(res) : res;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetImplementationInstance(T implementationInstance)
        {
            ImplementationInstance = implementationInstance;
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetLifetime(ServiceLifetime lifetime)
        {
            Lifetime = lifetime;
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> BindTo(Type abstractionType, ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = abstractionType, Options = bindingOptions });
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> BindTo<TAbstraction>(ServiceBindingOptions bindingOptions = ServiceBindingOptions.Default)
        {
            abstractions.Add(new ServiceBinding { Type = typeof(TAbstraction), Options = bindingOptions });
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetPathToResource(string pathToResource)
        {
            path = pathToResource;
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetReferenceToResource(T referenceToResource)
        {
            reference = referenceToResource;
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetKeepReferenceToResource(bool keepReference)
        {
            shouldKeepReference = keepReference;
            return this;
        }


        /// <inheritdoc />
        public IUnityFluentDescriptor<T> SetInstantiateOnDemand(bool instantiateOnBuild)
        {
            shouldInstantiate = instantiateOnBuild;
            return this;
        }


        public IUnityFluentDescriptor<T> SetUseInstance()
        {
            ImplementationFactory = null;
            return this;
        }


        protected override void Build()
        {
            if (ImplementationType == null)
            {
                ImplementationType = typeof(T);
            }
        }
    }
}
