using System;

namespace Http
{
    public static class HttpRequestExtensions
    {
        public static string GetHeader<T>(this IHttpRequest<T> request, string key) 
            where T : IHttpResponse
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return request.Headers.TryGetValue(key, out var value) ? value : null;
        }


        public static IHttpRequest<T> SetHeader<T>(this IHttpRequest<T> request, string key, string value)
            where T : IHttpResponse
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value != null)
                request.Headers[key] = value;

            return request;
        }


        public static string GetParameter<T>(this IHttpRequest<T> request, string key)
            where T : IHttpResponse
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            return request.Parameters.TryGetValue(key, out var value) ? value : null;
        }


        public static IHttpRequest<T> SetParameter<T>(this IHttpRequest<T> request, string key, string value)
            where T : IHttpResponse
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            if (value != null)
                request.Parameters[key] = value;

            return request;
        }


        public static IHttpRequest<T> SetParameter<T>(this IHttpRequest<T> request, string key, int value)
            where T : IHttpResponse
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            request.Parameters[key] = value.ToString();
            return request;
        }


        public static IHttpRequest<T> UseAuthorizationHeader<T>(this IHttpRequest<T> request) where T : IHttpResponse
        {
            string token = request.HttpClient.GetHeader("Authorization");
            if (!string.IsNullOrEmpty(token))
                request.SetHeader("Authorization", token);

            return request;
        }
    }
}
