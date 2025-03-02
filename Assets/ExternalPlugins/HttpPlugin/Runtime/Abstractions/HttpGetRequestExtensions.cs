namespace Http
{
    public static class HttpGetRequestExtensions
    {
        public static IHttpGetRequest IfNonMatch(this IHttpGetRequest request, string eTag)
        {
            request.SetHeader("If-None-Match", $"\"{eTag}\"");
            return request;
        }
    }
}
