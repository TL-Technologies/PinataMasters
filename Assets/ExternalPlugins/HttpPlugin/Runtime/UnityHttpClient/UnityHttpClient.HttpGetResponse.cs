using System.Collections.Generic;

namespace Http
{
    public partial class UnityHttpClient
    {
        private class HttpGetResponse : IHttpGetResponse
        {
            public int ResponseCode { get; }


            public IReadOnlyDictionary<string, string> Headers { get; }

            public string ETag => this.GetHeader("ETag");

            public string Body { get; }


            public HttpGetResponse(
                int responseCode, 
                IReadOnlyDictionary<string, string> headers,
                string body)
            {
                ResponseCode = responseCode;
                Headers = headers ?? new Dictionary<string, string>();
                Body = body;
            }
        }
    }
}
