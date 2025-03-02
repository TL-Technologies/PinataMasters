using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.Scripting;


namespace Modules.Analytics
{
    [Preserve]
    [InitQueueService(1, bindTo: typeof(IAnalyticsProcessor))]
    public class AnalyticsProcessor : ServicesInitializer, IAnalyticsProcessor, IInitializableService
    {
        #region Fields

        private readonly Dictionary<Type, IAnalyticsService> availableAnalyticsServices =
            new Dictionary<Type, IAnalyticsService>();

        private IEventsSamplingService eventsSamplingService;

        #endregion



        #region Properties

        private IEventsSamplingService EventsSamplingService =>
            eventsSamplingService ?? (eventsSamplingService = Services.EventsSamplingService);

        #endregion



        #region Methods

        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            IAnalyticsManagerSettings settings = container.GetService<IAnalyticsManagerSettings>();

            if (settings != null)
            {
                OnInitialized += OnManagerInitialized;

                Initialize(settings.Services);
            }
            else
            {
                onErrorCallback?.Invoke(this, InitializationStatus.Failed);
            }

            void OnManagerInitialized(InitializationStatus status)
            {
                OnInitialized -= OnManagerInitialized;

                switch (status)
                {
                    case InitializationStatus.Failed:
                        onErrorCallback?.Invoke(this, InitializationStatus.Failed);
                        break;

                    case InitializationStatus.Warning:
                        onErrorCallback?.Invoke(this, InitializationStatus.Warning);
                        break;

                    default:
                        onCompleteCallback?.Invoke();
                        break;
                }
            }
        }


        /// <summary>
        /// Performs service initialization with specified array of services.
        /// </summary>
        /// <param name="analyticsServices">The array of analytics services.</param>
        public void Initialize(IAnalyticsService[] analyticsServices)
        {
            DeviceInfo.RequestDeviceId(id =>
            {
                foreach (IAnalyticsService analyticsService in analyticsServices)
                {
                    analyticsService.SetDeviceId(id);
                }

                base.Initialize(analyticsServices);
            });
        }


        /// <summary>
        /// Gets a service of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T"/> or null if there is no such service.</returns>
        public T GetService<T>() where T : class, IAnalyticsService
        {
            T result = null;

            if (availableAnalyticsServices.TryGetValue(typeof(T), out var resultAnalyticsService))
            {
                result = resultAnalyticsService as T;
            }
            else
            {
                CustomDebug.LogError($"Service with type {typeof(T)} is empty");
            }

            return result;
        }


        /// <summary>
        /// Performs sending event with parameters in all available analytics services.
        /// </summary>
        /// <param name="eventName">The name of event.</param>
        /// <param name="parameters"> Custom parameters to send with the event.
        /// </param>
        public void SendEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            foreach (IAnalyticsService analyticsService in availableAnalyticsServices.Values)
            {
                if (IsEventSendingAvailable(analyticsService.GetType() ,eventName))
                {
                    analyticsService.SendEvent(eventName, parameters);
                }
            }
            
        }


        /// <summary>
        /// Performs sending event with parameters in one specific analytics service.
        /// </summary>
        /// <param name="type">>The type of analytics service.</param>
        /// <param name="eventName">The name of event.</param>
        /// <param name="parameters"> Custom parameters to send with the event.
        /// </param>
        public void SendEvent(Type type, string eventName, Dictionary<string, string> parameters = null)
        {
            if (IsEventSendingAvailable(type, eventName))
            {
                if (availableAnalyticsServices.TryGetValue(type, out var analyticsService))
                {
                    analyticsService.SendEvent(eventName, parameters);
                }
                else
                {
                    CustomDebug.LogError(
                        $"Can't send event to service with type {type}. " +
                        "Service is not available");
                }
            }
        }


        /// <summary>
        /// Performs setting user property value in all available analytics services.
        /// </summary>
        /// <param name="name">The name of property.</param>
        /// <param name="value">The value of property.</param>
        public void SetUserProperty(string name, string value)
        {
            foreach (IAnalyticsService analyticsService in availableAnalyticsServices.Values)
            {
                if (IsEventSendingAvailable(analyticsService.GetType(), name))
                {
                    analyticsService.SetUserProperty(name, value);
                }
            }
        }


        /// <summary>
        /// Performs setting user property value in one specific analytics service.
        /// </summary>
        /// <param name="type">>The type of analytics service.</param>
        /// <param name="name">The name of property.</param>
        /// <param name="value">The value of property.</param>
        public void SetUserProperty(Type type, string name, string value)
        {
            if (IsEventSendingAvailable(type, name))
            {
                if (availableAnalyticsServices.TryGetValue(type, out var analyticsService))
                {
                    analyticsService.SetUserProperty(name, value);
                }
                else
                {
                    CustomDebug.LogError(
                        $"Can't set property in service with type {type}. " +
                        "Service is not available");
                }
            }
        }


        private bool IsEventSendingAvailable(Type type, string eventName)
        {
            if (EventsSamplingService == null || !EventsSamplingService.IsBlockingEventsEnabledForModule(type.FullName))
            {
                return true;
            }

            return !EventsSamplingService.FindBlockedEventsListForModule(type.FullName).Contains(eventName);
        }

        #endregion



        #region ServicesInitializer

        protected override void AfterInitializationCallback(InitializationStatus initializationStatus)
        {
            base.AfterInitializationCallback(initializationStatus);

            CommonGameAnalyticsTracker.Initialize();
        }


        protected override void AvailableServices_OnCollectionChanged(object sender,
            NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            base.AvailableServices_OnCollectionChanged(sender, notifyCollectionChangedEventArgs);

            switch (notifyCollectionChangedEventArgs.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (notifyCollectionChangedEventArgs.NewItems[0] is IAnalyticsService service)
                        {
                            availableAnalyticsServices.Add(service.GetType(), service);
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (notifyCollectionChangedEventArgs.OldItems[0] is IAnalyticsService service)
                        {
                            availableAnalyticsServices.Remove(service.GetType());
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:

                    break;
            }
        }

        #endregion
    }
}