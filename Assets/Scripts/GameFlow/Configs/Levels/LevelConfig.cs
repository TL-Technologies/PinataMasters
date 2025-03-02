using Modules.General.HelperClasses;
using System;
using UnityEngine;


namespace PinataMasters
{
    [Serializable]
    public struct LevelConfig
    {
        [Header("Back")]
        [ResourceLink]
        public AssetLink AssetBack;

        [Header("Pinata")]
        [ResourceLink]
        public AssetLink AssetPinata;
        public float HealthPinata;
        public float WinCoinsPersentHealthPinata;


        [Header("Obstacles")]
        [SerializeField]
        public bool UseCustomHealth;

        [SerializeField]
        [ConditionalHide("UseCustomHealth")]
        public float LevelObstacleHealth;
        [ResourceLink]
        public AssetLink AssetLevelObstacles;

        [SerializeField]
        [ConditionalHide("UseCustomHealth")]
        public float PinataObstacleHealth;
        [ResourceLink]
        public AssetLink AssetPinataObstacles;

        [Header("Prestige")]
        [SerializeField]
        public float GemsForReset;
    }
}
