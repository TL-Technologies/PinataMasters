using System;


namespace Modules.General.Abstraction.GooglePlayGameServices
{
    public struct NativeScore
    {
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public long score;
        public long timestampMillis;
        public string displayScore;
        public string playerId;
        public long rank;
        
        
        public DateTime Date => UnixEpoch.AddMilliseconds(timestampMillis);
        
        
        public override string ToString() =>
            $"[NativeScore] {nameof(score)}={score}, {nameof(timestampMillis)}={timestampMillis}, " +
            $"{nameof(displayScore)}={displayScore}, {nameof(playerId)}={playerId}, {nameof(rank)}={rank}.";
    }
}