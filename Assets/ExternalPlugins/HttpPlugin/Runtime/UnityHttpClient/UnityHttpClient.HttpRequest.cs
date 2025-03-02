using System.Collections.Generic;
using System.Text;

namespace Http
{
    public partial class UnityHttpClient
    {
        private abstract class HttpRequest
        {
            protected readonly UnityHttpClient httpClient;
            private IDictionary<string, string> headers;
            private Dictionary<string, string> parameters;


            public IHttpClient HttpClient => httpClient;


            public IDictionary<string, string> Headers => headers ?? (headers = new Dictionary<string, string>());


            public IDictionary<string, string> Parameters => parameters ?? (parameters = new Dictionary<string, string>());


            public string Url { get; }


            public HttpRequest(UnityHttpClient httpClient, string url)
            {
                this.httpClient = httpClient;
                headers = httpClient.Headers;
                Url = url;
            }


            protected string BuildUrl()
            {
                if (parameters == null || parameters.Count == 0)
                    return Url;

                var url = new StringBuilder(Url.Length * 2);
                url.Append(Url);
                url.Append('?');
                foreach (var pair in parameters)
                {
                    url.Append(pair.Key);
                    url.Append('=');
                    url.Append(pair.Value); // TODO: need to escape string
                    url.Append('&');
                }
                url.Length = url.Length - 1;
                return url.ToString();
            }
        }
    }
}
