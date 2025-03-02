using Modules.General.InitializationQueue;
using Modules.General.ServicesInitialization;
using Modules.Hive.Ioc;
using Newtonsoft.Json;
using System;
using PlayerPrefs = UnityEngine.PlayerPrefs;


namespace Modules.General
{
    [InitQueueService(-667, bindTo: typeof(IDataStateService))]
    public class DataStateService : IInitializableService, IDataStateService
    {
        #region Fields

        private static IDataStateService instance = null;
        private readonly DataState state = new DataState();

        #endregion



        #region Properties

        public static IDataStateService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Services.GetService<IDataStateService>();
                    if (instance == null)
                    {
                        instance = new DataStateService();

                        Services.Container.CreateService<IDataStateService>()
                            .BindTo<IDataStateService>()
                            .SetImplementationInstance(instance)
                            .AsSingleton();
                    }
                }

                return instance;
            }
        }

        #endregion



        #region Methods

        public bool TryGet<T>(string path, out T value)
        {
            return state.TryGet(path, out value);
        }


        public T Get<T>(string path, T defaultValue = default)
        {
            bool hasInState = TryGet(path, out T value);
            if (!hasInState)
            {
                ReadFromPrefs<T>(path, defaultValue);
                return Get<T>(path);
            }

            return value;
        }


        public void Set<T>(string path, T value, bool writeToPrefs = true)
        {
            if (writeToPrefs)
            {
                WriteToPrefs(path, value);
            }

            state.Set(path, value);
        }


        public void Remove(string path)
        {
            PlayerPrefs.DeleteKey(path);
            state.Remove(path);
        }


        public void AddListener(string valueName, Action<object, object> callback)
        {
            state.AddListener(valueName, callback);
        }


        public void RemoveListener(string valueName)
        {
            state.RemoveListener(valueName);
        }


        public void InitializeService(
            IServiceContainer container,
            Action onCompleteCallback,
            Action<IInitializableService, InitializationStatus> onErrorCallback)
        {
            if (container.GetService<IDataStateService>() == null)
            {
                instance = this;
            }

            onCompleteCallback?.Invoke();
        }


        private void WriteToPrefs<T>(string key, T value)
        {
            if (typeof(T) == typeof(int))
            {
                PlayerPrefs.SetInt(key, (int)(object)value);
            }
            else if (typeof(T) == typeof(float))
            {
                PlayerPrefs.SetFloat(key, (float)(object)value);
            }
            else if (typeof(T) == typeof(string))
            {
                PlayerPrefs.SetString(key, value.ToString());
            }
            else if (typeof(T) == typeof(bool))
            {
                PlayerPrefs.SetInt(key, (bool)(object)value ? 1 : 0);
            }
            else if (value is DateTime dateTime)
            {
                PlayerPrefs.SetString(key, dateTime.ToBinary().ToString());
            }
            else
            {
                PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
            }
        }


        private void ReadFromPrefs<T>(string key, T defaultValue = default)
        {
            if (typeof(T) == typeof(int))
            {
                int intValue = PlayerPrefs.GetInt(key, (int)(object)defaultValue);
                Set(key, intValue, false);
            }
            else if (typeof(T) == typeof(float))
            {
                float floatValue = PlayerPrefs.GetFloat(key, (float)(object)defaultValue);
                Set(key, floatValue, false);
            }
            else if (typeof(T) == typeof(string))
            {
                string stringValue = PlayerPrefs.GetString(key, (string)(object)defaultValue);
                Set(key, stringValue, false);
            }
            else if (typeof(T) == typeof(bool))
            {
                bool boolValue = PlayerPrefs.GetInt(key, (bool)(object)defaultValue ? 1 : 0) == 1;
                Set(key, boolValue, false);
            }
            else if (typeof(T) == typeof(DateTime))
            {
                string savedString = PlayerPrefs.GetString(key);
                var result = (DateTime)(object)defaultValue;

                if (!string.IsNullOrEmpty(savedString))
                {
                    long temp = Convert.ToInt64(savedString);
                    result = DateTime.FromBinary(temp);
                }

                Set(key, result, false);
            }
            else
            {
                string stringValue = PlayerPrefs.GetString(key);
                T result = defaultValue;

                if (!string.IsNullOrEmpty(stringValue))
                {
                    result = JsonConvert.DeserializeObject<T>(stringValue);
                }

                Set(key, result, false);
            }
        }

        #endregion
    }
}
