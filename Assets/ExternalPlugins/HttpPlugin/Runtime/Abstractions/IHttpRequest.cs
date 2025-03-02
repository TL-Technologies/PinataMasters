using System.Collections.Generic;


namespace Http
{
    public interface IHttpRequest<T> : IHttpOperation<T> where T : IHttpResponse
    {
        IHttpClient HttpClient { get; }


        IDictionary<string, string> Headers { get; }


        IDictionary<string, string> Parameters { get; }


        string Url { get; }
    }
}