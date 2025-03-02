using System.Collections.Generic;

namespace Http
{
    public partial class UnityHttpClient
    {
        private class HttpDownloadFileResponse : IHttpDownloadFileResponse
        {
            public int ResponseCode { get; }


            public IReadOnlyDictionary<string, string> Headers { get; }


            public string OutputPath { get; }


            public string ETag => this.GetHeader("ETag");


            public HttpDownloadFileResponse(
                int responseCode,
                IReadOnlyDictionary<string, string> headers,
                string outputPath)
            {
                ResponseCode = responseCode;
                Headers = headers ?? new Dictionary<string, string>();
                OutputPath = outputPath;
            }
        }
    }
}
