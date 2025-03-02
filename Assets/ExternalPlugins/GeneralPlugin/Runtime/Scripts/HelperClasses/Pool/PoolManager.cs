using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.Pool
{
    public class PoolManager
    {
        #region Fields

        private static PoolManager instance;
        private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();

        private Pool defaultPool;
    
        #endregion
    
    
    
        #region Properties
    
        public static PoolManager Instance => instance ?? (instance = new PoolManager());
    
        public Pool Pool => defaultPool ?? (defaultPool = MakePool());
    
        public Transform WorkAnchor { get; }
        public Transform IdleAnchor { get; }

        #endregion
    
    
    
        #region Class lifecycle

        private PoolManager()
        {
            GameObject poolGo = new GameObject("PoolManager");
            Transform poolTransform = poolGo.transform;
            WorkAnchor = CreateObject(poolTransform, "Work").transform;
            IdleAnchor = CreateObject(poolTransform, "Idle").transform;
            IdleAnchor.gameObject.SetActive(false);
            
            
            GameObject CreateObject(Transform anchor, string name)
            {
                GameObject go = new GameObject(name);
                Transform transform = go.transform;
                transform.SetParent(anchor);
        
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
                transform.name = name;
        
                return go;
            }
        }
    
        #endregion
    
    
    
        #region Methods

        public Pool MakePool(string poolName = "Pool")
        {
            if (!pools.TryGetValue(poolName, out Pool pool))
            {
                pool = new Pool(this);
                pools.Add(poolName, pool);            
            }
        
            return pool;
        }


        public Pool GetPool(string poolName)
        {
            return pools.TryGetValue(poolName, out Pool pool) ? pool : null;
        }


        public void ClearPoolGroup(string poolName, GameObject prefab, bool clearWorkList = false)
        {
            var pool = GetPool(poolName);
            pool?.ClearGroup(prefab, clearWorkList);
        }


        public void ClearGroup(string poolName, object prefab, bool clearWorkList = false)
        {
            var pool = GetPool(poolName);
            pool?.ClearGroup(prefab, clearWorkList);
        }


        public void ClearPool(string poolName, bool clearWorkList = false)
        {
            var pool = GetPool(poolName);
            if (pool?.Clear(clearWorkList) ?? true)
            {
                pools.Remove(poolName);
            }
        }
        
        #endregion
    }
}
