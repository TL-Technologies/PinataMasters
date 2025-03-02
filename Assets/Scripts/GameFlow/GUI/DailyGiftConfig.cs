using System;
using UnityEngine;


namespace PinataMasters
{
    public enum TypeReward
    {
        Small,
        Medium,
        Large,
    }

    [Serializable]
    public struct DailyGiftData
    {
        public float Multiplier;
        public TypeReward Type;
    }

    [Serializable]
    public struct DailyGiftConfig
    {
        public TypeReward Type;

        public Sprite BackSimple;
        public Sprite BackToday;

        public Sprite CoinsClaimed;
        public Sprite CoinsToday;
        public Sprite CoinsFuture;

        public Sprite BigCoins;
    }
}
