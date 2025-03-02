using UnityEngine;


namespace Modules.General.Obsolete
{
    public static class ObjectPoolHelper 
    {
        #region Methods

        public static void ReturnToPool(this GameObject t)
        {
            PoolableObjectInfo objectPoolInfo = t.GetComponent<PoolableObjectInfo>();
            
            if (objectPoolInfo != null) 
            {
                objectPoolInfo.ReturnToPool();
            } 
            else 
            {
                Object.Destroy(t);
            }
        }


        public static GameObject Clone(this GameObject t, bool autocreatepool = false,
                                       System.Action<GameObject> preAction = null)
        {
            GameObject clone = null;
            PoolableObjectInfo poolInfo = t.GetComponent<PoolableObjectInfo>();

            if (Application.isPlaying)
            {
                ObjectPool pool = null;

                if (poolInfo == null)
                {
                    if (autocreatepool)
                    {
                        pool = PoolManager.Instance.PoolForObject(t);
                    }
                }
                else
                {
                    pool = poolInfo.GetPool();
                } 

                if (pool != null)
                {
                    clone = pool.Pop(preAction);
                }
            }

            if (clone == null)
            {    
                clone = Object.Instantiate(t) as GameObject;
                clone.name = t.name;
            }

            return clone;
        }

        #endregion
    }
}
