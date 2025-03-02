using UnityEngine;


namespace Modules.General.Pool
{
    internal sealed class PoolUnit
    {
        #region Fields

        public readonly object target;
        private UnitGroup parentList;

        #endregion 
    
    
    
        #region Properties
    
        internal PoolUnitStateType State { get; set; } = PoolUnitStateType.Idle;

        #endregion
    
    
    
        #region Class lifecycle
    
        internal PoolUnit(object target)
        {
            this.target = target;
        }
    
        #endregion
    
    
    
        #region Methods
    
        public GameObject GetTargetGo()
        {
            GameObject result = null;
            
            if (target != null)
            {
                if (target is GameObject gameObject)
                {
                    result = gameObject;
                }
                else if (target is Component component)
                {
                    result = component.gameObject;
                }
            }
        
            return result;
        }
    

        internal void SetParentList(UnitGroup list)
        {
            parentList = list;
        }

    
        internal void Restore()
        {
            parentList?.RestoreUnit(this);
        }

        #endregion
    }
}
