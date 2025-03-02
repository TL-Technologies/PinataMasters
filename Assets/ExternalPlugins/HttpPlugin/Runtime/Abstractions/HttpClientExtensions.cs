namespace Http
{
    public static class HttpClientExtensions
    {
        public static string GetHeader(this IHttpClient client, string key)
        {
            if (client.Headers.TryGetValue(key, out var value))
                return value;

            return null;
        }


        public static void SetHeader(this IHttpClient client, string key, string value)
        {
            client.Headers[key] = value;
        }


        public static void SetBasicAuthorizationToken(this IHttpClient client, string token)
        {
            client.SetHeader("Authorization", $"Basic {token}");
        }
    }
}
