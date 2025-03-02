using Modules.General.HelperClasses;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.Obsolete
{
    public class PoolManager : SingletonMonoBehaviour<PoolManager>
    {
        #region Fields

        public List<ObjectPool> pools = new List<ObjectPool>();
        public Dictionary<int, ObjectPool> poolMap = new Dictionary<int, ObjectPool>();

        private Transform poolObjectsRoot;

        #endregion



        #region Properties

        new public static PoolManager Instance
        {
            get
            {
                PoolManager pm = PoolManager.InstanceIfExist;

                if (pm == null)
                {
                    GameObject g = new GameObject("PoolManager");
                    pm = g.AddComponent<PoolManager>();
                }

                return pm;
            }
        }


        public Transform PoolObjectsRoot
        {
            get
            {
                if (poolObjectsRoot == null)
                {
                    GameObject root = new GameObject("PoolObjectsRoot");
                    poolObjectsRoot = root.transform;
                }

                return poolObjectsRoot;
            }
        }

        #endregion



        #region Unity lifecycle

        protected override void Awake()
        {
            base.Awake();
            gameObject.GetComponentsInChildren<ObjectPool>(pools);

            #if UNITY_EDITOR
            pools.Sort((a, b) => string.CompareOrdinal(a.name, b.name));
            #endif

            foreach (ObjectPool pool in pools)
            {
                if (pool.prefab == null)
                {
                    CustomDebug.LogError("Missing prefab in pool : " + pool.name, pool);
                }
                else
                {
                    if (!poolMap.ContainsKey(pool.prefab.GetInstanceID()))
                    {
                        poolMap.Add(pool.prefab.GetInstanceID(), pool);
                    }
                    else
                    {
                        CustomDebug.LogError("Duplicate : " + pool.prefab.name, pool);
                    }
                }
            }
        }

        #endregion



        #region Methods

        public ObjectPool FindPool(Object prefab)
        {
            ObjectPool pool = null;

            if (!poolMap.TryGetValue(prefab.GetInstanceID(), out pool))
            {
                //CustomDebug.LogError("Cant find pool for prefab : " + prefab.name);
            }

            return pool;
        }


        public ObjectPool PoolForObject(GameObject prefab)
        {
            ObjectPool pool = FindPool(prefab);

            if (pool == null)
            {
                GameObject poolObject = new GameObject();
                poolObject.name = prefab.name + "Pool";
                poolObject.transform.position = Vector3.zero;
                poolObject.transform.parent = transform;
                pool = poolObject.AddComponent<ObjectPool>();
                pool.prefab = prefab;
                pool.autoExtend = true;

                pools.Add(pool);
                poolMap.Add(pool.prefab.GetInstanceID(), pool);
            }

            return pool;
        }


        public void RemoveObjectPool(ObjectPool pool)
        {
            if (pool != null)
            {
                pools.Remove(pool);
                poolMap.Remove(pool.prefab.GetInstanceID());
            }
        }

        #endregion
    }
}
