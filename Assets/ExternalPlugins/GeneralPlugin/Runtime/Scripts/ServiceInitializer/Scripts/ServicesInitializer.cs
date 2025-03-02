using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;


namespace Modules.General.ServicesInitialization
{
    public class ServicesInitializer
    {
        #region Fields

        private readonly ObservableCollection<IInitializable> availableServices;
        private readonly object serviceInitializationLock = new object();
        private readonly List<IInitializable> initializingServices;
        
        /// <summary>
        /// Event at the end of initialization of all analytics services.
        /// <param type="InitializationStatus">The status of initialization.
        /// <remarks>Fail if one or more services return status failed</remarks>
        /// <remarks>None if array of services is empty</remarks>
        /// </param>
        /// </summary>
        public event Action<InitializationStatus> OnInitialized;
        
        private bool isAnyServiceInitializationFailed = false;

        #endregion



        #region Class lifecycle

        protected ServicesInitializer()
        {
            initializingServices = new List<IInitializable>();
            
            availableServices = new ObservableCollection<IInitializable>();
            availableServices.CollectionChanged += AvailableServices_OnCollectionChanged;
        }

        #endregion



        #region Methods

        /// <summary>
        /// Performs service initialization with specified array of services.
        /// </summary>
        /// <param name="services">The array of analytics services.</param>
        protected async void Initialize(IInitializable[] services)
        {
            if (services.Length > 0)
            {
                List<IInitializable> newInitializables = new List<IInitializable>();
                
                foreach (IInitializable service in services)
                {
                    if (!initializingServices.Contains(service))
                    {
                        newInitializables.Add(service);

                        lock (serviceInitializationLock)
                        {
                            initializingServices.Add(service);
                        }
                    }
                }

                bool isConsentShouldBeSet = Services.PrivacyManager == null;
                
                foreach (IInitializable service in newInitializables)
                {
                    if (service.IsAsyncInitializationEnabled)
                    {
                        await InitializeServiceAsync(service, isConsentShouldBeSet);
                    }
                    else
                    {
                        InitializeService(service, isConsentShouldBeSet);
                    }
                }
            }
            else
            {
                BeforeInitializationCallback(InitializationStatus.None);
                OnInitialized?.Invoke(InitializationStatus.None);
            }
        }


        protected virtual void BeforeInitializationCallback(InitializationStatus initializationStatus) { }
        
        protected virtual void AfterInitializationCallback(InitializationStatus initializationStatus) { }


        private async Task InitializeServiceAsync(IInitializable service, bool isConsentShouldBeSet)
        {
            Task task = null;
            try
            {
                service.PreInitialize();
                task = Task.Run(() =>
                {
                    InitializeService(service, isConsentShouldBeSet);
                });
                await task;
            }
            catch (Exception exception)
            {
                Service_OnServiceInitialized(service, InitializationStatus.Failed);
                
                CustomDebug.Log($"Task {task?.Id} Exception Message" +
                                    $" {exception.Message}" +
                                    $"with StackTrace {exception.StackTrace}");
            }
        }


        private void InitializeService(IInitializable service, bool isConsentShouldBeSet)
        {
            if (isConsentShouldBeSet)
            {
                service.SetUserConsent(true);
            }
            
            service.OnServiceInitialized += Service_OnServiceInitialized;
            service.Initialize();
        }

        #endregion
        
        
        
        #region Events handlers

        private void Service_OnServiceInitialized(
            IInitializable service, 
            InitializationStatus initializationStatus)
        {
            lock (serviceInitializationLock)
            {
                if (initializationStatus == InitializationStatus.Failed)
                {
                    isAnyServiceInitializationFailed = true;

                    CustomDebug.LogError($"Service {service.GetType()} initialized " +
                                         $"with status {initializationStatus}");

                    if (availableServices.Contains(service))
                    {
                        availableServices.Remove(service);
                    }
                }
                else
                {
                    availableServices.Add(service);
                }

                initializingServices.Remove(service);
                if (initializingServices.Count == 0)
                {
                    InitializationStatus globalAnalyticServicesInitializationStatus =
                        (isAnyServiceInitializationFailed) ? 
                            (InitializationStatus.Failed) : 
                            (InitializationStatus.Success);
                    
                    Scheduler.Instance.CallMethodWithDelay(this, () =>
                    {
                        BeforeInitializationCallback(globalAnalyticServicesInitializationStatus);
                        OnInitialized?.Invoke(globalAnalyticServicesInitializationStatus);
                        AfterInitializationCallback(globalAnalyticServicesInitializationStatus);
                    }, 0.0f);
                }
            }
        }
        
        
        protected virtual void AvailableServices_OnCollectionChanged(
            object sender, 
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs) { }

        #endregion
    }
}
