namespace Http
{
    public static class HttpResponseExtensions
    {
        public static string GetHeader(this IHttpResponse response, string key)
        {
            if (response.Headers.TryGetValue(key, out var value))
                return value;

            return null;
        }
    }
}
