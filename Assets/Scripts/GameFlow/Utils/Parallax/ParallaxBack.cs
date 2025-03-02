using System;
using UnityEngine;


namespace PinataMasters
{
    public class ParallaxBack : MonoBehaviour
    {
        [Serializable]
        private class Layer
        {
            public float Offset = 0;
            public Transform Body = null;
        }

        [SerializeField]
        private Layer[] layers = null;

        public static ParallaxBack Instance { get; private set; }

        public static bool ShouldShow { get; set; } = true;


        private void OnEnable()
        {
            UpdateParallax();
        }


        private void Update()
        {
            foreach (Layer layer in layers)
            {
                layer.Body.position = new Vector3(ShooterLegs.Offset * layer.Offset, layer.Body.position.y, layer.Body.position.z);
            }
        }


        public void UpdateParallax()
        {
            foreach (var i in layers)
            {
                if (i.Offset < 0f)
                {
                    i.Body.gameObject.SetActive(ShouldShow);
                }
            }
        }
    }
}