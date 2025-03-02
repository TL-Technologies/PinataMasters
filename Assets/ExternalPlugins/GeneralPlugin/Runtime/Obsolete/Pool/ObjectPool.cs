using UnityEngine;
using System.Collections.Generic;


namespace Modules.General.Obsolete
{
    public class ObjectPool : MonoBehaviour 
    {    
        #region Fields
        
        public GameObject prefab;
        public Transform  Parent;

        public int preInstantiateCount;
        public bool autoExtend;
        public int currentCount;

        protected Stack<GameObject> stack = new Stack<GameObject>();

        private Vector3 GO_POOL_POSITION = new Vector3(0f, 0f, -1000f);
        private int countInPool;
        
        #endregion



        #region Methods
      
        public int GetStackLength()
        {
            return stack.Count;
        }
        

        public virtual void Push(GameObject go)
        {
            go.GetComponent<PoolableObjectInfo>().inPool = true;

            PushObject(go);

            foreach (IPoolCallback callback in go.GetComponentsInChildren(typeof(IPoolCallback)))
            {
                callback.OnReturnToPool();
            }
            
            foreach (IPoolCallback callback in go.GetComponents(typeof(IPoolCallback)))
            {
                callback.OnPush();
            }

            Transform goTrans = go.transform;
            goTrans.position = GO_POOL_POSITION;
            
            if (goTrans.parent != Parent)
            {
                goTrans.SetParent(Parent);//.parent = Parent;
            }
            
            if (Parent == null)
            {
                goTrans.SetParent(PoolManager.Instance.PoolObjectsRoot);
            }

            go.SetActive(false);
            currentCount = GetObjectsCount();
        }


        public virtual GameObject Pop(System.Action<GameObject> preAction = null)
        {
            GameObject go = null;

            if (GetObjectsCount() == 0)
            {
                if (autoExtend) 
                {
                    go = CreateNewObject();
                } 
                else 
                {
                    return null;
                }
            }
            else
            {
                go = PopObject();
            }

            go.SetActive(true);
            go.GetComponent<PoolableObjectInfo>().inPool = false;
            
            if (preAction != null)
            {
                preAction(go);
            }
            
            foreach (IPoolCallback callback in go.GetComponentsInChildren(typeof(IPoolCallback)))
            {
                callback.OnCreateFromPool();
            }
            
            foreach (IPoolCallback callback in go.GetComponents(typeof(IPoolCallback)))
            {
                callback.OnPop();
            }
            
            currentCount = GetObjectsCount();

            return go;
        }
        
        
        protected virtual void Start()
        {
            Parent = transform;

            for (int i = 0; i < preInstantiateCount; i++) 
            {
                var go = CreateNewObject();
                Push(go);
            }
        }


        protected virtual int GetObjectsCount()
        {
            return stack.Count;
        }


        protected virtual void PushObject(GameObject go)
        {
            stack.Push(go);
        }


        protected virtual GameObject PopObject()
        {
            return stack.Pop();
        }
        

        public GameObject PopObjectForUnload()
        {
            return PopObject ();
        }
        

        protected virtual GameObject CreateNewObject()
        {
            //CustomDebug.LogError("CreateNewObject : " + prefab.name);
            if (!prefab)
            {
                throw new System.Exception("Missing prefab in pool : " + gameObject.name);
            }
            
            GameObject go = Instantiate(prefab) as GameObject;
            go.name = prefab.name;
            PoolableObjectInfo poolInfo = go.GetComponent<PoolableObjectInfo>();
            
            if (poolInfo == null) 
            { 
                poolInfo = go.AddComponent<PoolableObjectInfo>();
            }
            
            poolInfo.poolReference = this;

            return go;
        }
        
        #endregion
    }
}
