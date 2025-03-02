using Modules.General;
using Modules.General.Abstraction;
using Modules.General.HelperClasses;
using Modules.General.InitializationQueue;
using System;
using System.Collections.Generic;
using UnityEngine.Scripting;


namespace Modules.Analytics
{
    [Preserve]
    [InitQueueService(-999, bindTo: typeof(IAnalyticsManager))]
    public class AnalyticsManager : IAnalyticsManager
    {
        #region Helper classes

        private struct CachedEvent
        {
            public Type type;
            public string eventName;
            public Dictionary<string, string> parameters;
        }

        private struct CachedUserProperty
        {
            public Type type;
            public string name;
            public string value;
        }

        #endregion



        #region Fields

        private const string AnalyticsEventCache = "analytics_event_cache";
        private const string AnalyticsPropertyCache = "analytics_property_cache";
        private static IAnalyticsManager instance = default;
        private readonly List<CachedEvent> cachedEvents =
            CustomPlayerPrefs.GetObjectValue(AnalyticsEventCache, new List<CachedEvent>());
        private readonly List<CachedUserProperty> cachedUserProperties =
            CustomPlayerPrefs.GetObjectValue(AnalyticsPropertyCache, new List<CachedUserProperty>());

        #endregion



        #region Properties

        /// <summary>
        /// Gets instance of the AnalyticsManager.
        /// </summary>
        public static IAnalyticsManager Instance => instance ?? (instance = Services.AnalyticsManager);

        #endregion



        #region Methods

        public void SendEvent(string eventName, Dictionary<string, string> parameters = null)
        {
            if (Services.AnalyticsProcessor != null)
            {
                if (cachedEvents.Count > 0)
                {
                    ProcessEvents();
                }

                Services.AnalyticsProcessor.SendEvent(eventName, parameters);
            }
            else
            {
                CacheEvent(eventName, parameters);
            }
        }


        public void SendEvent(Type type, string eventName, Dictionary<string, string> parameters = null)
        {
            if (Services.AnalyticsProcessor != null)
            {
                ProcessEvents();

                Services.AnalyticsProcessor.SendEvent(type, eventName, parameters);
            }
            else
            {
                CacheEvent(eventName, parameters, type);
            }
        }


        public void SendEvent(Type[] types, string eventName, Dictionary<string, string> parameters = null)
        {
            if (types == null)
            {
                return;
            }
            foreach (Type type in types)
            {
                SendEvent(type, eventName, parameters);
            }
        }


        private void CacheEvent(string eventName, Dictionary<string, string> parameters = null, Type type = null)
        {
            cachedEvents.Add(new CachedEvent()
            {
                eventName  = eventName,
                parameters = parameters,
                type = type
            });

            CustomPlayerPrefs.SetObjectValue(AnalyticsEventCache, cachedEvents);
        }


        private void ProcessEvents()
        {
            cachedEvents.ForEach(e =>
            {
                if (e.type != null)
                {
                    Services.AnalyticsProcessor.SendEvent(e.type, e.eventName, e.parameters);
                }
                else
                {
                    Services.AnalyticsProcessor.SendEvent(e.eventName, e.parameters);
                }
            });

            ClearEventsCache();
        }


        private void ClearEventsCache()
        {
            CustomPlayerPrefs.DeleteKey(AnalyticsEventCache);
            cachedEvents.Clear();
        }


        public void SetUserProperty(string name, string value)
        {
            if (Services.AnalyticsProcessor != null)
            {
                if (cachedUserProperties.Count > 0)
                {
                    ProcessUserProperties();
                }

                Services.AnalyticsProcessor.SetUserProperty(name, value);
            }
            else
            {
                CacheUserProperty(name, value);
            }
        }


        public void SetUserProperty(Type type, string name, string value)
        {
            if (Services.AnalyticsProcessor != null)
            {
                ProcessUserProperties();

                Services.AnalyticsProcessor.SetUserProperty(type, name, value);
            }
            else
            {
                CacheUserProperty(name, value, type);
            }
        }


        public void SetUserProperty(Type[] types, string name, string value)
        {
            if (types == null)
            {
                return;
            }
            foreach (Type type in types)
            {
                SetUserProperty(type, name, value);
            }
        }


        private void CacheUserProperty(string name, string value, Type type = null)
        {
            cachedUserProperties.Add(new CachedUserProperty()
            {
                name  = name,
                value = value,
                type = type
            });

            CustomPlayerPrefs.SetObjectValue(AnalyticsPropertyCache, cachedUserProperties);
        }


        private void ProcessUserProperties()
        {
            cachedUserProperties.ForEach(e =>
            {
                if (e.type != null)
                {
                    Services.AnalyticsProcessor.SetUserProperty(e.type, e.name, e.value);
                }
                else
                {
                    Services.AnalyticsProcessor.SetUserProperty(e.name, e.value);
                }
            });

            ClearUserPropertiesCache();
        }


        private void ClearUserPropertiesCache()
        {
            CustomPlayerPrefs.DeleteKey(AnalyticsPropertyCache);
            cachedUserProperties.Clear();
        }


        #endregion
    }
}
