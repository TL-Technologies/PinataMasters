namespace Http
{
    public static class UniformRepeaterExtensions
    {
        public static UniformRepeater<T> AddUniformRepeater<T>(this IHttpOperation<T> operation) where T : IHttpResponse
        {
            return new UniformRepeater<T>(operation);
        }


        public static UniformRepeater<T> SetMaxAttempts<T>(this UniformRepeater<T> repeater, int maxAttempts)
            where T : IHttpResponse
        {
            repeater.MaxAttempts = maxAttempts;
            return repeater;
        }


        public static UniformRepeater<T> SetDelay<T>(this UniformRepeater<T> repeater, int delay)
            where T : IHttpResponse
        {
            repeater.Delay = delay;
            return repeater;
        }


        public static UniformRepeater<T> MakeInfinity<T>(this UniformRepeater<T> repeater) where T : IHttpResponse
        {
            repeater.MaxAttempts = 0;
            return repeater;
        }
    }
}
