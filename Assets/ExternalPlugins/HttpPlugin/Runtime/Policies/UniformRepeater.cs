namespace Http
{
    public sealed class UniformRepeater<T> : BaseRepeater<T> where T : IHttpResponse
    {
        private int attempt = 0;


        public int Delay { get; set; } = 1000;


        public int MaxAttempts { get; set; } = 2;


        public UniformRepeater(IHttpOperation<T> operation) : base(operation)
        {
        }


        protected override int GetNextDelayTime()
        {
            if (MaxAttempts <= 0)
            {
                return Delay;
            }

            attempt++;
            if (attempt >= MaxAttempts)
            {
                return -1;
            }

            return Delay;
        }


        protected override void Reset()
        {
            attempt = 0;
        }
    }
}
