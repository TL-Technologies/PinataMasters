using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General
{
    public class SerializableDictionaryAttribute : PropertyAttribute { }


    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>,
        ISerializationCallbackReceiver
    {
        #region Fields

        [SerializeField] private List<TKey> keysList = new List<TKey>();
        [SerializeField] private List<TValue> valuesList = new List<TValue>();
        private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        #endregion
        
        
        
        #region Properties
        
        public Dictionary<TKey, TValue>.KeyCollection Keys => dictionary.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => dictionary.Values;
        
        #endregion
        
        

        #region Properties

        public TValue this[TKey key]
        {
            get => dictionary[key];
            set => dictionary[key] = value;
        }

        public TValue this[TKey key, TValue defaultValue] =>
            dictionary.TryGetValue(key, out var value) ? value : defaultValue;

        public int Count => dictionary.Count;

        #endregion



        #region Methods

        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);


        public void Add(TKey key, TValue value) => dictionary.Add(key, value);
        
        
        public void Clear() => dictionary.Clear();


        public bool Remove(TKey key) => dictionary.Remove(key);


        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);


        public bool ContainsValue(TValue value) => dictionary.ContainsValue(value);


        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();


        IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();


        public void OnBeforeSerialize() { }


        public void OnAfterDeserialize()
        {
            dictionary = new Dictionary<TKey, TValue>();
            for (var i = 0; i < keysList.Count; i++)
            {
                if (valuesList.Count > i)
                {
                    dictionary[keysList[i]] = valuesList[i];
                }
            }
        }


        #if UNITY_EDITOR

        public void ReplaceSerializedValue()
        {
            keysList.Clear();
            valuesList.Clear();
            foreach (var pair in dictionary)
            {
                keysList.Add(pair.Key);
                valuesList.Add(pair.Value);
            }
        }
        
        
        public void CopyFrom(Dictionary<TKey, TValue> srcDictionary)
        {
            foreach(KeyValuePair<TKey, TValue> kv in srcDictionary)
            {
                Add(kv.Key, kv.Value);
            }
            
            ReplaceSerializedValue();
        }
        
        #endif

        #endregion
    }
}
