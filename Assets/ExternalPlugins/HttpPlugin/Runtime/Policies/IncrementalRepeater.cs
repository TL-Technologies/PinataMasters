namespace Http
{
    public class IncrementalRepeater<T> : BaseRepeater<T> where T : IHttpResponse
    {
        private int attempt = 0;


        public int BaseDelay { get; set; } = 1000;


        public int MaxDelay { get; set; } = 0;


        public int MaxAttempts { get; set; } = 3;


        public IncrementalRepeater(IHttpOperation<T> operation) : base(operation)
        {
        }


        protected override int GetNextDelayTime()
        {
            attempt++;
            if (MaxAttempts > 0 && attempt >= MaxAttempts)
                return -1;

            int delay = BaseDelay * attempt;

            if (MaxDelay > 0 && delay > MaxDelay)
                delay = MaxDelay;

            return delay;
        }


        protected override void Reset()
        {
            attempt = 0;
        }
    }
}
