using UnityEngine;

namespace PinataMasters
{
    public class ResourceGameObject<T> : ResourceAsset<GameObject> where T : MonoBehaviour
    {
        private GameObject instance;

        public T Instance
        {
            get
            {
                Instantiate();
                return instance.GetComponent<T>();
            }
        }

        public ResourceGameObject(string path) : base(path) { }

        public void Instantiate()
        {
            if (instance == null)
            {
                instance = Object.Instantiate(Value);
            }
        }
    }
}
