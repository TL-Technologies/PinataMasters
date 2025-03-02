using UnityEngine;

namespace PinataMasters
{
    [RequireComponent(typeof(Renderer))]
    public class SortingLayerRenderer : MonoBehaviour 
    {
        public string SortingLayerName = "Default";
        public int SortingOrder;

        private void Awake()
        {
            Renderer render = GetComponent<Renderer>();
            render.sortingLayerID = SortingLayer.NameToID(SortingLayerName);
            render.sortingOrder = SortingOrder;
        }
    }
}