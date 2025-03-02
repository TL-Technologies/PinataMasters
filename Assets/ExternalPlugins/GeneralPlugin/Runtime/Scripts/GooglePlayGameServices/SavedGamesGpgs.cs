using Modules.General.Abstraction.GooglePlayGameServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.GooglePlayGameServices
{
    internal class SavedGamesGpgs : ISavedGamesGpgs
    {
        #region Nested types

        private interface IAndroidJavaClass
        {
            void CallStatic(string methodName, params object[] args);
        }
        
        
        private class DummyAndroidJavaClass : IAndroidJavaClass
        {
            public void CallStatic(string methodName, params object[] args) { }
        }
        
        
        private class ActualAndroidJavaClass : IAndroidJavaClass
        {
            private AndroidJavaClass javaClass;
            
            public ActualAndroidJavaClass(string className)
            {
                javaClass = new AndroidJavaClass(className);
            }


            public void CallStatic(string methodName, params object[] args)
            {
                javaClass.CallStatic(methodName, args);
            }
        }

        #endregion
        
        
        
        #region Fields
        
        private const string InitMethodName = "LLSavedGamesGPGSInit";
        private const string SetCallbacksMethodName = "LLSavedGamesGPGSSetCallbacks";
        private const string LoadGameProgressMethodName = "LLSavedGamesGPGSLoadGameProgress";
        private const string SaveGameProgressMethodName = "LLSavedGamesGPGSSaveGameProgress";
        private const string DeleteGameProgressMethodName = "LLSavedGamesGPGSDeleteGameProgress";
        
        public event Action<bool> OnProgressLoad;
        public event Action<bool> OnProgressSaved;
        
        private bool isInitialized;
        private Dictionary<string, object> gameProgress = new Dictionary<string, object>();
        
        private ISocialGpgs socialGpgs;
        private IAndroidJavaClass androidJavaClass;
        
        #endregion



        #region Properties

        private IAndroidJavaClass SavedGamesAndroidJavaClass
        {
            get
            {
                if (androidJavaClass == null)
                {
                    androidJavaClass =
                        #if UNITY_EDITOR
                            new DummyAndroidJavaClass();
                        #else
                            new ActualAndroidJavaClass("com.lllibset.LLSocialGPGS.LLSavedGamesGPGS");
                        #endif
                }
                
                return androidJavaClass;
            }
        }

        #endregion
        
        
        
        #region Class lifecycle
        
        public SavedGamesGpgs(ISocialGpgs socialGpgsImplementation)
        {
            socialGpgs = socialGpgsImplementation;
        }
        
        #endregion



        #region ISavedGamesGpgs
        
        public bool IsAuthenticated =>
            isInitialized &&
            socialGpgs.HasPermission(GooglePermission.DriveAppFolder) &&
            socialGpgs.HasPermission(GooglePermission.GamesLite);


        public bool IsLoaded { get; private set; }
        

        public void Initialize()
        {
            if (!isInitialized)
            {
                SavedGamesAndroidJavaClass.CallStatic(InitMethodName);
                
                if (socialGpgs.localUser.authenticated)
                {
                    CustomDebug.Log("SavedGamesGpgs initialized.");
                    isInitialized = true;
                    
                    SavedGamesAndroidJavaClass.CallStatic(
                        SetCallbacksMethodName,
                        LLAndroidJavaCallback.ProxyCallback(OnGameProgressSaved),
                        LLAndroidJavaCallback.ProxyCallback(OnJsonGameProgressLoaded));
                }
                else
                {
                    CustomDebug.Log("Try authenticate for initialize SavedGamesGpgs");
                    socialGpgs.localUser.Authenticate(success =>
                    {
                        if (success)
                        {
                            Initialize();
                        }
                    });
                }
            }
        }
        
        
        public void SetBool(string key, bool value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);


        public void SetFloat(string key, float value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);


        public void SetDouble(string key, double value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);


        public void SetInt(string key, int value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);


        public void SetDateTime(string key, DateTime value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);


        public void SetString(string key, string value, bool shouldSaveImmediately = false) =>
            SetValue(key, value, shouldSaveImmediately);
        
        
        public bool GetBool(string key, bool defaultValue = default) =>
            GetValue(key, defaultValue);


        public float GetFloat(string key, float defaultValue = default) =>
            GetValue(key, defaultValue);


        public double GetDouble(string key, double defaultValue = default) =>
            GetValue(key, defaultValue);


        public int GetInt(string key, int defaultValue = default) =>
            GetValue(key, defaultValue);


        public DateTime GetDateTime(string key, DateTime defaultValue) =>
            GetValue(key, defaultValue);


        public string GetString(string key, string defaultValue) =>
            GetValue(key, defaultValue);
        
        
        public bool HasKey(string key) => gameProgress.ContainsKey(key);


        public void DeleteKey(string key)
        {
            if (gameProgress.ContainsKey(key))
            {
                gameProgress.Remove(key);
                Save();
            }
        }


        public void DeleteAll()
        {
            if (gameProgress.Keys.Count > 0)
            {
                gameProgress.Clear();
                Save();
            }
        }


        public void Save()
        {
            if (IsAuthenticated)
            {
                string json = JsonConvert.SerializeObject(gameProgress);
                CustomDebug.Log("Save GameProgress " + json);

                SavedGamesAndroidJavaClass.CallStatic(SaveGameProgressMethodName, json);
            }
        }


        public void LoadGameProgress()
        {
            if (IsAuthenticated)
            {
                CustomDebug.Log("Load GameProgress");

                SavedGamesAndroidJavaClass.CallStatic(LoadGameProgressMethodName);
            }
        }


        public void DeleteGameProgress()
        {
            if (IsAuthenticated)
            {
                CustomDebug.Log("Delete GameProgress");

                SavedGamesAndroidJavaClass.CallStatic(DeleteGameProgressMethodName);
            }
        }

        #endregion



        #region Methods
        
        private void SetValue(string key, object value, bool shouldSaveImmediately = false)
        {
            if (gameProgress.ContainsKey(key))
            {
                gameProgress[key] = value;
            }
            else
            {
                gameProgress.Add(key, value);
            }

            if (shouldSaveImmediately)
            {
                Save();
            }
        }


        private T GetValue<T>(string key, T defaultValue)
        {
            if (gameProgress.TryGetValue(key, out object value))
            {
                if (value is ValueType)
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                if (value is T value1)
                {
                    return value1;
                }
            }

            return defaultValue;
        }
        

        private void OnGameProgressSaved(bool success)
        {
            CustomDebug.Log("Gpgs: game progress saved " + success);

            if (!success)
            {
                IsLoaded = false;
                LoadGameProgress();
            }

            OnProgressSaved?.Invoke(success);
        }
        
        
        private void OnJsonGameProgressLoaded(string json)
        {
            CustomDebug.Log("Gpgs: json game progress loaded " + json);

            IsLoaded = false;
            gameProgress.Clear();

            if (!string.IsNullOrEmpty(json))
            {
                gameProgress = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);

                if (gameProgress != null)
                {
                    OnProgressLoad?.Invoke(true);
                    IsLoaded = true;

                    return;
                }
            }

            OnProgressLoad?.Invoke(false);
        }

        #endregion
    }
}
