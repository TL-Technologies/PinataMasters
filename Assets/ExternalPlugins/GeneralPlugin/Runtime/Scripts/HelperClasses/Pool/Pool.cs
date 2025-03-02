using System;
using System.Collections.Generic;
using UnityEngine;


namespace Modules.General.Pool
{
    public class Pool
    {
        #region Fields
        
        [NonSerialized] private PoolManager poolManager;
    
        private Dictionary<int, UnitGroup> poolCache = new Dictionary<int, UnitGroup>();
        private Dictionary<object, PoolUnit> unitGroupMark = new Dictionary<object, PoolUnit>();

        private bool restoreComponentsByDefault = true;
        
        #endregion
        
        
        
        #region Properties
        
        public int CreatedCount { get; internal set; }
        
        #endregion
        
        
        
        #region Class lifecycle
        
        internal Pool(PoolManager poolManager)
        {
            this.poolManager = poolManager;
        }
        
        #endregion
        
        
        
        #region Methods
        
        public GameObject TakeGameObject(GameObject prefab)
        {
            return TakeObject<GameObject>(prefab);
        }


        public T TakeObject<T>(T prefab) where T : class
        {
            UnitGroup group = GetUnitGroup<T>(prefab);
            return group.TakeUnit<T>();
        }


        public void SetRestoreComponentsByDefault(bool value, bool applyForAlreadyCreatedGroups = true)
        {
            restoreComponentsByDefault = value;

            if (!applyForAlreadyCreatedGroups) return;

            foreach (UnitGroup unit in poolCache.Values)
            {
                unit.SetRestoreComponents(restoreComponentsByDefault);
            }
        }


        public void SetRestoreComponentsForObject<T>(T prefab, bool value) where T : class
        {
            UnitGroup group = GetUnitGroup<T>(prefab);
            group.SetRestoreComponents(value);
        }


        internal UnitGroup GetUnitGroup<T>(T prefab) where T : class
        {
            int instanceId = GetInstanceId(prefab);

            if (!poolCache.TryGetValue(instanceId, out UnitGroup group))
            {
                group = new UnitGroup(this, instanceId);
                group.SetTemplate(prefab);
                group.SetRestoreComponents(restoreComponentsByDefault);
                poolCache.Add(instanceId, group);
            }

            return group;
        }
        
        
        public bool Restore(object target)
        {
            bool result = false;
            if (unitGroupMark.ContainsKey(target))
            {
                unitGroupMark[target].Restore();
                result = true;
            }
            
            return result;
        }


        public void ClearGroup(object prefab, bool clearWorkList = false)
        {
            int instanceId = GetInstanceId(prefab);
            
            if (!poolCache.TryGetValue(instanceId, out UnitGroup group))
            {
                return;
            }

            if (group.Clear(clearWorkList))
            {
                poolCache.Remove(instanceId);
            }
        }


        public bool Clear(bool clearWorkList = false)
        {
            bool isEmpty = true;
            List<int> keys = new List<int>(poolCache.Keys);
            foreach (var key in keys)
            {
                if (poolCache[key].Clear(clearWorkList))
                {
                    isEmpty = false;
                    poolCache.Remove(key);
                }
            }

            return isEmpty;
        }
        
    
        internal void MarkUnit(object target, PoolUnit holder)
        {
            if (!unitGroupMark.ContainsKey(target))
            {
                unitGroupMark.Add(target, holder);
            }
            else
            {
                unitGroupMark[target] = holder;
            }
        }


        internal int GetInstanceId(object prefab)
        {
            if (prefab is UnityEngine.Object go)
            {
                return go.GetInstanceID();
            }
            else
            {
                return prefab.GetHashCode();
            }
        }
        
    
        internal void UnmarkUnit(object target)
        {
            if (unitGroupMark.ContainsKey(target))
            {
                unitGroupMark.Remove(target);
            }
        }
        
        
        internal void OnTookUnit(PoolUnit unit)
        {
            GameObject targetGo = unit.GetTargetGo();
            if (targetGo != null)
            {
                targetGo.transform.SetParent(poolManager.WorkAnchor);
            }
        }
        
        
        internal void OnRestoreUnit(PoolUnit unit, bool restoreComponents)
        {
            GameObject targetGo = unit.GetTargetGo();
            if (targetGo != null)
            {
                if (restoreComponents)
                {
                    IPoolRestorable[] restorableComponents = targetGo.GetComponents<IPoolRestorable>();
                    for (int i = 0; i < restorableComponents.Length; i++)
                    {
                        restorableComponents[i].Restore();
                    }
                }

                targetGo.transform.SetParent(poolManager.IdleAnchor);
            }
        }
        
        #endregion
    }
}
