using System;
using UnityEngine;


namespace PinataMasters
{
    [CreateAssetMenu]
    public class Purchases : ScriptableObject
    {
        #region Variables

        [Serializable]
        public class Config
        {
            public IAPs.Name productName = new IAPs.Name();        
            public float coins = 0;
            public float freeCoins = 0;       
        }
        

        private const string PATH_RESOURCES = "Game/Purchases";
        
        [SerializeField] Config[] purchases = null;
        [SerializeField] Config[] lateGamePurchases = null;

        private static Purchases instance;

        #endregion



        #region Properties

        private static Purchases Instance
        {
            get
            {
                instance = instance ?? (Purchases)Resources.Load(PATH_RESOURCES);
                return instance;
            }
        }

        
        public static Config[] DefaultPurchases => Instance.purchases;
        
        
        public static Config[] LateGamePurchases => Instance.lateGamePurchases;

        #endregion
    }
}
