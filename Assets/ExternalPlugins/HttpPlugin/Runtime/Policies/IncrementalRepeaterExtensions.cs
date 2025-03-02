namespace Http
{
    public static class IncrementalRepeaterExtensions
    {
        public static IncrementalRepeater<T> AddIncrementalRepeater<T>(this IHttpOperation<T> operation)
            where T : IHttpResponse
        {
            return new IncrementalRepeater<T>(operation);
        }


        public static IncrementalRepeater<T> SetMaxAttempts<T>(this IncrementalRepeater<T> repeater, int maxAttempts)
            where T : IHttpResponse
        {
            repeater.MaxAttempts = maxAttempts;
            return repeater;
        }


        public static IncrementalRepeater<T> SetBaseDelay<T>(this IncrementalRepeater<T> repeater, int delay)
            where T : IHttpResponse
        {
            repeater.BaseDelay = delay;
            return repeater;
        }


        public static IncrementalRepeater<T> SetMaxDelay<T>(this IncrementalRepeater<T> repeater, int delay)
            where T : IHttpResponse
        {
            repeater.MaxDelay = delay;
            return repeater;
        }


        public static IncrementalRepeater<T> MakeInfinity<T>(this IncrementalRepeater<T> repeater)
            where T : IHttpResponse
        {
            repeater.MaxAttempts = 0;
            return repeater;
        }
    }
}
