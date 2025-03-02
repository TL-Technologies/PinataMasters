using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Modules.General.Pool
{
    internal sealed class UnitGroup
    {
        #region Fields

        private Pool pool;

        private List<PoolUnit> idleList;
        private List<PoolUnit> workList;

        private GameObject template;

        private bool restoreComponents;

        private static Type gameObjectType = typeof(GameObject);
        private static Type componentType = typeof(Component);
        
        #endregion
        
        
        
        #region Class lifecycle
        
        public UnitGroup(Pool pool, int typeId)
        {
            this.pool = pool;
            idleList = new List<PoolUnit>();
            workList = new List<PoolUnit>();
        }
        
        #endregion
        
        
        
        #region Methods
        
        internal void SetTemplate(object newTemplate)
        {
            if (template != null)
            {
                Debug.LogError("You can't reset the template!");
                return;
            }
            if (newTemplate is Component templateComponent)
            {
                template = templateComponent.gameObject;
            }
            else if (newTemplate is GameObject templateGameObject)
            {
                template = templateGameObject;
            }
        }


        internal void SetRestoreComponents(bool value)
        {
            restoreComponents = value;
        }
        
        
        internal T TakeUnit<T>() where T : class
        {
            PoolUnit unit;
    
            if (idleList.Count > 0)
            {
                unit = idleList[0];
                idleList.RemoveAt(0);
            }
            else
            {
                unit = CreateNewUnit(typeof(T));
                if(unit != null)
                {
                    unit.SetParentList(this);
                }
            }
            
            T result = null;
            
            if (unit != null)
            {
                workList.Add(unit);
                unit.State = PoolUnitStateType.Work;
                pool.OnTookUnit(unit);
                result = unit.target as T;
            }
            
            return result;
        }
    
        
        internal void RestoreUnit(PoolUnit unit)
        {
            if (unit != null && unit.State == PoolUnitStateType.Work)
            {
                workList.Remove(unit);
                idleList.Add(unit);
                unit.State = PoolUnitStateType.Idle;
                pool.OnRestoreUnit(unit, restoreComponents);
            }
        }


        internal bool Clear(bool clearWorkList = false)
        {
            ClearList(idleList);

            if (clearWorkList)
            {
                ClearList(workList);
            }

            return workList.Count == 0;
        }


        private void ClearList(List<PoolUnit> poolUnits)
        {
            while (poolUnits.Count > 0)
            {
                int index = poolUnits.Count - 1;
                pool.UnmarkUnit(poolUnits[index].target);
                var go = poolUnits[index].GetTargetGo();
                if (go != null)
                {
                    Object.Destroy(go);
                }
                pool.CreatedCount--;
                poolUnits.RemoveAt(index);
            }
        }

        
        private PoolUnit CreateNewUnit(Type t)
        {
            object target = null;
            bool isQueryingGameObject = (t == gameObjectType);
            bool isQueryingComponent = (t == componentType);
            
            if (isQueryingGameObject || isQueryingComponent)
            {
                if (template != null)
                {
                    GameObject targetGo = Object.Instantiate(template);
                    if (isQueryingGameObject)
                    {
                        target = targetGo;
                    }
                    else
                    {
                        target = targetGo.GetComponent(t);
                    }
                }
            }
            else
            {
                target = Activator.CreateInstance(t);
            }
            
            if (target == null)
            {
                return null;
            }
            
            PoolUnit poolUnit = new PoolUnit(target);
            pool.CreatedCount++;
            pool.MarkUnit(target, poolUnit);
            return poolUnit;
        }
        
        #endregion
    }
}
