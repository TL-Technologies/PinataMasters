using UnityEngine;

namespace PinataMasters
{
    public class ResourceAsset<T> where T : Object
    {
        private readonly string path;
        private T asset;

        public ResourceAsset(string path)
        {
            this.path = path;
        }

        public T Value
        {
            get 
            {
                asset = asset ?? Resources.Load<T>(path);
                return asset as T;
            }
         }

        public void LoadValueAsync()
        {
            asset = Resources.LoadAsync<T>(path).asset as T;
        }
    }
}
